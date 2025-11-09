namespace TaskManager.Core.Entities;

public partial class TaskComment : BaseEntity
{
    public int TaskId { get; set; }

    public int UserId { get; set; }

    public string Comment { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual TaskEntity Task { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
