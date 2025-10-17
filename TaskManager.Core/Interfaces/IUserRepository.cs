using System.Linq.Expressions;
using TaskManager.Core.Entities;

namespace TaskManager.Core.Interfaces
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<IEnumerable<User>> Find(Expression<Func<User, bool>> predicate);
    }
}
