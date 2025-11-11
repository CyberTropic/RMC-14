using Content.Shared.Audio;
using Content.Shared.Materials;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Network;

namespace Content.Shared._RMC14.Power;

public sealed partial class RMCPortableGeneratorSystem : EntitySystem
{
    [Dependency] private readonly SharedUserInterfaceSystem _uiSystem = default!;
    [Dependency] private readonly SharedMaterialStorageSystem _materialStorage = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
    [Dependency] private readonly SharedAmbientSoundSystem _ambientSound = default!;
    [Dependency] private readonly INetManager _net = default!;

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
        component.TargetPower = args.TargetPower;
    }

    public void OnStartRequested(EntityUid uid,
        RMCPortableGeneratorComponent component,
        RMCPortableGeneratorStartMessage args)
    {
        Logger.Debug("Start requested");
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
        Logger.Debug("Stop requested");
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

        foreach (var comp in EntityManager.EntityQuery<RMCPortableGeneratorComponent>())
        {
            UpdateRmcPortableUi(comp.Owner, comp);
        }
    }

    private void UpdateRmcPortableUi(EntityUid uid, RMCPortableGeneratorComponent comp)
    {
        if (_net.IsClient)
            return;

        if (!_uiSystem.IsUiOpen(uid, RMCPortableGeneratorUIKey.Key))
            return;

        var remainingFuel = _materialStorage.GetMaterialAmount(uid, comp.Material);

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
