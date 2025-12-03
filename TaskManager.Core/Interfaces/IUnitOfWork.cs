using System.Data;
using TaskManager.Core.Entities;

namespace TaskManager.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ITaskEntityRepository TaskEntityRepository { get; }
        IBaseRepository<Project> ProjectRepository { get; }
        IBaseRepository<Status> StatusRepository { get; }
        ITaskAssignmentRepository TaskAssignmentRepository { get; }
        IUserRepository UserRepository { get; }
        ITaskCommentRepository TaskCommentRepository { get; }
        ISecurityRepository SecurityRepository { get; }

        void SaveChanges();
        Task SaveChangesAsync();

        Task BeginTransaccionAsync();
        Task CommitAsync();
        Task RollbackAsync();

        IDbConnection? GetDbConnection();
        IDbTransaction? GetDbTransaction();
    }
}
