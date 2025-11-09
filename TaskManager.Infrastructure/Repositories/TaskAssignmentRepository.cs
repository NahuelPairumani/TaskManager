using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SocialMedia.Core.Enum;
using TaskManager.Core.CustomEntities;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;
using TaskManager.Infrastructure.Data;
using TaskManager.Infrastructure.Queries;

namespace TaskManager.Infrastructure.Repositories
{
    public class TaskAssignmentRepository : BaseRepository<TaskAssignment>, ITaskAssignmentRepository
    {
        private readonly IDapperContext _dapper;

        public TaskAssignmentRepository(TaskManagerContext context, IDapperContext dapper) : base(context)
        {
            _dapper = dapper;
        }

        /// <summary>
        /// Obtiene todos los registros de TaskAssignment usando Dapper.
        /// </summary>
        public async Task<IEnumerable<TaskAssignment>> GetAllAssignmentsDapperAsync()
        {
            try
            {
                var sql = _dapper.Provider switch
                {
                    DatabaseProvider.SqlServer => TaskAssignmentQueries.GetAllTaskAssignmentsSqlServer,
                    DatabaseProvider.MySql => TaskAssignmentQueries.GetAllTaskAssignmentsMySql,
                    _ => throw new NotSupportedException("Proveedor no soportado")
                };

                return await _dapper.QueryAsync<TaskAssignment>(sql);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener los registros: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene un registro de TaskAssignment por su Id usando Dapper.
        /// </summary>
        public async Task<TaskAssignmentResponse?> GetByIdDapperAsync(int id)
        {
            try
            {
                var sql = _dapper.Provider switch
                {
                    DatabaseProvider.SqlServer => TaskAssignmentQueries.GetTaskAssignmentByIdSqlServer,
                    DatabaseProvider.MySql => TaskAssignmentQueries.GetTaskAssignmentByIdMySql,
                    _ => throw new NotSupportedException("Proveedor no soportado")
                };

                return await _dapper.QueryFirstOrDefaultAsync<TaskAssignmentResponse>(sql, new { Id = id });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el registro con Id={id}: {ex.Message}");
            }
        }
    }
}
