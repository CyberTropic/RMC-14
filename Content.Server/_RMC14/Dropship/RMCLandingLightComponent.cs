namespace Content.Server._RMC14.Dropship;


[RegisterComponent]
[Access(typeof(DropshipSystem))]
public sealed partial class RMCLandingLightComponent : Component
{
    [DataField]
    public EntityUid? LinkedDropshipDestination;

    [DataField("on")]
    public bool On;
}
