using Swashbuckle.AspNetCore.Annotations;

namespace TaskManager.Core.QueryFilters
{
    /// <summary>
    /// Filtro de búsqueda y paginación para asignaciones de tareas a usuarios.
    /// </summary>
    /// <remarks>
    /// Permite filtrar asignaciones por identificador de tarea o de usuario. 
    /// Incluye parámetros de paginación heredados de <see cref="PaginationQueryFilter"/>.
    /// </remarks>
    public class TaskAssignmentQueryFilter : PaginationQueryFilter
    {
        /// <summary>
        /// Filtrar por identificador de la tarea.
        /// </summary>
        [SwaggerSchema("Filtrar por identificador de la tarea")]
        /// <example>5</example>
        public int? TaskId { get; set; }

        /// <summary>
        /// Filtrar por identificador del usuario asignado.
        /// </summary>
        [SwaggerSchema("Filtrar por identificador del usuario asignado")]
        /// <example>7</example>
        public int? UserId { get; set; }
    }
}
