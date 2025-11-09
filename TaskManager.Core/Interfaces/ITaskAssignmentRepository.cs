using TaskManager.Core.CustomEntities;
using TaskManager.Core.Entities;

namespace TaskManager.Core.Interfaces
{
    public interface ITaskAssignmentRepository : IBaseRepository<TaskAssignment>
    {
        Task<IEnumerable<TaskAssignment>> GetAllAssignmentsDapperAsync();
        Task<TaskAssignmentResponse?> GetByIdDapperAsync(int id);
    }
}
