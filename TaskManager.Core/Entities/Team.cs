using System;
using System.Collections.Generic;

namespace TaskManager.Core.Entities;

public partial class Team : BaseEntity
{
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    public virtual ICollection<TeamUser> TeamUsers { get; set; } = new List<TeamUser>();
}
