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
    /// Controlador de versión 1 para la gestión de tareas, que utiliza Entity Framework core para gets.
    /// </summary>
    /// <remarks>
    /// Este controlador contiene los endpoints relacionados con la entidad <see cref="TaskEntity"/>, 
    /// implementados utilizando servicios, validaciones y mapeadores.
    /// </remarks>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TaskEntityController : ControllerBase
    {
        private readonly ITaskEntityService _taskService;
        private readonly IMapper _mapper;
        private readonly IValidationService _validationService;

        public TaskEntityController(ITaskEntityService taskService, IMapper mapper, IValidationService validationService)
        {
            _taskService = taskService;
            _mapper = mapper;
            _validationService = validationService;
        }

        /// <summary>
        /// Recupera todas las tareas con DTOs mapeados y paginación.
        /// </summary>
        /// <remarks>
        /// Este método obtiene una lista de tareas filtradas según los parámetros enviados en <see cref="TaskEntityQueryFilter"/>.
        /// Convierte los resultados a DTOs y devuelve la información junto con la paginación.
        /// </remarks>
        /// <param name="taskEntityFilter">Filtros de búsqueda y paginación para las tareas.</param>
        /// <returns>Colección de tareas en formato <see cref="TaskEntityDto"/> con paginación.</returns>
        /// <response code="200">Tareas recuperadas correctamente.</response>
        /// <response code="400">Solicitud incorrecta o datos de filtro inválidos.</response>
        /// <response code="500">Error interno del servidor.</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<TaskEntityDto>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [HttpGet("dto/mapper")]
        public async Task<IActionResult> GetTasksDtoMapper([FromQuery] TaskEntityQueryFilter taskEntityFilter)
        {
            try
            {
                var tasks = await _taskService.GetAllTasksAsync(taskEntityFilter);
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
                var responsePost = new ResponseData()
                {
                    Messages = new Message[] { new() { Type = "Error", Description = err.Message } },
                };
                return StatusCode(500, responsePost);
            }
        }

        /// <summary>
        /// Recupera una tarea por su Id usando DTO mapeado.
        /// </summary>
        /// <remarks>
        /// Este método obtiene la información completa de la tarea según el Id proporcionado.
        /// Devuelve la información en formato DTO, incluyendo referencias a proyecto y estado.
        /// </remarks>
        /// <param name="id">Identificador de la tarea.</param>
        /// <returns>Objeto <see cref="TaskEntityDto"/> con los datos de la tarea.</returns>
        /// <response code="200">Tarea recuperada correctamente.</response>
        /// <response code="404">Tarea no encontrada.</response>
        /// <response code="500">Error interno del servidor.</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<TaskEntityDto>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [HttpGet("dto/mapper/{id}")]
        public async Task<IActionResult> GetTaskEntityDtoMapperById(int id)
        {
            try
            {
                var task = await _taskService.GetTaskAsync(id);
                var TaskEntityDto = _mapper.Map<TaskEntityDto>(task);

                var response = new ApiResponse<TaskEntityDto>(TaskEntityDto);
                return Ok(response); // para ver la informacion completa con proyecto y status de actividad
            }
            catch (Exception err)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Inserta una nueva tarea en la base de datos.
        /// </summary>
        /// <remarks>
        /// Valida la información de la tarea antes de insertar y retorna el registro insertado en formato estandarizado.
        /// </remarks>
        /// <param name="TaskEntityDto">Objeto DTO con los datos de la tarea a insertar.</param>
        /// <returns>Objeto <see cref="ApiResponse{TaskEntity}"/> con la tarea creada.</returns>
        /// <response code="200">Tarea insertada correctamente.</response>
        /// <response code="400">Datos inválidos o falló la validación.</response>
        /// <response code="500">Error interno del servidor.</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<TaskEntity>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [HttpPost("dto/mapper")]
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
        /// Actualiza una tarea existente.
        /// </summary>
        /// <remarks>
        /// Valida los datos enviados y actualiza el registro correspondiente.
        /// No permite cambiar la tarea de proyecto.
        /// </remarks>
        /// <param name="id">Identificador de la tarea a actualizar.</param>
        /// <param name="TaskEntityDto">Objeto DTO con los datos actualizados de la tarea.</param>
        /// <returns>Objeto <see cref="ApiResponse{TaskEntity}"/> con la información actualizada.</returns>
        /// <response code="200">Tarea actualizada correctamente.</response>
        /// <response code="400">Id no coincide, datos inválidos o se intenta cambiar de proyecto.</response>
        /// <response code="404">Tarea no encontrada.</response>
        /// <response code="500">Error interno del servidor.</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<TaskEntity>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [HttpPut("dto/mapper/{id}")]
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
        /// Elimina una tarea existente por Id.
        /// </summary>
        /// <remarks>
        /// Este método elimina el registro de la tarea en la base de datos.
        /// </remarks>
        /// <param name="id">Identificador de la tarea a eliminar.</param>
        /// <response code="204">Tarea eliminada correctamente.</response>
        /// <response code="500">Error interno del servidor.</response>
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [HttpDelete("dto/mapper/{id}")]
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

        
        [HttpGet("get/usersByProject/{idProject}")]
        public async Task<IActionResult> GetUsersByProject([FromRoute] int idProject, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _taskService.GetUsersAssignedToProjectAsync(idProject, pageNumber, pageSize);

                var usersWithCount = result.Pagination;
                var pagination = new Pagination
                {
                    TotalCount = result.Pagination.TotalCount,
                    PageSize = result.Pagination.PageSize,
                    CurrentPage = result.Pagination.CurrentPage,
                    TotalPages = result.Pagination.TotalPages,
                    HasNextPage = result.Pagination.HasNextPage,
                    HasPreviousPage = result.Pagination.HasPreviousPage
                };

                var response = new ApiResponse<IEnumerable<object>>(usersWithCount)
                {
                    Pagination = pagination,
                    Messages = result.Messages
                };

                return StatusCode((int)result.StatusCode, response);
            }
            catch (Exception err)
            {
                var responsePost = new ResponseData()
                {
                    Messages = new Message[] { new() { Type = "Error", Description = err.Message } },
                };
                return StatusCode(500, responsePost);
            }
        }
    }
}
