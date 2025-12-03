using TaskManager.Core.Entities;

namespace TaskManager.Core.Interfaces
{
    public interface ISecurityServices
    {
        Task<Security> GetLoginByCredentials(UserLogin login);

        Task RegisterUser(Security security);
    }
}
