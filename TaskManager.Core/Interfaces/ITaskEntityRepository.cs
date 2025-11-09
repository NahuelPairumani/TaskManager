using TaskManager.Core.CustomEntities;
using TaskManager.Core.Entities;

namespace TaskManager.Core.Interfaces
{
    /// <summary>
    /// Define las operaciones específicas del repositorio para la entidad <see cref="TaskEntity"/>.
    /// </summary>
    /// <remarks>
    /// Esta interfaz se encarga de la comunicación directa con la base de datos para las tareas, 
    /// incluyendo operaciones personalizadas con Dapper.
    /// </remarks>
    public interface ITaskEntityRepository : IBaseRepository<TaskEntity>
    {
        /// <summary>
        /// Obtiene todas las tareas desde la base de datos utilizando Dapper.
        /// </summary>
        /// <returns>Colección enumerable de <see cref="TaskEntity"/>.</returns>
        Task<IEnumerable<TaskEntity>> GetAllTasksDapperAsync();

        /// <summary>
        /// Obtiene una tarea por su identificador utilizando Dapper, 
        /// incluyendo el nombre del proyecto y del estado asociado.
        /// </summary>
        /// <param name="taskId">Identificador único de la tarea.</param>
        /// <returns>
        /// Objeto <see cref="TaskDetailResponse"/> con la información combinada 
        /// de la tarea, el proyecto y el estado, o <c>null</c> si no se encuentra.
        /// </returns>
        Task<TaskDetailResponse?> GetTaskByIdDapperAsync(int taskId);

        /// <summary>
        /// Obtiene el conteo de tareas agrupadas por estado dentro de un proyecto específico.
        /// </summary>
        /// <param name="projectId">Identificador del proyecto.</param>
        /// <returns>Colección de <see cref="TaskStatusCountResponse"/> con el conteo por estado.</returns>
        Task<IEnumerable<TaskStatusCountResponse>> GetTaskCountByStatusAsync(int projectId);
    }
}
