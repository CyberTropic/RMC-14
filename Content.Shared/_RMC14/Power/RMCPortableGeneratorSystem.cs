using Content.Shared.Materials;

namespace Content.Shared._RMC14.Power;

public sealed partial class RMCPortableGeneratorSystem : EntitySystem
{
    [Dependency] private readonly SharedUserInterfaceSystem _uiSystem = default!;
    [Dependency] private readonly SharedMaterialStorageSystem _materialStorage = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<RMCPortableGeneratorComponent, RMCPortableGeneratorSetTargetPowerMessage>(OnTargetPowerSet);
        SubscribeLocalEvent<RMCPortableGeneratorComponent, RMCPortableGeneratorStartMessage>(OnStartRequested);
        SubscribeLocalEvent<RMCPortableGeneratorComponent, RMCPortableGeneratorStopMessage>(OnStopRequested);
        SubscribeLocalEvent<RMCPortableGeneratorComponent, RMCPortableGeneratorEjectFuelMessage>(OnEjectFuelRequested);
    }

    public void OnTargetPowerSet(EntityUid uid, RMCPortableGeneratorComponent component, RMCPortableGeneratorSetTargetPowerMessage args)
    {
        component.TargetPower = args.TargetPower;
    }

    public void OnStartRequested(EntityUid uid, RMCPortableGeneratorComponent component, RMCPortableGeneratorStartMessage args)
    {
        // component.RequestedState = true;
        Logger.Debug("Start requested");
    }

    public void OnStopRequested(EntityUid uid, RMCPortableGeneratorComponent component, RMCPortableGeneratorStopMessage args)
    {
        // component.RequestedState = false;
        Logger.Debug("Stop requested");
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
        if (!_uiSystem.IsUiOpen(uid, RMCPortableGeneratorUIKey.Key))
            return;

        var remainingFuel = _materialStorage.GetMaterialAmount(uid, comp.Material);

        // Send the shared RMCPortableGeneratorUiState to the client using the RMC UI key.
        _uiSystem.SetUiState(uid, RMCPortableGeneratorUIKey.Key,
            new RMCPortableGeneratorUiState(comp, remainingFuel));
    }
}

public sealed class RMCGeneratorEmpty
{
    public static readonly RMCGeneratorEmpty Instance = new();
}
