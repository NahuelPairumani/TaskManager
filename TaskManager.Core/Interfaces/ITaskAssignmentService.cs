using TaskManager.Core.Entities;

namespace TaskManager.Core.Interfaces
{
    public interface ITaskAssignmentService
    {
        Task<IEnumerable<TaskAssignment>> GetAllAssignmentsAsync();
        Task<TaskAssignment> GetAssignmentAsync(int id);
        Task AssignTaskToUserAsync(TaskAssignment task);
        Task RemoveAssignmentAsync(int id);
        Task<IEnumerable<TaskEntity>> GetTasksByUserAsync(int userId);
        Task<IEnumerable<User>> GetUsersByTaskAsync(int taskId);
    }
}
