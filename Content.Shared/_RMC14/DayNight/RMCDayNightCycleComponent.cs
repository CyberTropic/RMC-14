using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.DayNight;

public sealed partial class RMCDayNightCycleComponent : Component
{
    [DataField, AutoNetworkedField]
    public TimeSpan TotalTransitionTime = TimeSpan.Zero;

    [DataField, AutoNetworkedField]
    public TimeSpan RemainingTransitionTime = TimeSpan.Zero;

    [DataField, AutoNetworkedField]
    public string PreviousColor = "#000000";

    [DataField, AutoNetworkedField]
    public string NextColor = "#000000";
}
