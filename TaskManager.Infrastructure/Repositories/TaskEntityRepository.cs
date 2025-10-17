using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;
using TaskManager.Infrastructure.Data;

namespace TaskManager.Infrastructure.Repositories
{
    public class TaskEntityRepository : BaseRepository<TaskEntity>, ITaskEntityRepository
    {
        //private readonly TaskManagerContext _context;

        public TaskEntityRepository(TaskManagerContext context) : base(context)
        {
            //_context = context;
        }

        public async Task<IEnumerable<TaskEntity>> Find(Expression<Func<TaskEntity, bool>> predicate)
        {
            var taskEntities = await _entities.Where(predicate).ToListAsync();
            return taskEntities;
        }
        /*
        public async Task<IEnumerable<TaskEntity>> GetAllTasksAsync()
        {
            var tasks = await _context.TaskEntities.ToListAsync();
            return tasks;
        }

        public async Task<TaskEntity> GetTaskAsync(int id)
        {
            var task = await _context.TaskEntities.FirstOrDefaultAsync(x => x.TaskId == id);
            return task;
        }

        public async Task InsertTaskAsync(TaskEntity task)
        {
            _context.TaskEntities.Add(task);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTaskAsync(TaskEntity task)
        {
            _context.TaskEntities.Update(task);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTaskAsync(TaskEntity task)
        {
            _context.TaskEntities.Remove(task);
            await _context.SaveChangesAsync();
        }
        */
    }
}
