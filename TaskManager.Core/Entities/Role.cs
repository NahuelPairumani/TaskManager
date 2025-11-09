namespace TaskManager.Core.Entities;

public partial class Role : BaseEntity
{
    public string RoleName { get; set; } = null!;

    public virtual ICollection<TeamUser> TeamUsers { get; set; } = new List<TeamUser>();
}
