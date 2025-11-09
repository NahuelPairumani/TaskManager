using TaskManager.Core.CustomEntities;
using TaskManager.Core.Entities;
using TaskManager.Core.QueryFilters;

namespace TaskManager.Core.Interfaces
{
    public interface IUserService
    {
        Task<ResponseData> GetAllUsersAsync(UserQueryFilter filters);
        Task<User> GetUserAsync(int id);

        /// <summary>
        /// Obtiene todos los usuarios usando Dapper.
        /// </summary>
        Task<ResponseData> GetAllUsersDapperAsync(UserQueryFilter filters);

        /// <summary>
        /// Recupera un usuario específico utilizando Dapper
        /// </summary>
        /// <param name="id">Identificador del usuario</param>
        /// <returns>Objeto ResponseData con la información del usuario</returns>
        Task<ResponseData> GetUserByIdDapperAsync(int id);
        Task InsertUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(int id);
    }
}
