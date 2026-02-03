using Robust.Shared.GameStates;

namespace Content.Shared._RMC14.Power;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(SharedRMCPowerSystem))]
public sealed partial class RMCPowerProducerComponent : Component
{
    [DataField, AutoNetworkedField]
    public EntityUid? Area;

    [DataField, AutoNetworkedField]
    public EntityUid? Map;

    [DataField, AutoNetworkedField]
    public int Watts;

    [DataField, AutoNetworkedField]
    public bool Enabled;
}
