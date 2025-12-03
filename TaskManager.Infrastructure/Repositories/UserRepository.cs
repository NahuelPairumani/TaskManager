using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using TaskManager.Core.Enum;
using System.Linq.Expressions;
using TaskManager.Core.CustomEntities;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;
using TaskManager.Infrastructure.Data;
using TaskManager.Infrastructure.Queries;

namespace TaskManager.Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        private readonly IDapperContext _dapper;

        public UserRepository(TaskManagerContext context, IDapperContext dapper) : base(context)
        {
            _dapper = dapper;
        }

        public async Task<IEnumerable<User>> Find(Expression<Func<User, bool>> predicate)
        {
            return await _entities.Where(predicate).ToListAsync();
        }

        /// <summary>
        /// Recupera todos los usuarios desde la base de datos utilizando Dapper.
        /// </summary>
        /// <returns>Lista de usuarios con nombre, apellido y correo electrónico.</returns>
        public async Task<IEnumerable<User>> GetAllUsersDapperAsync()
        {
            try
            {
                var sql = _dapper.Provider switch
                {
                    DatabaseProvider.SqlServer => UserQueries.GetAllUsersSqlServer,
                    DatabaseProvider.MySql => UserQueries.GetAllUsersMySql,
                    _ => throw new NotSupportedException("Proveedor de base de datos no soportado")
                };

                return await _dapper.QueryAsync<User>(sql);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener los usuarios: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene un usuario por su identificador utilizando Dapper
        /// </summary>
        /// <param name="id">Identificador único del usuario</param>
        /// <returns>Instancia de UserResponse con los datos del usuario</returns>
        public async Task<UserResponse?> GetUserByIdDapperAsync(int id)
        {
            try
            {
                var sql = _dapper.Provider switch
                {
                    DatabaseProvider.SqlServer => UserQueries.GetUserByIdSqlServer,
                    DatabaseProvider.MySql => UserQueries.GetUserByIdMySql,
                    _ => throw new NotSupportedException("Provider no soportado")
                };

                return await _dapper.QueryFirstOrDefaultAsync<UserResponse>(sql, new { Id = id });
            }
            catch (Exception err)
            {
                throw new Exception($"Error al obtener el usuario: {err.Message}");
            }
        }

    }
}
