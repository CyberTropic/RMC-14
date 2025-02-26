using Content.Shared._RMC14.Item;
using Robust.Shared.GameStates;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Rules;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(RMCPlanetSystem))]
public sealed partial class RMCPlanetMapPrototypeComponent : Component
{

    [DataField(required: true), AutoNetworkedField, Access(Other = AccessPermissions.ReadExecute)]
    public ResPath Map;

    [DataField, AutoNetworkedField]
    public CamouflageType Camouflage = CamouflageType.Jungle;

    [DataField, AutoNetworkedField]
    public List<ScenarioBase> Scenarios = new();

    [DataField, AutoNetworkedField]
    public int MinPlayers;

    [DataField(required: true), AutoNetworkedField]
    public string Announcement = string.Empty;

    [DataRecord]
    public abstract class ScenarioBase;

    public sealed class ScenarioType : ScenarioBase
    {
        [DataField, AutoNetworkedField]
        public string ScenarioName = string.Empty;

        [DataField, AutoNetworkedField]
        public float Probability = 0f;
    }

    public sealed class ScenarioPick : ScenarioBase
    {
        [DataField, AutoNetworkedField]
        public List<ScenarioType> Pick = new();
    }
}
