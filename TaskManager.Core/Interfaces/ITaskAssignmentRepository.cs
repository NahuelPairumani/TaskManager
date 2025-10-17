using TaskManager.Core.Entities;

namespace TaskManager.Core.Interfaces
{
    public interface ITaskAssignmentRepository : IBaseRepository<TaskAssignment>
    {
        Task<IEnumerable<TaskAssignment>> GetAssignmentsByTaskIdAsync(int taskId);
        Task<IEnumerable<TaskAssignment>> GetAssignmentsByUserIdAsync(int userId);
        Task<TaskAssignment> GetAssignmentAsync(int taskId, int userId);
    }
}
