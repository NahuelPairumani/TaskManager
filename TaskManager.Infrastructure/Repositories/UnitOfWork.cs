using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;
using TaskManager.Infrastructure.Data;

namespace TaskManager.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TaskManagerContext _context;
        public readonly ITaskEntityRepository _taskEntityRepository;
        public readonly IBaseRepository<Project> _projectRepository;
        public readonly IBaseRepository<Status> _statusRepository;

        public readonly ITaskAssignmentRepository _taskAssignmentRepository;
        public readonly IUserRepository _userRepository;



        public UnitOfWork(TaskManagerContext context)
        {
            _context = context;
        }

        public ITaskEntityRepository TaskEntityRepository =>
            _taskEntityRepository ?? new TaskEntityRepository(_context);

        public IBaseRepository<Project> ProjectRepository =>
           _projectRepository ?? new BaseRepository<Project>(_context);

        public IBaseRepository<Status> StatusRepository =>
            _statusRepository ?? new BaseRepository<Status>(_context);

        public ITaskAssignmentRepository TaskAssignmentRepository =>
            _taskAssignmentRepository ?? new TaskAssignmentRepository(_context);

        public IUserRepository UserRepository =>
            _userRepository ?? new UserRepository(_context);

        public void Dispose()
        {
            if (_context != null)
            {
                _context.Dispose();
            }
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
