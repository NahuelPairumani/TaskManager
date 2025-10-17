using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;
using TaskManager.Infrastructure.Data;

namespace TaskManager.Infrastructure.Repositories
{
    public class TaskAssignmentRepository : BaseRepository<TaskAssignment>, ITaskAssignmentRepository
    {
        public TaskAssignmentRepository(TaskManagerContext context) : base(context)
        {
        }

        public async Task<IEnumerable<TaskAssignment>> GetAssignmentsByTaskIdAsync(int taskId)
        {
            return await _entities.Where(a => a.TaskId == taskId).ToListAsync();
        }

        public async Task<IEnumerable<TaskAssignment>> GetAssignmentsByUserIdAsync(int userId)
        {
            return await _entities.Where(a => a.UserId == userId).ToListAsync();
        }

        public async Task<TaskAssignment> GetAssignmentAsync(int taskId, int userId)
        {
            return await _entities.FirstOrDefaultAsync(a => a.TaskId == taskId && a.UserId == userId);
        }
    }
}
