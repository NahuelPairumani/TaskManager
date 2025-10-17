using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;

namespace TaskManager.Core.Services
{
    public class TaskAssignmentService : ITaskAssignmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TaskAssignmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<TaskAssignment>> GetAllAssignmentsAsync()
        {
            return await _unitOfWork.TaskAssignmentRepository.GetAll();
        }

        public async Task<TaskAssignment> GetAssignmentAsync(int id)
        {
            var assignment = await _unitOfWork.TaskAssignmentRepository.GetById(id);
            if (assignment == null)
                throw new Exception("Asignación no encontrada");

            return assignment;
        }

        public async Task AssignTaskToUserAsync(TaskAssignment assignment)
        {
            var task = await _unitOfWork.TaskEntityRepository.GetById(assignment.TaskId);
            if (task == null)
                throw new Exception("La tarea no existe");

            var user = await _unitOfWork.UserRepository.GetById(assignment.UserId);
            if (user == null)
                throw new Exception("El usuario no existe");

            // Evitar asignaciones duplicadas
            var existing = await _unitOfWork.TaskAssignmentRepository.GetAssignmentAsync(assignment.TaskId, assignment.UserId);
            if (existing != null)
                throw new Exception("Esta tarea ya está asignada a este usuario");

            await _unitOfWork.TaskAssignmentRepository.Add(assignment);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RemoveAssignmentAsync(int id)
        {
            var assignment = await _unitOfWork.TaskAssignmentRepository.GetById(id);
            if (assignment == null)
                throw new Exception("Asignación no encontrada");

            await _unitOfWork.TaskAssignmentRepository.Delete(assignment);
        }

        public async Task<IEnumerable<TaskEntity>> GetTasksByUserAsync(int userId)
        {
            var assignments = await _unitOfWork.TaskAssignmentRepository.GetAssignmentsByUserIdAsync(userId);
            var tasks = new List<TaskEntity>();
            foreach (var a in assignments)
            {
                var task = await _unitOfWork.TaskEntityRepository.GetById(a.TaskId);
                if (task != null)
                    tasks.Add(task);
            }
            return tasks;
        }

        public async Task<IEnumerable<User>> GetUsersByTaskAsync(int taskId)
        {
            var assignments = await _unitOfWork.TaskAssignmentRepository.GetAssignmentsByTaskIdAsync(taskId);
            var users = new List<User>();
            foreach (var a in assignments)
            {
                var user = await _unitOfWork.UserRepository.GetById(a.UserId);
                if (user != null)
                    users.Add(user);
            }
            return users;
        }
    }
}
