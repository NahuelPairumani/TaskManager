using TaskManager.Core.Entities;

namespace TaskManager.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ITaskEntityRepository TaskEntityRepository { get; }
        IBaseRepository<Project> ProjectRepository { get; }
        IBaseRepository<Status> StatusRepository { get; }

        //IBaseRepository<User> UserRepository { get; }
        ITaskAssignmentRepository TaskAssignmentRepository { get; }

        IUserRepository UserRepository { get; }

        void SaveChanges();
        Task SaveChangesAsync();
    }
}
