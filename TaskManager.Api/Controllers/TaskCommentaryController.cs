using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TaskManager.Api.Responses;
using TaskManager.Core.CustomEntities;
using TaskManager.Core.Entities;
using TaskManager.Core.Exceptions;
using TaskManager.Core.Interfaces;
using TaskManager.Core.QueryFilters;
using TaskManager.Infrastructure.DTOs;
using TaskManager.Infrastructure.Validators;

namespace TaskManager.Api.Controllers
{
    /// <summary>
    /// Controlador para la gestión de comentarios de tareas.
    /// </summary>
    /// <remarks>
    /// Este controlador administra los endpoints relacionados con la entidad <see cref="TaskComment"/>,
    /// permitiendo realizar operaciones CRUD utilizando servicios, validaciones y mapeadores automáticos (AutoMapper).
    /// Los métodos incluyen versiones que trabajan con Dapper y soportan filtrado y paginación.
    /// </remarks>
    [Route("api/[controller]")]
    [ApiController]
    public class TaskCommentController : ControllerBase
    {
        private readonly ITaskCommentService _commentService;
        private readonly IMapper _mapper;
        private readonly IValidationService _validationService;

        public TaskCommentController(ITaskCommentService commentService, IMapper mapper, IValidationService validationService)
        {
            _commentService = commentService;
            _mapper = mapper;
            _validationService = validationService;
        }

        /// <summary>
        /// Recupera todos los comentarios utilizando Dapper, aplicando filtros opcionales.
        /// </summary>
        /// <remarks>
        /// Permite obtener la lista de comentarios registrados en la base de datos según los criterios enviados
        /// en <see cref="TaskCommentQueryFilter"/>.  
        /// Devuelve los resultados paginados dentro de un objeto estándar <see cref="ApiResponse{T}"/>.
        /// </remarks>
        /// <param name="filters">Parámetros opcionales de filtrado y paginación.</param>
        /// <returns>Colección paginada de comentarios de tareas.</returns>
        /// <response code="200">Comentarios recuperados correctamente.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet("dapper")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<TaskCommentDto>>))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAllComments([FromQuery] TaskCommentQueryFilter filters)
        {
            try
            {
                var comments = await _commentService.GetAllCommentsDapperAsync(filters);
                var commentsDto = _mapper.Map<IEnumerable<TaskCommentDto>>(comments.Pagination.Cast<TaskComment>());

                var pagination = new Pagination
                {
                    TotalCount = comments.Pagination.TotalCount,
                    PageSize = comments.Pagination.PageSize,
                    CurrentPage = comments.Pagination.CurrentPage,
                    TotalPages = comments.Pagination.TotalPages,
                    HasNextPage = comments.Pagination.HasNextPage,
                    HasPreviousPage = comments.Pagination.HasPreviousPage
                };

                var response = new ApiResponse<IEnumerable<TaskCommentDto>>(commentsDto)
                {
                    Pagination = pagination,
                    Messages = comments.Messages
                };

                return StatusCode((int)comments.StatusCode, response);
            }
            catch (Exception ex)
            {
                var responseError = new ResponseData()
                {
                    Messages = new[] { new Message { Type = "Error", Description = ex.Message } }
                };
                return StatusCode((int)HttpStatusCode.InternalServerError, responseError);
            }
        }

        /// <summary>
        /// Recupera un comentario específico por su identificador utilizando Dapper.
        /// </summary>
        /// <remarks>
        /// Obtiene los datos de un comentario según su <paramref name="commentId"/> y los retorna en formato DTO.
        /// </remarks>
        /// <param name="commentId">Identificador del comentario.</param>
        /// <returns>Objeto <see cref="TaskCommentDto"/> con los datos del comentario.</returns>
        /// <response code="200">Comentario encontrado correctamente.</response>
        /// <response code="404">Comentario no encontrado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet("dapper/{commentId}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<TaskCommentDto>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetCommentById(int commentId)
        {
            try
            {
                var comment = await _commentService.GetCommentByIdDapperAsync(commentId);
                var commentDto = _mapper.Map<TaskCommentDto>(comment);
                var response = new ApiResponse<TaskCommentDto>(commentDto);
                return Ok(response);
            }
            catch (BussinesException ex)
            {
                return NotFound(new ResponseData()
                {
                    Messages = new[] { new Message { Type = "Warning", Description = ex.Message } }
                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ResponseData()
                {
                    Messages = new[] { new Message { Type = "Error", Description = ex.Message } }
                });
            }
        }

        /// <summary>
        /// Recupera todos los comentarios asociados a una tarea específica.
        /// </summary>
        /// <remarks>
        /// Devuelve todos los comentarios relacionados con la tarea cuyo identificador se pasa como parámetro.
        /// </remarks>
        /// <param name="taskId">Identificador de la tarea.</param>
        /// <returns>Colección de comentarios asociados a la tarea.</returns>
        /// <response code="200">Comentarios encontrados correctamente.</response>
        /// <response code="404">No existen comentarios para la tarea indicada.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet("dapper/task/{taskId}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<TaskCommentDto>>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetCommentsByTaskId(int taskId)
        {
            try
            {
                var comments = await _commentService.GetCommentsByTaskIdAsync(taskId);
                var commentsDto = _mapper.Map<IEnumerable<TaskCommentDto>>(comments);
                var response = new ApiResponse<IEnumerable<TaskCommentDto>>(commentsDto);
                return Ok(response);
            }
            catch (BussinesException ex)
            {
                return NotFound(new ResponseData()
                {
                    Messages = new[] { new Message { Type = "Warning", Description = ex.Message } }
                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ResponseData()
                {
                    Messages = new[] { new Message { Type = "Error", Description = ex.Message } }
                });
            }
        }

        /// <summary>
        /// Inserta un nuevo comentario asociado a una tarea.
        /// </summary>
        /// <remarks>
        /// Valida la información enviada mediante el DTO <see cref="TaskCommentDto"/>,
        /// y si es válida, la inserta en la base de datos.
        /// </remarks>
        /// <param name="commentDto">Datos del comentario a insertar.</param>
        /// <returns>Objeto <see cref="TaskCommentDto"/> con el comentario creado.</returns>
        /// <response code="200">Comentario insertado correctamente.</response>
        /// <response code="400">Datos inválidos o falló la validación.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpPost("post")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<TaskCommentDto>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> InsertComment(TaskCommentDto commentDto)
        {
            try
            {
                var validationResult = await _validationService.ValidateAsync(commentDto);
                if (!validationResult.IsValid)
                    return BadRequest(new { Errors = validationResult.Errors });

                var comment = _mapper.Map<TaskComment>(commentDto);
                await _commentService.InsertCommentAsync(comment);

                var response = new ApiResponse<TaskCommentDto>(_mapper.Map<TaskCommentDto>(comment));
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ResponseData()
                {
                    Messages = new[] { new Message { Type = "Error", Description = ex.Message } }
                });
            }
        }

        /// <summary>
        /// Actualiza los datos de un comentario existente.
        /// </summary>
        /// <remarks>
        /// Valida los datos actualizados y verifica que el Id del comentario coincida con el del cuerpo de la solicitud.
        /// </remarks>
        /// <param name="id">Identificador del comentario a actualizar.</param>
        /// <param name="commentDto">Datos actualizados del comentario.</param>
        /// <returns>Objeto actualizado <see cref="TaskCommentDto"/>.</returns>
        /// <response code="200">Comentario actualizado correctamente.</response>
        /// <response code="400">El Id no coincide o los datos son inválidos.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpPut("put/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<TaskCommentDto>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdateComment(int id, TaskCommentDto commentDto)
        {
            try
            {
                if (id != commentDto.Id)
                    return BadRequest(new { Errors = new[] { "El Id del comentario no coincide" } });

                var validationResult = await _validationService.ValidateAsync(commentDto);
                if (!validationResult.IsValid)
                    return BadRequest(new { Errors = validationResult.Errors });

                var comment = await _commentService.GetCommentByIdDapperAsync(id);
                _mapper.Map(commentDto, comment);
                await _commentService.UpdateCommentAsync(comment);

                var response = new ApiResponse<TaskCommentDto>(_mapper.Map<TaskCommentDto>(comment));
                return Ok(response);
            }
            catch (BussinesException ex)
            {
                return BadRequest(new ResponseData()
                {
                    Messages = new[] { new Message { Type = "Warning", Description = ex.Message } }
                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ResponseData()
                {
                    Messages = new[] { new Message { Type = "Error", Description = ex.Message } }
                });
            }
        }

        /// <summary>
        /// Elimina un comentario existente de la base de datos.
        /// </summary>
        /// <remarks>
        /// Este método elimina el comentario especificado por su Id.
        /// </remarks>
        /// <param name="id">Identificador del comentario a eliminar.</param>
        /// <response code="204">Comentario eliminado correctamente.</response>
        /// <response code="404">Comentario no encontrado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpDelete("delete/{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteComment(int id)
        {
            try
            {
                await _commentService.DeleteCommentAsync(id);
                return NoContent();
            }
            catch (BussinesException ex)
            {
                return NotFound(new ResponseData()
                {
                    Messages = new[] { new Message { Type = "Warning", Description = ex.Message } }
                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ResponseData()
                {
                    Messages = new[] { new Message { Type = "Error", Description = ex.Message } }
                });
            }
        }
    }
}
