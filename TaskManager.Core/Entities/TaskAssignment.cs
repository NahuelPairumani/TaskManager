using System;
using System.Collections.Generic;

namespace TaskManager.Core.Entities;

public partial class TaskAssignment : BaseEntity
{
    public int TaskId { get; set; }

    public int UserId { get; set; }

    public virtual TaskEntity Task { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
