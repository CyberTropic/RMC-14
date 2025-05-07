using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Content.IntegrationTests;
using Content.Server.GameTicking;
using Content.Server.Maps;
using Robust.Client.GameObjects;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Robust.Shared.EntitySerialization.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Content.MapRenderer.Painters
{
    public sealed class MapPainter
    {
        public static async IAsyncEnumerable<RenderedGridImage<Rgba32>> Paint(string map, bool mapIsFilename = false)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            await using var pair = await PoolManager.GetServerClient(new PoolSettings
            {
                DummyTicker = false,
                Connected = true,
                Fresh = true,
                // Seriously whoever made MapPainter use GameMapPrototype I wish you step on a lego one time.
                Map = mapIsFilename ? "Empty" : map,
            });

            var server = pair.Server;
            var client = pair.Client;

            Console.WriteLine($"Loaded client and server in {(int) stopwatch.Elapsed.TotalMilliseconds} ms");

            stopwatch.Restart();

            var cEntityManager = client.ResolveDependency<IClientEntityManager>();
            var cPlayerManager = client.ResolveDependency<Robust.Client.Player.IPlayerManager>();

            await client.WaitPost(() =>
            {
                if (cEntityManager.TryGetComponent(cPlayerManager.LocalEntity, out SpriteComponent? sprite))
                {
                    sprite.Visible = false;
                }
            });

            var sEntityManager = server.ResolveDependency<IServerEntityManager>();
            var sPlayerManager = server.ResolveDependency<IPlayerManager>();

            var entityManager = server.ResolveDependency<IEntityManager>();
            var mapLoader = entityManager.System<MapLoaderSystem>();
            var mapSys = entityManager.System<SharedMapSystem>();

            Entity<MapComponent>? loadedMap = null;
            Entity<MapGridComponent>[] grids = [];

            var isPlanetMap = false;

            if (mapIsFilename)
            {
                await server.WaitPost(() =>
                {
                    if (mapLoader.TryLoadGeneric(new ResPath(map), out var loadedMaps, out var loadedGrids))
                    {
                        grids = loadedGrids.ToArray();
                        if (loadedMaps.Count > 0)
                            isPlanetMap = true;
                    }
                });
            }

            await pair.RunTicksSync(10);
            await Task.WhenAll(client.WaitIdleAsync(), server.WaitIdleAsync());

            var sMapManager = server.ResolveDependency<IMapManager>();

            var tilePainter = new TilePainter(client, server);
            var entityPainter = new GridPainter(client, server);
            var xformQuery = sEntityManager.GetEntityQuery<TransformComponent>();
            var xformSystem = sEntityManager.System<SharedTransformSystem>();

            await server.WaitPost(() =>
            {
                var playerEntity = sPlayerManager.Sessions.Single().AttachedEntity;

                if (playerEntity.HasValue)
                {
                    sEntityManager.DeleteEntity(playerEntity.Value);
                }

                if (!mapIsFilename)
                {
                    var mapId = sEntityManager.System<GameTicker>().DefaultMap;
                    grids = sMapManager.GetAllGrids(mapId).ToArray();
                }

                foreach (var (uid, _) in grids)
                {
                    var gridXform = xformQuery.GetComponent(uid);
                    xformSystem.SetWorldRotation(gridXform, Angle.Zero);
                }
            });

            await pair.RunTicksSync(10);
            await Task.WhenAll(client.WaitIdleAsync(), server.WaitIdleAsync());

            foreach (var (uid, grid) in grids)
            {
                var tileXSize = grid.TileSize * TilePainter.TileImageSize;
                var tileYSize = grid.TileSize * TilePainter.TileImageSize;
                int w, h;
                var customOffset = new Vector2();

                if (isPlanetMap)
                {
                    var tiles = mapSys.GetAllTiles(uid, grid).ToList();
                    if (tiles.Count == 0)
                    {
                        Console.WriteLine($"Warning: Grid {uid} was empty. Skipping image rendering.");
                        continue;
                    }

                    var minX = tiles.Min(t => t.X);
                    var minY = tiles.Min(t => t.Y);
                    var maxX = tiles.Max(t => t.X);
                    var maxY = tiles.Max(t => t.Y);
                    w = (maxX - minX + 1) * tileXSize;
                    h = (maxY - minY + 1) * tileYSize;
                    customOffset = new Vector2(-minX, -minY);
                }
                else
                {
                    // Skip empty grids
                    if (grid.LocalAABB.IsEmpty())
                    {
                        Console.WriteLine($"Warning: Grid {uid} was empty. Skipping image rendering.");
                        continue;
                    }

                    var bounds = grid.LocalAABB;

                    var left = bounds.Left;
                    var right = bounds.Right;
                    var top = bounds.Top;
                    var bottom = bounds.Bottom;

                    w = (int) Math.Ceiling(right - left) * tileXSize;
                    h = (int) Math.Ceiling(top - bottom) * tileYSize;
                }

                var gridCanvas = new Image<Rgba32>(w, h);

                await server.WaitPost(() =>
                {
                    tilePainter.Run(gridCanvas, uid, grid, customOffset);
                    entityPainter.Run(gridCanvas, uid, grid, customOffset);

                    gridCanvas.Mutate(e => e.Flip(FlipMode.Vertical));
                });

                var renderedImage = new RenderedGridImage<Rgba32>(gridCanvas)
                {
                    GridUid = uid,
                    Offset = xformSystem.GetWorldPosition(uid),
                };

                yield return renderedImage;
            }

            // We don't care if it fails as we have already saved the images.
            try
            {
                await pair.CleanReturnAsync();
            }
            catch
            {
                // ignored
            }
        }
    }
}
