using TaskManager.Core.CustomEntities;
using TaskManager.Core.Entities;
using TaskManager.Core.QueryFilters;

namespace TaskManager.Core.Interfaces
{
    /// <summary>
    /// Define las operaciones del servicio para la entidad <see cref="TaskComment"/>.
    /// </summary>
    public interface ITaskCommentService
    {
        /// <summary>
        /// Obtiene comentarios de tareas paginados según filtros.
        /// </summary>
        Task<ResponseData> GetAllCommentsDapperAsync(TaskCommentQueryFilter filters);

        /// <summary>
        /// Obtiene un comentario específico por su Id.
        /// </summary>
        Task<TaskComment> GetCommentByIdDapperAsync(int id);

        /// <summary>
        /// Obtiene todos los comentarios de una tarea especifica
        /// </summary>
        Task<IEnumerable<TaskComment>> GetCommentsByTaskIdAsync(int taskId);

        /// <summary>
        /// Inserta un nuevo comentario en la base de datos.
        /// </summary>
        Task InsertCommentAsync(TaskComment comment);

        /// <summary>
        /// Actualiza un comentario existente.
        /// </summary>
        Task UpdateCommentAsync(TaskComment comment);

        /// <summary>
        /// Elimina un comentario por su identificador.
        /// </summary>
        Task DeleteCommentAsync(int id);
    }
}
