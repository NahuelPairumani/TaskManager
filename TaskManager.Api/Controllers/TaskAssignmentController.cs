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
    /// Controlador para la gestión de asignaciones entre usuarios y tareas.
    /// </summary>
    /// <remarks>
    /// Este controlador contiene endpoints para operaciones CRUD sobre la entidad <see cref="TaskAssignment"/> 
    /// utilizando servicios, validaciones y mapeadores automáticos.
    /// </remarks>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TaskAssignmentController : ControllerBase
    {
        private readonly ITaskAssignmentService _service;
        private readonly IMapper _mapper;
        private readonly IValidationService _validationService;

        public TaskAssignmentController(
            ITaskAssignmentService service,
            IMapper mapper,
            IValidationService validationService)
        {
            _service = service;
            _mapper = mapper;
            _validationService = validationService;
        }

        /// <summary>
        /// Recupera todas las asignaciones de tareas utilizando Entity Framework Core.
        /// </summary>
        /// <remarks>
        /// Este método obtiene una lista paginada de asignaciones según los filtros proporcionados 
        /// en <see cref="TaskAssignmentQueryFilter"/> y las convierte a DTOs para la respuesta.
        /// </remarks>
        /// <param name="taskAssignmentFilter">Filtros para búsqueda y paginación de asignaciones.</param>
        /// <returns>Colección de <see cref="TaskAssignmentDto"/> con información de paginación.</returns>
        /// <response code="200">Asignaciones recuperadas correctamente.</response>
        /// <response code="500">Error interno del servidor.</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<TaskAssignmentDto>>))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [HttpGet("get/efc")]
        public async Task<IActionResult> GetAll([FromQuery] TaskAssignmentQueryFilter taskAssignmentFilter)
        {
            try
            {
                var assignments = await _service.GetAllAssignmentsAsync(taskAssignmentFilter);
                var dto = _mapper.Map<IEnumerable<TaskAssignmentDto>>(assignments.Pagination);
                var pagination = new Pagination
                {
                    TotalCount = assignments.Pagination.TotalCount,
                    PageSize = assignments.Pagination.PageSize,
                    CurrentPage = assignments.Pagination.CurrentPage,
                    TotalPages = assignments.Pagination.TotalPages,
                    HasNextPage = assignments.Pagination.HasNextPage,
                    HasPreviousPage = assignments.Pagination.HasPreviousPage
                };
                var response = new ApiResponse<IEnumerable<TaskAssignmentDto>>(dto)
                {
                    Pagination = pagination,
                    Messages = assignments.Messages
                };

                return StatusCode((int)assignments.StatusCode, response);
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
        /// Recupera todas las asignaciones de tareas utilizando Dapper.
        /// </summary>
        /// <remarks>
        /// Este método obtiene una lista paginada de asignaciones aplicando los filtros 
        /// proporcionados en <see cref="TaskAssignmentQueryFilter"/> y devuelve la respuesta con DTOs.
        /// </remarks>
        /// <param name="filters">Filtros de búsqueda y paginación para las asignaciones.</param>
        /// <returns>Colección de <see cref="TaskAssignmentDto"/> con información de paginación.</returns>
        /// <response code="200">Asignaciones recuperadas correctamente.</response>
        /// <response code="500">Error interno del servidor.</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<TaskAssignmentDto>>))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [HttpGet("get/dapper")]
        public async Task<IActionResult> GetAllDapper([FromQuery] TaskAssignmentQueryFilter filters)
        {
            try
            {
                var result = await _service.GetAllAssignmentsDapperAsync(filters);
                var assignmentDto = _mapper.Map<IEnumerable<TaskAssignmentDto>>(result.Pagination);

                var pagination = new Pagination
                {
                    TotalCount = result.Pagination.TotalCount,
                    PageSize = result.Pagination.PageSize,
                    CurrentPage = result.Pagination.CurrentPage,
                    TotalPages = result.Pagination.TotalPages,
                    HasNextPage = result.Pagination.HasNextPage,
                    HasPreviousPage = result.Pagination.HasPreviousPage
                };

                var response = new ApiResponse<IEnumerable<TaskAssignmentDto>>(assignmentDto)
                {
                    Pagination = pagination,
                    Messages = result.Messages
                };
                return StatusCode((int)result.StatusCode, response);
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
        /// Recupera una asignación de tarea por su Id usando Dapper.
        /// </summary>
        /// <param name="id">Identificador de la asignación.</param>
        /// <returns>Objeto <see cref="TaskAssignment"/> encontrado.</returns>
        /// <response code="200">Asignación encontrada correctamente.</response>
        /// <response code="404">Asignación no encontrada.</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<TaskAssignment>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpGet("get/dapper/{id}")]
        public async Task<IActionResult> GetByIdDapper(int id)
        {
            var result = await _service.GetByIdDapperAsync(id);
            var response = new ApiResponse<object>(result.Data)
            {
                Messages = result.Messages
            };
            return StatusCode((int)result.StatusCode, response);
        }

        /// <summary>
        /// Asigna una tarea a un usuario.
        /// </summary>
        /// <param name="dto">DTO con los datos de la asignación a crear.</param>
        /// <returns>DTO de la asignación creada.</returns>
        /// <response code="200">Asignación creada correctamente.</response>
        /// <response code="400">Datos inválidos o fallo en la validación.</response>
        /// <response code="500">Error interno del servidor.</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<TaskAssignmentDto>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [HttpPost("assign")]
        public async Task<IActionResult> Assign([FromBody] TaskAssignmentDto dto)
        {
            try
            {
                var validationResult = await _validationService.ValidateAsync(dto);
                if (!validationResult.IsValid)
                    return BadRequest(new { Errors = validationResult.Errors });

                var assignment = _mapper.Map<TaskAssignment>(dto);

                await _service.AssignTaskToUserAsync(assignment);

                var response = new ApiResponse<TaskAssignmentDto>(dto);
                return Ok(response);
            }
            catch (Exception err)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Elimina una asignación de tarea por su Id.
        /// </summary>
        /// <param name="id">Identificador de la asignación a eliminar.</param>
        /// <response code="204">Asignación eliminada correctamente.</response>
        /// <response code="500">Error interno del servidor.</response>
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.RemoveAssignmentAsync(id);
                return NoContent();
            }
            catch (Exception err)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, err.Message);
            }
        }
    }
}
