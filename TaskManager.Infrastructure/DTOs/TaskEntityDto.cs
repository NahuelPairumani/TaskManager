namespace TaskManager.Infrastructure.DTOs
{
    /// <summary>
    /// Representa una tarea dentro del sistema.
    /// </summary>
    /// <remarks>
    /// Este DTO almacena la información principal de una tarea, 
    /// incluyendo su proyecto asociado, estado, título, descripción y fecha de vencimiento.
    /// Se utiliza para transferir datos entre las capas de la aplicación.
    /// </remarks>
    public class TaskEntityDto
    {
        /// <summary>
        /// Identificador único de la tarea.
        /// </summary>
        /// <example>1</example>
        public int Id { get; set; }

        /// <summary>
        /// Identificador del proyecto al que pertenece la tarea.
        /// </summary>
        /// <example>3</example>
        public int ProjectId { get; set; }

        /// <summary>
        /// Identificador del estado actual de la tarea (puede ser nulo si no se ha asignado).
        /// </summary>
        /// <example>2</example>
        public int? StatusId { get; set; }

        /// <summary>
        /// Título breve que describe la tarea.
        /// </summary>
        /// <example>Actualizar documentación del módulo de usuarios</example>
        public string Title { get; set; } = null!;

        /// <summary>
        /// Descripción detallada de la tarea.
        /// </summary>
        /// <example>Se debe revisar y actualizar los comentarios XML para los controladores de usuario.</example>
        public string Description { get; set; } = null!;

        /// <summary>
        /// Fecha límite para completar la tarea (en formato ISO 8601).
        /// </summary>
        /// <example>2025-11-15T23:59:00Z</example>
        public string? DueDate { get; set; }
    }
}
