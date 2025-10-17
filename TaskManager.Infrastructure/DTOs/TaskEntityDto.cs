namespace TaskManager.Infrastructure.DTOs
{
    public class TaskEntityDto
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int? StatusId { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? DueDate { get; set; }
    }
}
