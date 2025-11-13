using Content.Shared.Audio;
using Content.Shared.Materials;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Power;

public sealed partial class RMCPortableGeneratorSystem : EntitySystem
{
    [Dependency] private readonly SharedUserInterfaceSystem _uiSystem = default!;
    [Dependency] private readonly SharedMaterialStorageSystem _materialStorage = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
    [Dependency] private readonly SharedAmbientSoundSystem _ambientSound = default!;
    [Dependency] private readonly INetManager _net = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<RMCPortableGeneratorComponent, RMCPortableGeneratorSetTargetPowerMessage>(OnTargetPowerSet);
        SubscribeLocalEvent<RMCPortableGeneratorComponent, RMCPortableGeneratorStartMessage>(OnStartRequested);
        SubscribeLocalEvent<RMCPortableGeneratorComponent, RMCPortableGeneratorStopMessage>(OnStopRequested);
        SubscribeLocalEvent<RMCPortableGeneratorComponent, RMCPortableGeneratorEjectFuelMessage>(OnEjectFuelRequested);
        SubscribeLocalEvent<RMCPortableGeneratorComponent, AnchorStateChangedEvent>(OnAnchorStateChanged);
    }

    private void OnAnchorStateChanged(EntityUid uid, RMCPortableGeneratorComponent component, ref AnchorStateChangedEvent args)
    {
        if (!component.On)
            return;

        OnStopRequested(uid, component, new());
    }

    public void OnTargetPowerSet(EntityUid uid, RMCPortableGeneratorComponent component, RMCPortableGeneratorSetTargetPowerMessage args)
    {
        component.TargetPower =
            Math.Clamp(args.TargetPower, component.MinimumPower / 1000, component.MaximumPower / 1000) * 1000;
    }

    public void OnStartRequested(EntityUid uid,
        RMCPortableGeneratorComponent component,
        RMCPortableGeneratorStartMessage args)
    {
        if (!Transform(uid).Anchored)
            return;

        var empty = _materialStorage.GetMaterialAmount(uid, component.Material) <= 0;
        var sound = empty ? component.StartSoundEmpty : component.StartSound;
        _audio.PlayPvs(sound, uid);

        if (!empty)
        {
            component.On = true;
            UpdateState(uid, component);
        }
    }

    public void OnStopRequested(EntityUid uid, RMCPortableGeneratorComponent component, RMCPortableGeneratorStopMessage args)
    {
        component.On = false;
        UpdateState(uid, component);
    }

    public void OnEjectFuelRequested(EntityUid uid, RMCPortableGeneratorComponent component, RMCPortableGeneratorEjectFuelMessage args)
    {
        // EjectAllMaterial is in Server only, so we send an event to it
        RaiseLocalEvent(uid, RMCGeneratorEmpty.Instance);
    }

    public override void Update(float frameTime)
    {
        if (_net.IsClient)
            return;

        var query = EntityQueryEnumerator<RMCPortableGeneratorComponent>();
        while (query.MoveNext(out var uid, out var gen))
        {
            if (!gen.On)
            {
                UpdateRmcPortableUi(uid, gen);
                continue;
            }

            var powerRatio = gen.TargetPower / gen.MaximumPower;
            var consumeTotal = gen.MaterialPerSheet / gen.TimePerSheet * frameTime * powerRatio;
            gen.FractionalMaterial -= consumeTotal;

            if (gen.FractionalMaterial < 0)
            {
                var consumeWhole = -(int)MathF.Floor(gen.FractionalMaterial);

                if (_materialStorage.TryChangeMaterialAmount(uid, gen.Material, -consumeWhole))
                {
                    gen.FractionalMaterial += consumeWhole;
                }
                else
                {
                    gen.FractionalMaterial = 0f;
                    OnStopRequested(uid, gen, new());
                }
            }

            UpdateRmcPortableUi(uid, gen);
        }
    }

    public float GetTotalMaterial(EntityUid uid, RMCPortableGeneratorComponent component)
    {
        var totalMaterial = _materialStorage.GetMaterialAmount(uid, component.Material) + component.FractionalMaterial;
        return totalMaterial;
    }

    private void UpdateRmcPortableUi(EntityUid uid, RMCPortableGeneratorComponent comp)
    {
        if (!_uiSystem.IsUiOpen(uid, RMCPortableGeneratorUIKey.Key))
            return;

        var remainingFuel = GetTotalMaterial(uid, comp) / comp.MaterialPerSheet;

        _uiSystem.SetUiState(uid,
            RMCPortableGeneratorUIKey.Key,
            new RMCPortableGeneratorUiState(comp, remainingFuel));
    }

    private void UpdateState(EntityUid uid, RMCPortableGeneratorComponent component)
    {
        _appearance.SetData(uid, RMCGeneratorVisuals.Running, component.On);
        _ambientSound.SetAmbience(uid, component.On);
    }
}

public sealed class RMCGeneratorEmpty
{
    public static readonly RMCGeneratorEmpty Instance = new();
}
