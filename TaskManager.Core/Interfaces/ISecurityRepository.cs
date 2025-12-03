using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;

namespace TaskManager.Core.Interfaces
{
    public interface ISecurityRepository : IBaseRepository<Security>
    {
        Task<Security> GetLoginByCredentials(UserLogin login);
    }
}
