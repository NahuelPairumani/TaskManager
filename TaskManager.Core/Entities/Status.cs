using System;
using System.Collections.Generic;

namespace TaskManager.Core.Entities;

public partial class Status : BaseEntity
{
    public string StatusName { get; set; } = null!;

    public virtual ICollection<TaskEntity> TaskEntities { get; set; } = new List<TaskEntity>();
}
