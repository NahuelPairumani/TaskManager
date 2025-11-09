using Swashbuckle.AspNetCore.Annotations;

namespace TaskManager.Core.QueryFilters
{
    /// <summary>
    /// Filtro de búsqueda y paginación para tareas (TaskEntity).
    /// </summary>
    /// <remarks>
    /// Permite filtrar tareas por proyecto, estado, fecha de vencimiento, título, descripción o texto libre. 
    /// También incluye parámetros de paginación heredados de <see cref="PaginationQueryFilter"/>.
    /// </remarks>
    public class TaskEntityQueryFilter : PaginationQueryFilter
    {
        /// <summary>
        /// Identificador del proyecto al que pertenece la tarea.
        /// </summary>
        [SwaggerSchema("Identificador del proyecto al que pertenece la tarea")]
        /// <example>3</example>
        public int? ProjectId { get; set; }

        /// <summary>
        /// Identificador del estado de la tarea.
        /// </summary>
        [SwaggerSchema("Identificador del estado de la tarea")]
        /// <example>2</example>
        public int? StatusId { get; set; }

        /// <summary>
        /// Fecha de vencimiento de la tarea para filtrar.
        /// </summary>
        [SwaggerSchema("Fecha de vencimiento de la tarea")]
        /// <example>2025-11-15</example>
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Filtrar por título de la tarea (coincidencia parcial).
        /// </summary>
        [SwaggerSchema("Filtrar por título de la tarea")]
        /// <example>Actualizar documentación</example>
        public string? Title { get; set; }

        /// <summary>
        /// Filtrar por descripción de la tarea (coincidencia parcial).
        /// </summary>
        [SwaggerSchema("Filtrar por descripción de la tarea")]
        /// <example>Revisar comentarios XML</example>
        public string? Description { get; set; }

        /// <summary>
        /// Texto libre para buscar coincidencias en título o descripción.
        /// </summary>
        [SwaggerSchema("Texto libre para buscar coincidencias en título o descripción")]
        /// <example>documentación</example>
        public string? Texto { get; set; }
    }
}
    