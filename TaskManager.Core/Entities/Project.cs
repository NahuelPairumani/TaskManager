using System;
using System.Collections.Generic;

namespace TaskManager.Core.Entities;

public partial class Project : BaseEntity
{
    public int TeamId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public virtual ICollection<TaskEntity> TaskEntities { get; set; } = new List<TaskEntity>();

    public virtual Team Team { get; set; } = null!;
}
