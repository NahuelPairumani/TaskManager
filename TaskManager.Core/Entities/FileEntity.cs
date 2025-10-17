using System;
using System.Collections.Generic;

namespace TaskManager.Core.Entities;

public partial class FileEntity : BaseEntity
{
    public string FileName { get; set; } = null!;

    public string FileType { get; set; } = null!;

    public string FileUrl { get; set; } = null!;

    public DateTime? UploadedAt { get; set; }

    public virtual ICollection<FileAssignment> FileAssignments { get; set; } = new List<FileAssignment>();
}
