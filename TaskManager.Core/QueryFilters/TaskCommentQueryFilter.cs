using Swashbuckle.AspNetCore.Annotations;

namespace TaskManager.Core.QueryFilters
{
    /// <summary>
    /// Filtro de búsqueda y paginación para comentarios de tareas (TaskComment).
    /// </summary>
    public class TaskCommentQueryFilter : PaginationQueryFilter
    {
        /// <summary>
        /// Identificador de la tarea.
        /// </summary>
        [SwaggerSchema("Identificador de la tarea")]
        public int? TaskId { get; set; }

        /// <summary>
        /// Identificador del usuario que realizó el comentario.
        /// </summary>
        [SwaggerSchema("Identificador del usuario")]
        public int? UserId { get; set; }

        /// <summary>
        /// Filtrar comentarios por contenido (coincidencia parcial).
        /// </summary>
        [SwaggerSchema("Texto del comentario")]
        public string? Comment { get; set; }
    }
}
