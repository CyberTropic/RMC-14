using Content.Shared.Stacks;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Power;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState, Access(typeof(RMCPortableGeneratorSystem))]
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
}
