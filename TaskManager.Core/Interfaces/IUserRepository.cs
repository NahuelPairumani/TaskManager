using System.Linq.Expressions;
using TaskManager.Core.CustomEntities;
using TaskManager.Core.Entities;

namespace TaskManager.Core.Interfaces
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<IEnumerable<User>> Find(Expression<Func<User, bool>> predicate);
        /// <summary>
        /// Recupera todos los usuarios 
        /// nte una consulta SQL usando Dapper.
        /// </summary>
        Task<IEnumerable<User>> GetAllUsersDapperAsync();

        /// <summary>
        /// Obtiene un usuario por su identificador utilizando Dapper
        /// </summary>
        /// <param name="id">Identificador único del usuario</param>
        /// <returns>Instancia de UserResponse con los datos del usuario</returns>
        Task<UserResponse?> GetUserByIdDapperAsync(int id);
    }
}
