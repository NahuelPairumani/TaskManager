using TaskManager.Core.Entities;

namespace TaskManager.Core.Interfaces
{
    public interface ITaskEntityService
    {
        Task<IEnumerable<TaskEntity>> GetAllTasksAsync();
        Task<TaskEntity> GetTaskAsync(int id);
        Task InsertTaskAsync(TaskEntity task);
        Task UpdateTaskAsync(TaskEntity task);
        Task DeleteTaskAsync(TaskEntity task);

        // son funciones adicionales para busquedas y filtrados
        Task<IEnumerable<TaskEntity>> SearchTasksAsync(string searchText);
        Task<IEnumerable<TaskEntity>> GetAllProjectTasksAsync(int projectId, int? statusId = null);
    }
}
