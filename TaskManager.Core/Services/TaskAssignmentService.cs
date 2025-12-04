using System.Net;
using TaskManager.Core.CustomEntities;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;
using TaskManager.Core.QueryFilters;

namespace TaskManager.Core.Services
{
    public class TaskAssignmentService : ITaskAssignmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TaskAssignmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseData> GetAllAssignmentsAsync(TaskAssignmentQueryFilter filters)
        {
            var assignments = await _unitOfWork.TaskAssignmentRepository.GetAll();

            if (filters.TaskId != null)
                assignments = assignments.Where(t => t.TaskId == filters.TaskId);

            if (filters.UserId != null)
                assignments = assignments.Where(t => t.UserId == filters.UserId);

            var pagedTasks = PagedList<object>.Create(assignments, filters.PageNumber, filters.PageSize);
            if (pagedTasks.Any())
            {
                return new ResponseData()
                {
                    Messages = new Message[] { new() { Type = "Information", Description = "Registros de asignaciones recuperados correctamente" } },
                    Pagination = pagedTasks,
                    StatusCode = HttpStatusCode.OK
                };
            }
            else
            {
                return new ResponseData()
                {
                    Messages = new Message[] { new() { Type = "Warning", Description = "No fue posible recuperar la cantidad de registros" } },
                    Pagination = pagedTasks,
                    StatusCode = HttpStatusCode.NotFound
                };
            }
        }

        public async Task<TaskAssignment> GetAssignmentAsync(int id)
        {
            var assignment = await _unitOfWork.TaskAssignmentRepository.GetById(id);
            if (assignment == null)
                throw new Exception("Asignación no encontrada");

            return assignment;
        }

        // Dapper *********************************************************

        /// <summary>
        /// Recupera todos los registros de TaskAssignment.
        /// </summary>
        public async Task<ResponseData> GetAllAssignmentsDapperAsync(TaskAssignmentQueryFilter filters)
        {
            var assignments = await _unitOfWork.TaskAssignmentRepository.GetAllAssignmentsDapperAsync();
        
            if (filters.TaskId != null)
                assignments = assignments.Where(t => t.TaskId == filters.TaskId);

            if (filters.UserId != null)
                assignments = assignments.Where(t => t.UserId == filters.UserId);

            var pagedTasks = PagedList<object>.Create(assignments, filters.PageNumber, filters.PageSize);

            if (pagedTasks.Any())
            {
                return new ResponseData()
                {
                    Messages = new Message[] { new() { Type = "Information", Description = "Registros de asignaciones recuperados correctamente" } },
                    Pagination = pagedTasks,
                    StatusCode = HttpStatusCode.OK
                };
            }
            else
            {
                return new ResponseData()
                {
                    Messages = new Message[] { new() { Type = "Warning", Description = "No fue posible recuperar la cantidad de registros" } },
                    Pagination = pagedTasks,
                    StatusCode = HttpStatusCode.NotFound
                };
            }
        }

        /// <summary>
        /// Recupera un registro de TaskAssignment por Id.
        /// </summary>
        public async Task<ResponseData> GetByIdDapperAsync(int id)
        {
            var taskAssignment = await _unitOfWork.TaskAssignmentRepository.GetByIdDapperAsync(id);

            if (taskAssignment != null)
            {
                return new ResponseData
                {
                    Data = taskAssignment,
                    Messages = new[] { new Message { Type = "Information", Description = $"Registro con Id={id} recuperado correctamente." } },
                    StatusCode = HttpStatusCode.OK
                };
            }

            return new ResponseData
            {
                Messages = new[] { new Message { Type = "Warning", Description = $"No se encontró el registro con Id={id}." } },
                StatusCode = HttpStatusCode.NotFound
            };
        }

        public async Task AssignTaskToUserAsync(TaskAssignment assignment)
        {
            var task = await _unitOfWork.TaskEntityRepository.GetById(assignment.TaskId);
            if (task == null)
                throw new Exception("La tarea no existe");

            var user = await _unitOfWork.UserRepository.GetById(assignment.UserId);
            if (user == null)
                throw new Exception("El usuario no existe");

            await _unitOfWork.TaskAssignmentRepository.Add(assignment);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RemoveAssignmentAsync(int id)
        {
            var assignment = await _unitOfWork.TaskAssignmentRepository.GetById(id);
            if (assignment == null)
                throw new Exception("Asignación no encontrada");

            await _unitOfWork.TaskAssignmentRepository.Delete(id);
        }

        /*
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
        */
    }
}
