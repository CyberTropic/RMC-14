using Content.Shared.Mind;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.Roles;

/// <summary>
/// This holds data for, and indicates, a Mind Role entity
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class MindRoleComponent : BaseMindRoleComponent
{
    /// <summary>
    ///     Marks this Mind Role as Antagonist
    ///     A single antag Mind Role is enough to make the owner mind count as Antagonist.
    /// </summary>
    [DataField]
    public bool Antag;

    /// <summary>
    ///     The mind's current antagonist/special role, or lack thereof;
    /// </summary>
    [DataField]
    public ProtoId<RoleTypePrototype>? RoleType;

    /// <summary>
    ///     The role's subtype, shown only to admins to help with antag categorization
    /// </summary>
    [DataField]
    public LocId? Subtype;

    /// <summary>
    ///     True if this mindrole is an exclusive antagonist. Antag setting is not checked if this is True.
    /// </summary>
    [DataField]
    public bool ExclusiveAntag;

    /// <summary>
    ///     The Mind that this role belongs to
    /// </summary>
    public Entity<MindComponent> Mind { get; set; }

    /// <summary>
    ///     The Antagonist prototype of this role
    /// </summary>
    [DataField]
    public ProtoId<AntagPrototype>? AntagPrototype { get; set; }

    /// <summary>
    ///     The Job prototype of this role
    /// </summary>
    [DataField, AutoNetworkedField]
    public ProtoId<JobPrototype>? JobPrototype { get; set; }

    /// <summary>
    ///     Used to order the characters on by role/antag status. Highest numbers are shown first.
    /// </summary>
    [DataField]
    public int SortWeight;
}

// Why does this base component actually exist? It does make auto-categorization easy, but before that it was useless?
// I used it for easy organisation/bookkeeping of what components are for mindroles
[EntityCategory("Roles")]
public abstract partial class BaseMindRoleComponent : Component
{

}
