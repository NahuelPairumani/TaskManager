using System;
using System.Collections.Generic;

namespace TaskManager.Core.Entities;

public partial class TaskEntity : BaseEntity
{
    //public int TaskId { get; set; }

    public int ProjectId { get; set; }

    public int StatusId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime? DueDate { get; set; }

    public virtual ICollection<FileAssignment> FileAssignments { get; set; } = new List<FileAssignment>();

    public virtual Project Project { get; set; } = null!;

    public virtual Status Status { get; set; } = null!;

    public virtual ICollection<TaskAssignment> TaskAssignments { get; set; } = new List<TaskAssignment>();

    public virtual ICollection<TaskComment> TaskComments { get; set; } = new List<TaskComment>();
}
