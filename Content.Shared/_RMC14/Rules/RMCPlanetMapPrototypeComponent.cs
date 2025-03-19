using Content.Shared._RMC14.Item;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
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
    public int MinPlayers;

    [DataField(required: true), AutoNetworkedField]
    public string Announcement = string.Empty;

    [DataField, AutoNetworkedField]
    public List<MapInsertScenario> ScenarioList = new();

    [DataDefinition]
    [Serializable, NetSerializable]
    public partial struct MapInsertScenario()
    {
        [DataField, AutoNetworkedField]
        public List<string> ScenarioNames = new();

        [DataField, AutoNetworkedField]
        public List<float> ScenarioProbabilities = new();
    }
}
