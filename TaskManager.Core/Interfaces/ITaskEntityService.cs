using TaskManager.Core.CustomEntities;
using TaskManager.Core.Entities;
using TaskManager.Core.QueryFilters;

namespace TaskManager.Core.Interfaces
{
    /// <summary>
    /// Define las operaciones del servicio para la entidad <see cref="TaskEntity"/>.
    /// </summary>
    /// <remarks>
    /// Esta interfaz declara los métodos que gestionan las operaciones de negocio 
    /// relacionadas con las tareas, incluyendo recuperación, inserción, actualización y eliminación.
    /// </remarks>
    public interface ITaskEntityService
    {
        /// <summary>
        /// Obtiene una lista paginada de tareas según los filtros especificados.
        /// </summary>
        /// <param name="filters">Filtros que pueden incluir parámetros como paginación, búsqueda o proyecto.</param>
        /// <returns>Objeto <see cref="ResponseData"/> que contiene la lista de tareas y metadatos de paginación.</returns>
        Task<ResponseData> GetAllTasksAsync(TaskEntityQueryFilter filters);

        /// <summary>
        /// Obtiene una tarea específica por su identificador.
        /// </summary>
        /// <param name="id">Identificador único de la tarea.</param>
        /// <returns>Objeto <see cref="TaskEntity"/> con los detalles de la tarea.</returns>
        Task<TaskEntity> GetTaskAsync(int id);

        /// <summary>
        /// Obtiene una lista de tareas utilizando Dapper con filtros personalizados.
        /// </summary>
        /// <param name="filters">Filtros para filtrar las tareas.</param>
        /// <returns>Objeto <see cref="ResponseData"/> con la colección de tareas resultante.</returns>
        Task<ResponseData> GetAllTasksDapperAsync(TaskEntityQueryFilter filters);

        /// <summary>
        /// Recupera una tarea por su Id incluyendo el estado y el proyecto al que pertenece.
        /// </summary>
        /// <param name="taskId">Identificador de la tarea.</param>
        /// <returns>Objeto <see cref="ResponseData"/> con los datos de la tarea.</returns>
        Task<ResponseData> GetTaskByIdDapperAsync(int taskId);

        /// <summary>
        /// Obtiene el conteo de tareas agrupadas por estado dentro de un proyecto determinado.
        /// </summary>
        /// <param name="projectId">Identificador del proyecto.</param>
        /// <param name="pageNumber">Número de página solicitada.</param>
        /// <param name="pageSize">Cantidad de registros por página.</param>
        /// <returns>Objeto <see cref="ResponseData"/> con el conteo de tareas por estado.</returns>
        Task<ResponseData> GetTaskCountByStatusAsync(int projectId, int pageNumber, int pageSize);

        Task<ResponseData> GetUsersAssignedToProjectAsync(int projectId, int pageNumber, int pageSize);

        /// <summary>
        /// Inserta una nueva tarea en el sistema.
        /// </summary>
        /// <param name="task">Entidad <see cref="TaskEntity"/> con los datos de la tarea a registrar.</param>
        Task InsertTaskAsync(TaskEntity task);

        /// <summary>
        /// Actualiza una tarea existente.
        /// </summary>
        /// <param name="task">Entidad <see cref="TaskEntity"/> con los datos actualizados.</param>
        Task UpdateTaskAsync(TaskEntity task);

        /// <summary>
        /// Elimina una tarea por su identificador.
        /// </summary>
        /// <param name="id">Identificador de la tarea a eliminar.</param>
        Task DeleteTaskAsync(int id);

        // Métodos adicionales de búsqueda o filtrado pueden agregarse aquí.
    }
}
