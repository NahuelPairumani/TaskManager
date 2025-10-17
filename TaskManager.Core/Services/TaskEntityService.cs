using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;

namespace TaskManager.Core.Services
{
    public class TaskEntityService : ITaskEntityService
    {
        public readonly IUnitOfWork _unitOfWork;

        public TaskEntityService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<TaskEntity>> GetAllTasksAsync()
        {
            return await _unitOfWork.TaskEntityRepository.GetAll();
        }

        public async Task<TaskEntity> GetTaskAsync(int id)
        {
            // validar que la tarea exista
            var task = await _unitOfWork.TaskEntityRepository.GetById(id);

            if (task == null)
            {
                throw new Exception("Tarea no encontrada");
            }
            var project = await _unitOfWork.ProjectRepository.GetById(task.ProjectId);
            var status = await _unitOfWork.StatusRepository.GetById(task.StatusId);

            return task;
        }

        public async Task InsertTaskAsync(TaskEntity task)
        {
            var project = await _unitOfWork.ProjectRepository.GetById(task.ProjectId);
            if (project == null)
            {
                throw new Exception("El proyecto asignado a la tarea no existe.");
            }

            if (task.StatusId == 0)
            {
                task.StatusId = 1;
            }

            var status = await _unitOfWork.StatusRepository.GetById(task.StatusId);
            if (status == null)
            {
                throw new Exception($"El estado con Id {task.StatusId} no existe en la base de datos.");
            }

            await _unitOfWork.TaskEntityRepository.Add(task);
        }

        public async Task UpdateTaskAsync(TaskEntity task)
        {
            var project = await _unitOfWork.ProjectRepository.GetById(task.ProjectId);
            if (project == null)
            {
                throw new Exception("El proyecto asignado a la tarea no existe.");
            }

            if (task.StatusId == 0)
            {
                task.StatusId = 1;
            }

            var status = await _unitOfWork.StatusRepository.GetById(task.StatusId);
            if (status == null)
            {
                throw new Exception($"El estado con Id {task.StatusId} no existe en la base de datos.");
            }
            await _unitOfWork.TaskEntityRepository.Update(task);
        }

        public async Task DeleteTaskAsync(TaskEntity task)
        {
            await _unitOfWork.TaskEntityRepository.Delete(task);
        }

        public async Task<IEnumerable<TaskEntity>> SearchTasksAsync(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                throw new Exception("Debe especificar un texto para la búsqueda.");
            }

            // Normalizamos texto (minúsculas)
            searchText = searchText.Trim().ToLower();

            // Buscamos en título o descripción
            var tasks = await _unitOfWork.TaskEntityRepository.Find(t =>
                t.Title.ToLower().Contains(searchText) ||
                t.Description.ToLower().Contains(searchText)
            );

            return tasks;
        }

        public async Task<IEnumerable<TaskEntity>> GetAllProjectTasksAsync(int projectId, int? statusId = null)
        {
            // Validar existencia del proyecto
            var project = await _unitOfWork.ProjectRepository.GetById(projectId);
            if (project == null)
            {
                throw new Exception($"El proyecto con ID {projectId} no existe.");
            }

            // Si se especifica un estado, validar que exista
            if (statusId.HasValue)
            {
                var status = await _unitOfWork.StatusRepository.GetById(statusId.Value);
                if (status == null)
                {
                    throw new Exception($"El estado con ID {statusId.Value} no existe.");
                }
            }

            // Filtrar tareas según los parámetros
            IEnumerable<TaskEntity> tasks;

            if (statusId.HasValue)
            {
                tasks = await _unitOfWork.TaskEntityRepository.Find(t => t.ProjectId == projectId && t.StatusId == statusId.Value);
            }
            else
            {
                tasks = await _unitOfWork.TaskEntityRepository.Find(t => t.ProjectId == projectId);
            }
            return tasks;
        }
    }
}
