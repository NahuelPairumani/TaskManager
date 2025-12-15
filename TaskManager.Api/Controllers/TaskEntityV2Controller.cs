using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TaskManager.Api.Responses;
using TaskManager.Core.CustomEntities;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;
using TaskManager.Core.QueryFilters;
using TaskManager.Infrastructure.DTOs;
using TaskManager.Infrastructure.Validators;

using Microsoft.AspNetCore.Authorization;

namespace TaskManager.Api.Controllers
{
    /// <summary>
    /// Controlador de versión 2.0 para la gestión de tareas, que utiliza DAPPER para gets.
    /// </summary>
    /// <remarks>
    /// Este controlador contiene los endpoints relacionados con la entidad <see cref="TaskEntity"/>, 
    /// implementados utilizando servicios, validaciones y mapeadores automáticos.
    /// </remarks>
    [Authorize]
    [Route("api/v{version:ApiVersion}/taskentity")]
    [ApiVersion("2.0")]
    [ApiController]
    public class TaskEntityV2Controller : ControllerBase
    {
        private readonly ITaskEntityService _taskService;
        private readonly IMapper _mapper;
        private readonly IValidationService _validationService;

        public TaskEntityV2Controller(ITaskEntityService taskService, IMapper mapper, IValidationService validationService)
        {
            _taskService = taskService;
            _mapper = mapper;
            _validationService = validationService;
        }

        /// <summary>
        /// Recupera una lista paginada de tareas utilizando Dapper y mapeo a DTO.
        /// </summary>
        /// <remarks>
        /// Este método obtiene los registros de tareas según los filtros enviados como parámetros de consulta
        /// y los transforma en objetos de transferencia de datos (<see cref="TaskEntityDto"/>).
        /// </remarks>
        /// <param name="taskEntityFilter">
        /// Filtros aplicables para la búsqueda de tareas, como ID de proyecto, estado, fecha o texto.
        /// </param>
        /// <returns>
        /// Devuelve un objeto <see cref="ApiResponse{T}"/> con la lista paginada de tareas DTO y metadatos de paginación.
        /// </returns>
        /// <response code="200">Retorna la lista de tareas paginadas.</response>
        /// <response code="400">Solicitud inválida por parámetros erróneos.</response>
        /// <response code="404">No se encontraron tareas con los filtros indicados.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet("dapper")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<TaskEntityDto>>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetTasksDtoDapper([FromQuery] TaskEntityQueryFilter taskEntityFilter)
        {
            try
            {
                var tasks = await _taskService.GetAllTasksDapperAsync(taskEntityFilter);
                var tasksDto = _mapper.Map<IEnumerable<TaskEntityDto>>(tasks.Pagination);

                var pagination = new Pagination
                {
                    TotalCount = tasks.Pagination.TotalCount,
                    PageSize = tasks.Pagination.PageSize,
                    CurrentPage = tasks.Pagination.CurrentPage,
                    TotalPages = tasks.Pagination.TotalPages,
                    HasNextPage = tasks.Pagination.HasNextPage,
                    HasPreviousPage = tasks.Pagination.HasPreviousPage
                };
                var response = new ApiResponse<IEnumerable<TaskEntityDto>>(tasksDto)
                {
                    Pagination = pagination,
                    Messages = tasks.Messages
                };

                return StatusCode((int)tasks.StatusCode, response);
            }
            catch (Exception err)
            {
                var response = new ResponseData()
                {
                    Messages = new Message[] { new() { Type = "Error", Description = err.Message } },
                };
                return StatusCode(500, response);
            }
        }

        /// <summary>
        /// Recupera una tarea específica con su estado y proyecto asociados.
        /// </summary>
        /// <remarks>
        /// Devuelve los datos detallados de una tarea identificada por su Id, incluyendo información adicional 
        /// sobre el proyecto y estado al que pertenece.
        /// </remarks>
        /// <param name="taskId">Identificador único de la tarea.</param>
        /// <returns>Objeto <see cref="TaskDetailResponse"/> con los datos completos de la tarea.</returns>
        /// <response code="200">Retorna la información detallada de la tarea.</response>
        /// <response code="404">No se encontró la tarea especificada.</response>
        /// <response code="500">Error interno del servidor.</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<TaskDetailResponse>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [HttpGet("dapper/{taskId}")]
        public async Task<IActionResult> GetTaskByIdDapperAsync(int taskId)
        {
            try
            {
                var task = await _taskService.GetTaskByIdDapperAsync(taskId);

                var response = new ApiResponse<TaskDetailResponse>((TaskDetailResponse)task.Data)
                {
                    Messages = task.Messages
                };

                return StatusCode((int)task.StatusCode, response);
            }
            catch (Exception err)
            {
                var responsePost = new ResponseData()
                {
                    Messages = new Message[]
                    {
                        new() { Type = "Error", Description = err.Message }
                    }
                };

                return StatusCode((int)HttpStatusCode.InternalServerError, responsePost);
            }
        }

        /// <summary>
        /// Devuelve el conteo de tareas agrupadas por estado dentro de un proyecto, con soporte de paginación.
        /// </summary>
        /// <remarks>
        /// Este método agrupa las tareas de un proyecto según su estado y devuelve los totales de cada grupo.
        /// </remarks>
        /// <param name="projectId">Identificador del proyecto.</param>
        /// <param name="pageNumber">Número de página para la paginación (por defecto = 1).</param>
        /// <param name="pageSize">Cantidad de elementos por página (por defecto = 10).</param>
        /// <returns>
        /// Colección de <see cref="TaskStatusCountResponse"/> con el conteo de tareas por estado.
        /// </returns>
        /// <response code="200">Retorna los conteos agrupados correctamente.</response>
        /// <response code="404">No se encontraron tareas para el proyecto indicado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet("dapper/countStatuses/{projectId}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<TaskStatusCountResponse>>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetTaskCountByStatus(int projectId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _taskService.GetTaskCountByStatusAsync(projectId, pageNumber, pageSize);

                var pagination = new Pagination
                {
                    TotalCount = result.Pagination.TotalCount,
                    PageSize = result.Pagination.PageSize,
                    CurrentPage = result.Pagination.CurrentPage,
                    TotalPages = result.Pagination.TotalPages,
                    HasNextPage = result.Pagination.HasNextPage,
                    HasPreviousPage = result.Pagination.HasPreviousPage
                };

                var casting = result.Pagination.Cast<TaskStatusCountResponse>();
                var response = new ApiResponse<IEnumerable<TaskStatusCountResponse>>(casting)
                {
                    Pagination = pagination,
                    Messages = result.Messages
                };

                return StatusCode((int)result.StatusCode, response);
            }
            catch (Exception err)
            {
                var responseError = new ResponseData()
                {
                    Messages = new Message[]
                    {
                        new() { Type = "Error", Description = err.Message }
                    }
                };

                return StatusCode((int)HttpStatusCode.InternalServerError, responseError);
            }
        }

        /// <summary>
        /// Inserta una nueva tarea utilizando un DTO mapeado a entidad.
        /// </summary>
        /// <remarks>
        /// Este endpoint valida los datos recibidos en el <see cref="TaskEntityDto"/>, los mapea a una entidad 
        /// y los almacena en la base de datos si son válidos.
        /// </remarks>
        /// <param name="TaskEntityDto">Objeto DTO con la información de la tarea a insertar.</param>
        /// <returns>Devuelve la tarea creada dentro de un objeto <see cref="ApiResponse{T}"/>.</returns>
        /// <response code="200">Tarea insertada correctamente.</response>
        /// <response code="400">Error de validación en los datos enviados.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpPost("dto/mapper")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<TaskEntity>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> InsertTaskEntityDtoMapper(TaskEntityDto TaskEntityDto)
        {
            try
            {
                var validationResult = await _validationService.ValidateAsync(TaskEntityDto);
                if (!validationResult.IsValid)
                    return BadRequest(new { Errors = validationResult.Errors });

                var task = _mapper.Map<TaskEntity>(TaskEntityDto);
                await _taskService.InsertTaskAsync(task);

                var response = new ApiResponse<TaskEntity>(task);
                return Ok(response);
            }
            catch (Exception err)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Actualiza una tarea existente a partir de un DTO mapeado.
        /// </summary>
        /// <remarks>
        /// Valida que el identificador coincida con el del DTO y que la tarea pertenezca al mismo proyecto 
        /// antes de actualizar los datos.
        /// </remarks>
        /// <param name="id">Identificador de la tarea a actualizar.</param>
        /// <param name="TaskEntityDto">Objeto DTO con los nuevos valores de la tarea.</param>
        /// <returns>Devuelve la tarea actualizada dentro de una respuesta <see cref="ApiResponse{T}"/>.</returns>
        /// <response code="200">Tarea actualizada exitosamente.</response>
        /// <response code="400">Datos inconsistentes o errores de validación.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpPut("dto/mapper/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<TaskEntity>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdateTaskEntityDtoMapper(int id, [FromBody] TaskEntityDto TaskEntityDto)
        {
            try
            {
                if (id != TaskEntityDto.Id)
                    return BadRequest("El Id de la tarea no coincide");

                var validationResult = await _validationService.ValidateAsync(TaskEntityDto);
                if (!validationResult.IsValid)
                    return BadRequest(new { Errors = validationResult.Errors });

                var task = await _taskService.GetTaskAsync(id);

                if (TaskEntityDto.ProjectId != task.ProjectId)
                    return BadRequest("La tarea no puede cambiar de proyecto");

                _mapper.Map(TaskEntityDto, task);
                await _taskService.UpdateTaskAsync(task);

                var response = new ApiResponse<TaskEntity>(task);
                return Ok(response);
            }
            catch (Exception err)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Elimina una tarea específica por su identificador.
        /// </summary>
        /// <remarks>
        /// Este endpoint elimina una tarea del sistema validando previamente su existencia.
        /// </remarks>
        /// <param name="id">Identificador único de la tarea a eliminar.</param>
        /// <returns>No devuelve contenido en caso de eliminación exitosa.</returns>
        /// <response code="204">Tarea eliminada correctamente.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpDelete("dto/mapper/{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteTaskEntityDtoMapper(int id)
        {
            try
            {
                await _taskService.DeleteTaskAsync(id);
                return NoContent();
            }
            catch (Exception err)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, err.Message);
            }
        }
    }
}
