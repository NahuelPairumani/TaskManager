namespace TaskManager.Core.Entities
{
    /// <summary>
    /// Representa una tarea dentro del sistema.
    /// </summary>
    /// <remarks>
    /// Esta entidad almacena información principal de una tarea, incluyendo su proyecto asociado, estado,
    /// asignaciones a usuarios, comentarios y archivos relacionados.
    /// </remarks>
    public partial class TaskEntity : BaseEntity
    {
        /// <summary>
        /// Identificador del proyecto al que pertenece la tarea.
        /// </summary>
        /// <example>3</example>
        public int ProjectId { get; set; }

        /// <summary>
        /// Identificador del estado de la tarea.
        /// </summary>
        /// <example>2</example>
        public int StatusId { get; set; }

        /// <summary>
        /// Título de la tarea.
        /// </summary>
        /// <example>Actualizar documentación de API</example>
        public string Title { get; set; } = null!;

        /// <summary>
        /// Descripción detallada de la tarea.
        /// </summary>
        /// <example>Revisar y documentar todos los endpoints de la API.</example>
        public string Description { get; set; } = null!;

        /// <summary>
        /// Fecha de vencimiento de la tarea.
        /// </summary>
        /// <example>2025-11-15</example>
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Colección de archivos asociados a la tarea.
        /// </summary>
        public virtual ICollection<FileAssignment> FileAssignments { get; set; } = new List<FileAssignment>();

        /// <summary>
        /// Proyecto al que pertenece la tarea.
        /// </summary>
        public virtual Project Project { get; set; } = null!;

        /// <summary>
        /// Estado actual de la tarea.
        /// </summary>
        public virtual Status Status { get; set; } = null!;

        /// <summary>
        /// Colección de asignaciones de usuarios a la tarea.
        /// </summary>
        public virtual ICollection<TaskAssignment> TaskAssignments { get; set; } = new List<TaskAssignment>();

        /// <summary>
        /// Colección de comentarios registrados para la tarea.
        /// </summary>
        public virtual ICollection<TaskComment> TaskComments { get; set; } = new List<TaskComment>();
    }
}
