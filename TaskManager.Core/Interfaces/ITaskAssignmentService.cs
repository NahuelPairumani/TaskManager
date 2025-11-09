using TaskManager.Core.CustomEntities;
using TaskManager.Core.Entities;
using TaskManager.Core.QueryFilters;

namespace TaskManager.Core.Interfaces
{
    public interface ITaskAssignmentService
    {
        Task<ResponseData> GetAllAssignmentsAsync(TaskAssignmentQueryFilter filters);
        Task<TaskAssignment> GetAssignmentAsync(int id);
        Task<ResponseData> GetAllAssignmentsDapperAsync(TaskAssignmentQueryFilter filters);
        Task<ResponseData> GetByIdDapperAsync(int id);
        Task AssignTaskToUserAsync(TaskAssignment task);
        Task RemoveAssignmentAsync(int id);

        // Task<IEnumerable<TaskEntity>> GetTasksByUserAsync(int userId);
        // Task<IEnumerable<User>> GetUsersByTaskAsync(int taskId);
    }
}
