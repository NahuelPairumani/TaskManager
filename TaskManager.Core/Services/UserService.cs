using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace TaskManager.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _unitOfWork.UserRepository.GetAll();
        }

        public async Task<User> GetUserAsync(int id)
        {
            var user = await _unitOfWork.UserRepository.GetById(id);
            if (user == null)
                throw new Exception($"No se encontró el usuario con Id {id}");

            return user;
        }

        public async Task InsertUserAsync(User user)
        {
            // Validar que el email no exista
            var existing = (await _unitOfWork.UserRepository.Find(u => u.Email == user.Email)).FirstOrDefault();
            if (existing != null)
                throw new Exception("Ya existe un usuario con el mismo correo electrónico.");

            user.PasswordHash = HashPassword(user.PasswordHash);
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

            // Si se envía una nueva contraseña, se actualiza el hash
            if (!string.IsNullOrWhiteSpace(user.PasswordHash))
                user.PasswordHash = HashPassword(user.PasswordHash);
            else
                user.PasswordHash = existingUser.PasswordHash;

            await _unitOfWork.UserRepository.Update(user);
        }

        public async Task DeleteUserAsync(User user)
        {
            await _unitOfWork.UserRepository.Delete(user);
        }

        private string HashPassword(string password) // Simple hash con SHA256, para no exponer contraseñas en texto plano
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }
}
