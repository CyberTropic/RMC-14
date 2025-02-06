using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.DayNight;

public sealed partial class RMCDayNightCycleComponent : Component
{
    [DataField, AutoNetworkedField]
    public TimeSpan TotalTransitionTime = TimeSpan.Zero;

    [DataField, AutoNetworkedField]
    public TimeSpan RemainingTransitionTime = TimeSpan.Zero;

    [DataField, AutoNetworkedField]
    public List<TimeEntry> TransitionValues = [];

    [DataDefinition, NetSerializable, Serializable]
    public sealed partial class TimeEntry
    {
        [DataField("colorHex")]
        public string ColorHex { get; set; } = "#FFFFFF";

        [DataField("time")]
        public float Time { get; set; } // Normalized time (0-1)
    }
}
