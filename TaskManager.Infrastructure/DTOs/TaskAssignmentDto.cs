namespace TaskManager.Infrastructure.DTOs
{
    /// <summary>
    /// Representa una asignación de tarea a un usuario dentro del sistema.
    /// </summary>
    /// <remarks>
    /// Este DTO asocia una tarea con un usuario específico, indicando quién es responsable de realizarla.
    /// Se utiliza para manejar las relaciones entre usuarios y tareas.
    /// </remarks>
    public class TaskAssignmentDto
    {
        /// <summary>
        /// Identificador único de la asignación.
        /// </summary>
        /// <example>10</example>
        public int Id { get; set; }

        /// <summary>
        /// Identificador de la tarea asignada.
        /// </summary>
        /// <example>5</example>
        public int TaskId { get; set; }

        /// <summary>
        /// Identificador del usuario asignado a la tarea.
        /// </summary>
        /// <example>7</example>
        public int UserId { get; set; }
    }
}
