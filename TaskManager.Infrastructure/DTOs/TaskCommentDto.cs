namespace TaskManager.Infrastructure.DTOs
{
    /// <summary>
    /// Representa un comentario asociado a una tarea dentro del sistema.
    /// </summary>
    public class TaskCommentDto
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public int UserId { get; set; }
        public string Comment { get; set; } = null!;

        /// <summary>
        /// Fecha y hora en que se creó el comentario (solo lectura).
        /// </summary>
        public string? CreatedAt { get; set; }
    }
}
