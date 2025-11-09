using System.Net;
using TaskManager.Core.CustomEntities;
using TaskManager.Core.Entities;
using TaskManager.Core.Exceptions;
using TaskManager.Core.Interfaces;
using TaskManager.Core.QueryFilters;

namespace TaskManager.Core.Services
{
    /// <summary>
    /// Servicio encargado de la lógica de negocio para la entidad <see cref="TaskEntity"/>.
    /// </summary>
    /// <remarks>
    /// Contiene la lógica principal relacionada con la gestión de tareas,
    /// incluyendo la obtención, creación, actualización y eliminación de registros,
    /// así como la validación de datos y aplicación de filtros.
    /// </remarks>
    public class TaskEntityService : ITaskEntityService
    {
        /// <summary>
        /// Unidad de trabajo utilizada para acceder a los repositorios de datos.
        /// </summary>
        public readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Inicializa una nueva instancia del servicio <see cref="TaskEntityService"/>.
        /// </summary>
        /// <param name="unitOfWork">Unidad de trabajo que gestiona las operaciones de persistencia.</param>
        public TaskEntityService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Recupera todas las tareas aplicando filtros opcionales y paginación.
        /// </summary>
        /// <remarks>
        /// Este método obtiene la lista de tareas desde el repositorio,
        /// y aplica filtros por proyecto, estado, fecha de vencimiento, título o descripción.
        /// Retorna los resultados paginados dentro de un objeto <see cref="ResponseData"/>.
        /// </remarks>
        /// <param name="filters">Filtros para limitar los resultados, incluyendo paginación, texto y campos específicos.</param>
        /// <returns>
        /// Retorna una colección paginada de tareas dentro de un objeto <see cref="ResponseData"/>.
        /// </returns>
        /// <response code="200">Registros de tareas recuperados correctamente.</response>
        /// <response code="404">No se encontraron tareas con los filtros aplicados.</response>
        public async Task<ResponseData> GetAllTasksAsync(TaskEntityQueryFilter filters)
        {
            var tasks = await _unitOfWork.TaskEntityRepository.GetAll();

            if (filters.ProjectId != null)
                tasks = tasks.Where(t => t.ProjectId == filters.ProjectId);

            if (filters.StatusId != null)
                tasks = tasks.Where(t => t.StatusId == filters.StatusId);

            if (filters.DueDate != null) // https://localhost:7078/api/taskentity/dto/mapper?DueDate=2025-11-15
            {
                tasks = tasks.Where(x => x.DueDate?.ToShortDateString() ==
                filters.DueDate?.ToShortDateString());
            }

            if (filters.Title != null)
                tasks = tasks.Where(t => t.Title.ToLower().Contains(filters.Title.ToLower()));

            if (filters.Description != null)
                tasks = tasks.Where(t => t.Description.ToLower().Contains(filters.Description.ToLower()));

            if (filters.Texto != null)
            {
                tasks = tasks.Where(t =>
                (t.Title.ToLower().Contains(filters.Texto.ToLower())) ||
                (t.Description.ToLower().Contains(filters.Texto.ToLower()))
                );
            }

            var pagedTasks = PagedList<object>.Create(tasks, filters.PageNumber, filters.PageSize);

            if (pagedTasks.Any())
            {
                return new ResponseData()
                {
                    Messages = new Message[] { new() { Type = "Information", Description = "Registros de tareas recuperados correctamente" } },
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
        /// Recupera todas las tareas utilizando Dapper con filtros opcionales y paginación.
        /// </summary>
        /// <remarks>
        /// Este método usa Dapper para ejecutar consultas más eficientes sobre la base de datos
        /// y filtra los resultados según los parámetros especificados en <paramref name="filters"/>.
        /// </remarks>
        /// <param name="filters">Filtros de búsqueda, paginación o coincidencia de texto.</param>
        /// <returns>Objeto <see cref="ResponseData"/> con la lista paginada de tareas.</returns>
        /// <response code="200">Registros recuperados correctamente utilizando Dapper.</response>
        /// <response code="404">No se encontraron registros que coincidan con los filtros.</response>
        public async Task<ResponseData> GetAllTasksDapperAsync(TaskEntityQueryFilter filters)
        {
            var tasks = await _unitOfWork.TaskEntityRepository.GetAllTasksDapperAsync();

            if (filters.ProjectId != null)
                tasks = tasks.Where(t => t.ProjectId == filters.ProjectId);

            if (filters.StatusId != null)
                tasks = tasks.Where(t => t.StatusId == filters.StatusId);

            if (filters.DueDate != null)
            {
                tasks = tasks.Where(x => x.DueDate?.ToShortDateString() ==
                filters.DueDate?.ToShortDateString());
            }

            if (filters.Title != null)
                tasks = tasks.Where(t => t.Title.ToLower().Contains(filters.Title.ToLower()));

            if (filters.Description != null)
                tasks = tasks.Where(t => t.Description.ToLower().Contains(filters.Description.ToLower()));

            if (filters.Texto != null)
            {
                tasks = tasks.Where(t =>
                (t.Title.ToLower().Contains(filters.Texto.ToLower())) ||
                (t.Description.ToLower().Contains(filters.Texto.ToLower()))
                );
            }

            var pagedTasks = PagedList<object>.Create(tasks, filters.PageNumber, filters.PageSize);

            if (pagedTasks.Any())
            {
                return new ResponseData()
                {
                    Messages = new Message[] { new() { Type = "Information", Description = "Registros de tareas recuperados correctamente utilizando Dapper" } },
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
        /// Recupera una tarea específica por su identificador utilizando Dapper.
        /// </summary>
        /// <remarks>
        /// Este método obtiene los detalles completos de una tarea, incluyendo su estado y proyecto asociado,
        /// sin aplicar paginación al tratarse de un solo registro.
        /// </remarks>
        /// <param name="taskId">Identificador único de la tarea.</param>
        /// <returns>
        /// Un objeto <see cref="ResponseData"/> con la información de la tarea o un mensaje de advertencia si no se encuentra.
        /// </returns>
        /// <response code="200">Tarea recuperada correctamente.</response>
        /// <response code="404">No se encontró la tarea solicitada.</response>
        public async Task<ResponseData> GetTaskByIdDapperAsync(int taskId)
        {
            var taskDetail = await _unitOfWork.TaskEntityRepository.GetTaskByIdDapperAsync(taskId);

            if (taskDetail != null)
            {
                return new ResponseData()
                {
                    Data = taskDetail,
                    Messages = new Message[]
                    {
                        new() { Type = "Information", Description = "Tarea recuperada correctamente." }
                    },
                    StatusCode = HttpStatusCode.OK
                };
            }

            return new ResponseData()
            {
                Messages = new Message[]
                {
                    new() { Type = "Warning", Description = "No se encontró la tarea solicitada." }
                },
                StatusCode = HttpStatusCode.NotFound
            };
        }

        /// <summary>
        /// Recupera una tarea existente por su Id validando su existencia en la base de datos.
        /// </summary>
        /// <param name="id">Identificador único de la tarea.</param>
        /// <returns>Instancia de <see cref="TaskEntity"/> encontrada.</returns>
        /// <exception cref="BussinesException">Se lanza si la tarea no existe.</exception>
        public async Task<TaskEntity> GetTaskAsync(int id)
        {
            var task = await _unitOfWork.TaskEntityRepository.GetById(id);
            if (task == null)
            {
                throw new BussinesException("La tarea especificada no existe.");
            }

            var project = await _unitOfWork.ProjectRepository.GetById(task.ProjectId);
            var status = await _unitOfWork.StatusRepository.GetById(task.StatusId);

            return task;
        }

        /// <summary>
        /// Obtiene la cantidad de tareas agrupadas por estado en un proyecto específico.
        /// </summary>
        /// <param name="projectId">Identificador del proyecto.</param>
        /// <param name="pageNumber">Número de página para la paginación.</param>
        /// <param name="pageSize">Tamaño de página.</param>
        /// <returns>Objeto <see cref="ResponseData"/> con los resultados paginados.</returns>
        /// <response code="200">Registros obtenidos correctamente.</response>
        /// <response code="404">No se encontraron resultados.</response>
        public async Task<ResponseData> GetTaskCountByStatusAsync(int projectId, int pageNumber, int pageSize)
        {
            var result = await _unitOfWork.TaskEntityRepository.GetTaskCountByStatusAsync(projectId);
            var pagedResult = PagedList<object>.Create(result, pageNumber, pageSize);

            if (pagedResult.Any())
            {
                return new ResponseData()
                {
                    Messages = new Message[] { new() { Type = "Information", Description = "Registros de tareas recuperados correctamente utilizando Dapper" } },
                    Pagination = pagedResult,
                    StatusCode = HttpStatusCode.OK
                };
            }
            else
            {
                return new ResponseData()
                {
                    Messages = new Message[] { new() { Type = "Warning", Description = "No fue posible recuperar la cantidad de registros" } },
                    Pagination = pagedResult,
                    StatusCode = HttpStatusCode.NotFound
                };
            }
        }

        /// <summary>
        /// Inserta una nueva tarea en la base de datos luego de validar su integridad y contenido.
        /// </summary>
        /// <param name="task">Entidad <see cref="TaskEntity"/> a registrar.</param>
        /// <exception cref="BussinesException">
        /// Se lanza si el proyecto no existe, el estado no es válido o el texto contiene palabras prohibidas.
        /// </exception>
        public async Task InsertTaskAsync(TaskEntity task)
        {
            var project = await _unitOfWork.ProjectRepository.GetById(task.ProjectId);
            if (project == null)
            {
                throw new BussinesException("El proyecto asignado a la tarea no existe.");
            }

            if (task.StatusId == 0)
            {
                task.StatusId = 1;
            }

            var status = await _unitOfWork.StatusRepository.GetById(task.StatusId);
            if (status == null)
            {
                throw new BussinesException($"El estado con Id {task.StatusId} no existe.");
            }

            if (ContainsForbiddenWord(task.Title) || ContainsForbiddenWord(task.Description))
            {
                throw new BussinesException("El contenido de la tarea contiene palabras no permitidas.");
            }

            await _unitOfWork.TaskEntityRepository.Add(task);
            await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Actualiza una tarea existente validando su existencia, proyecto, estado y contenido.
        /// </summary>
        /// <param name="task">Entidad <see cref="TaskEntity"/> con los datos actualizados.</param>
        /// <exception cref="BussinesException">Se lanza si la tarea no existe o contiene datos inválidos.</exception>
        public async Task UpdateTaskAsync(TaskEntity task)
        {
            var existingTask = await _unitOfWork.TaskEntityRepository.GetById(task.Id);
            if (existingTask == null)
            {
                throw new BussinesException("No se puede actualizar una tarea inexistente.");
            }

            var project = await _unitOfWork.ProjectRepository.GetById(task.ProjectId);
            if (project == null)
            {
                throw new BussinesException("El proyecto asignado a la tarea no existe.");
            }

            if (task.StatusId == 0)
            {
                task.StatusId = 1;
            }

            var status = await _unitOfWork.StatusRepository.GetById(task.StatusId);
            if (status == null)
            {
                throw new BussinesException($"El estado con Id {task.StatusId} no existe.");
            }

            if (ContainsForbiddenWord(task.Title) || ContainsForbiddenWord(task.Description))
            {
                throw new BussinesException("El contenido de la tarea contiene palabras no permitidas.");
            }

            await _unitOfWork.TaskEntityRepository.Update(task);
            await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Elimina una tarea existente de la base de datos validando su existencia previa.
        /// </summary>
        /// <param name="id">Identificador único de la tarea.</param>
        /// <exception cref="BussinesException">Se lanza si la tarea no existe.</exception>
        public async Task DeleteTaskAsync(int id)
        {
            var task = await _unitOfWork.TaskEntityRepository.GetById(id);
            if (task == null)
            {
                throw new BussinesException("La tarea no existe.");
            }

            await _unitOfWork.TaskEntityRepository.Delete(id);
            await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Lista de palabras no permitidas en los campos de texto de las tareas.
        /// </summary>
        private readonly string[] ForbiddenWords =
        {
            "violencia",
            "odio",
            "groseria",
            "discriminacion"
        };

        /// <summary>
        /// Determina si el texto proporcionado contiene alguna palabra prohibida.
        /// </summary>
        /// <param name="text">Texto a evaluar.</param>
        /// <returns><c>true</c> si contiene palabras prohibidas; en caso contrario, <c>false</c>.</returns>
        private bool ContainsForbiddenWord(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;

            foreach (var word in ForbiddenWords)
            {
                if (text.Contains(word, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }
    }
}

