using System.Net;
using System.Security.Cryptography;
using System.Text;
using TaskManager.Core.CustomEntities;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;
using TaskManager.Core.QueryFilters;

namespace TaskManager.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseData> GetAllUsersAsync(UserQueryFilter filters)
        {
            var users = await _unitOfWork.UserRepository.GetAll();

            if (filters.FirstName != null)
            {
                users = users.Where(t => t.FirstName.ToLower().Contains(filters.FirstName.ToLower()));
            }
            if (filters.LastName != null)
            {
                users = users.Where(t => t.LastName.ToLower().Contains(filters.LastName.ToLower()));
            }
            if (filters.Email != null)
            {
                users = users.Where(t => t.Email.ToLower().Contains(filters.Email.ToLower()));
            }

            var pagedPosts = PagedList<object>.Create(users, filters.PageNumber, filters.PageSize);
            if (pagedPosts.Any())
            {
                return new ResponseData()
                {
                    Messages = new Message[] { new() { Type = "Information", Description = "Registros de usuarios recuperados correctamente" } },
                    Pagination = pagedPosts,
                    StatusCode = HttpStatusCode.OK
                };
            }
            else
            {
                return new ResponseData()
                {
                    Messages = new Message[] { new() { Type = "Warning", Description = "No fue posible recuperar la cantidad de registros" } },
                    Pagination = pagedPosts,
                    StatusCode = HttpStatusCode.NotFound
                };
            }
        }

        public async Task<User> GetUserAsync(int id)
        {
            var user = await _unitOfWork.UserRepository.GetById(id);
            if (user == null)
                throw new Exception($"No se encontró el usuario con Id {id}");

            return user;
        }

        /// <summary>
        /// Obtiene todos los usuarios utilizando Dapper.
        /// </summary>
        /// <returns>Objeto ResponseData con lista de usuarios o mensaje de error.</returns>
        public async Task<ResponseData> GetAllUsersDapperAsync(UserQueryFilter filters)
        {
            var users = await _unitOfWork.UserRepository.GetAllUsersDapperAsync();

            if (filters.FirstName != null)
            {
                users = users.Where(t => t.FirstName.ToLower().Contains(filters.FirstName.ToLower()));
            }
            if (filters.LastName != null)
            {
                users = users.Where(t => t.LastName.ToLower().Contains(filters.LastName.ToLower()));
            }
            if (filters.Email != null)
            {
                users = users.Where(t => t.Email.ToLower().Contains(filters.Email.ToLower()));
            }

            var pagedPosts = PagedList<object>.Create(users, filters.PageNumber, filters.PageSize);
            if (pagedPosts.Any())
            {
                return new ResponseData()
                {
                    Messages = new Message[] { new() { Type = "Information", Description = "Registros de usuarios recuperados correctamente" } },
                    Pagination = pagedPosts,
                    StatusCode = HttpStatusCode.OK
                };
            }
            else
            {
                return new ResponseData()
                {
                    Messages = new Message[] { new() { Type = "Warning", Description = "No fue posible recuperar la cantidad de registros" } },
                    Pagination = pagedPosts,
                    StatusCode = HttpStatusCode.NotFound
                };
            }
        }

        /// <summary>
        /// Recupera un usuario específico desde la base de datos utilizando Dapper
        /// </summary>
        /// <param name="id">Identificador del usuario</param>
        /// <returns>Objeto ResponseData con la información del usuario</returns>
        public async Task<ResponseData> GetUserByIdDapperAsync(int id)
        {
            var user = await _unitOfWork.UserRepository.GetUserByIdDapperAsync(id);

            if (user == null)
            {
                return new ResponseData()
                {
                    Messages = new[] { new Message { Type = "Warning", Description = "Usuario no encontrado" } },
                    StatusCode = HttpStatusCode.NotFound
                };
            }

            return new ResponseData()
            {
                Data = user,
                Messages = new[] { new Message { Type = "Information", Description = "Usuario recuperado correctamente" } },
                StatusCode = HttpStatusCode.OK
            };
        }

        public async Task InsertUserAsync(User user)
        {
            // Validar que el email no exista
            var existing = (await _unitOfWork.UserRepository.Find(u => u.Email == user.Email)).FirstOrDefault();
            if (existing != null)
                throw new Exception("Ya existe un usuario con el mismo correo electrónico.");

            await _unitOfWork.UserRepository.Add(user);
        }

        public async Task UpdateUserAsync(User user)
        {
            var existingUser = await _unitOfWork.UserRepository.GetById(user.Id);
            if (existingUser == null)
                throw new Exception("El usuario no existe.");

            // Evitar duplicar email
            var duplicate = (await _unitOfWork.UserRepository.Find(u => u.Email == user.Email && u.Id != user.Id)).FirstOrDefault();
            if (duplicate != null)
                throw new Exception("Ya existe otro usuario con ese correo electrónico.");

            await _unitOfWork.UserRepository.Update(user);
        }

        public async Task DeleteUserAsync(int id)
        {
            await _unitOfWork.UserRepository.Delete(id);
        }
    }
}
