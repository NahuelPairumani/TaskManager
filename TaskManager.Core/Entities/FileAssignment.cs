using System;
using System.Collections.Generic;

namespace TaskManager.Core.Entities;

public partial class FileAssignment : BaseEntity
{
    public int TaskId { get; set; }

    public int FileId { get; set; }

    public virtual FileEntity File { get; set; } = null!;

    public virtual TaskEntity Task { get; set; } = null!;
}
