using System.Linq.Expressions;
using TaskManager.Core.Entities;

namespace TaskManager.Core.Interfaces
{
    public interface ITaskEntityRepository : IBaseRepository<TaskEntity>
    {
        Task<IEnumerable<TaskEntity>> Find(Expression<Func<TaskEntity, bool>> predicate);
    }
}