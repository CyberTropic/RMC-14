using Content.Shared.Stacks;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Power;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
// [Access(typeof(RMCPortableGeneratorSystem))]
public sealed partial class RMCPortableGeneratorComponent : Component
{
    [DataField, AutoNetworkedField]
    public bool On;

    [DataField]
    public ProtoId<StackPrototype>? Material = "CMPhoron";

    [DataField]
    public int MaxFuelUnits = 100;

    [DataField]
    public float TimePerFuelUnit = 60.0f; // Seconds per fuel unit.

    [DataField]
    public float TargetPower = 15000.0f;

    [DataField]
    public float MaxTargetPower = 30000.0f;
}


[Serializable, NetSerializable]
public enum RMCPortableGeneratorUIKey : byte
{
    Key
}


[Serializable, NetSerializable]
public sealed class RMCPortableGeneratorUiState : BoundUserInterfaceState
{
    public float RemainingFuel;
    public float TargetPower;
    public float MaximumPower;
    public float OptimalPower;
    public bool On;

    public RMCPortableGeneratorUiState(
        RMCPortableGeneratorComponent component)
    {
        // RemainingFuel = remainingFuel;
        TargetPower = component.TargetPower;
        MaximumPower = component.MaxTargetPower;
        // OptimalPower = component.OptimalPower;
        On = component.On;
    }
}

[Serializable, NetSerializable]
public sealed class RMCPortableGeneratorSetTargetPowerMessage(int targetPower) : BoundUserInterfaceMessage
{
    public int TargetPower = targetPower;
}

[Serializable, NetSerializable]
public sealed class RMCPortableGeneratorStartMessage : BoundUserInterfaceMessage
{
}

[Serializable, NetSerializable]
public sealed class RMCPortableGeneratorStopMessage : BoundUserInterfaceMessage
{
}

[Serializable, NetSerializable]
public sealed class RMCPortableGeneratorEjectFuelMessage : BoundUserInterfaceMessage
{
}
