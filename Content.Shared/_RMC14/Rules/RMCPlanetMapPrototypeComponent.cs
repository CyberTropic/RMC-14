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
    public List<ScenarioEntry> Scenarios = new();

    [DataField, AutoNetworkedField]
    public int MinPlayers;

    [DataField(required: true), AutoNetworkedField]
    public string Announcement = string.Empty;

    [DataDefinition]
    [Serializable, NetSerializable]
    public partial struct ScenarioEntry()
    {
        [DataField, AutoNetworkedField]
        public ScenarioEntryType Type = ScenarioEntryType.Regular;

        // For Regular type
        [DataField, AutoNetworkedField]
        public string ScenarioName = string.Empty;

        [DataField, AutoNetworkedField]
        public float Probability = 0f;

        // For Choice type
        [DataField, AutoNetworkedField]
        public List<ScenarioChoice> ScenarioList = new();
    }


    [DataDefinition]
    [Serializable, NetSerializable]
    public partial struct ScenarioChoice()
    {
        [DataField, AutoNetworkedField]
        public string ScenarioName = string.Empty;

        [DataField, AutoNetworkedField]
        public float Probability = 0f;
    }

    public enum ScenarioEntryType
    {
        Regular,
        Choice
    }
}
