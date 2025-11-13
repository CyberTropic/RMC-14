using Content.Shared.Materials;
using Content.Shared.Stacks;
using Robust.Shared.Audio;
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
    public ProtoId<MaterialPrototype>  Material = "RMCPhoron";

    [DataField]
    public int MaterialPerSheet = 100;

    [DataField]
    public string MaterialName = "Phoron";

    [DataField]
    public int MaxFuelUnits = 100;

    [DataField]
    public float TimePerSheet = 60.0f;

    [DataField]
    public float TargetPower = 15000.0f;

    [DataField]
    public float MaximumPower = 30000.0f;

    [DataField]
    public float MinimumPower = 1_000;

    [DataField]
    public SoundSpecifier StartSoundEmpty = new SoundCollectionSpecifier("GeneratorTugEmpty");

    [DataField]
    public SoundSpecifier StartSound = new SoundCollectionSpecifier("GeneratorTug");

    [DataField]
    public float FractionalMaterial;
}


[Serializable, NetSerializable]
public enum RMCPortableGeneratorUIKey : byte
{
    Key
}


[Serializable, NetSerializable]
public sealed class RMCPortableGeneratorUiState(
    RMCPortableGeneratorComponent component,
    float remainingFuel)
    : BoundUserInterfaceState
{
    public float RemainingFuel = remainingFuel;
    public float TargetPower = component.TargetPower;
    public float MaximumPower = component.MaximumPower;
    public string FuelType = component.MaterialName;
    public bool On = component.On;
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

[Serializable, NetSerializable]
public enum RMCGeneratorVisuals : byte
{
    Running,
}
