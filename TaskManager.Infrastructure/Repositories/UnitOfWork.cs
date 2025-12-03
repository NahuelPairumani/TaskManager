using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;
using TaskManager.Infrastructure.Data;

namespace TaskManager.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TaskManagerContext _context;
        public readonly ITaskEntityRepository? _taskEntityRepository;
        public readonly ITaskAssignmentRepository? _taskAssignmentRepository;
        public readonly IUserRepository? _userRepository;
        public readonly IBaseRepository<Project>? _projectRepository;
        public readonly IBaseRepository<Status>? _statusRepository;
        public readonly ITaskCommentRepository? _taskCommentRepository;
        public readonly ISecurityRepository _securityRepository;

        public readonly IDapperContext _dapper;
        private IDbContextTransaction? _efTransaction;

        public UnitOfWork(TaskManagerContext context, IDapperContext dapper)
        {
            _context = context;
            _dapper = dapper;
        }

        public ITaskEntityRepository TaskEntityRepository =>
            _taskEntityRepository ?? new TaskEntityRepository(_context, _dapper);

        public IUserRepository UserRepository =>
            _userRepository ?? new UserRepository(_context, _dapper);

        public ITaskAssignmentRepository TaskAssignmentRepository =>
            _taskAssignmentRepository ?? new TaskAssignmentRepository(_context, _dapper);

        public IBaseRepository<Project> ProjectRepository =>
           _projectRepository ?? new BaseRepository<Project>(_context);

        public IBaseRepository<Status> StatusRepository =>
            _statusRepository ?? new BaseRepository<Status>(_context);

        public ITaskCommentRepository TaskCommentRepository =>
            _taskCommentRepository ?? new TaskCommentRepository(_context, _dapper);

        public ISecurityRepository SecurityRepository =>
            _securityRepository ?? new SecurityRepository(_context, _dapper);

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

        public async Task BeginTransaccionAsync()
        {
            if (_efTransaction == null)
            {
                _efTransaction = await _context.Database.BeginTransactionAsync();

                //Registrar coneccion/tx DapperContext
                var conn = _context.Database.GetDbConnection();
                var tx = _efTransaction.GetDbTransaction();
                _dapper.SetAmbientConnection(conn, tx);
            }
        }

        public async Task CommitAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                if (_efTransaction != null)
                {
                    await _efTransaction.CommitAsync();
                    _efTransaction.Dispose();
                    _efTransaction = null;
                }
            }
            finally
            {
                _dapper.ClearAmbientConnection();
            }
        }

        public async Task RollbackAsync()
        {
            if (_efTransaction != null)
            {
                await _efTransaction.RollbackAsync();
                _efTransaction.Dispose();
                _efTransaction = null;
            }

            _dapper.ClearAmbientConnection();
        }

        public IDbConnection? GetDbConnection()
        {
            //Retornar la coneccion subyacente del DbContext
            return _context.Database.GetDbConnection();
        }

        public IDbTransaction? GetDbTransaction()
        {
            return _efTransaction?.GetDbTransaction();
        }
    }
}
