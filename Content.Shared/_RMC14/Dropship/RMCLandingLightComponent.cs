using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Dropship;

[RegisterComponent]
[Access(typeof(SharedDropshipSystem))]
public sealed partial class RMCLandingLightComponent : Component
{
    [DataField]
    public EntityUid? LinkedDropshipDestination;

    [DataField]
    public bool On;
}

[Serializable, NetSerializable]
public enum RMCLandingLightVisuals
{
    Enabled
}
