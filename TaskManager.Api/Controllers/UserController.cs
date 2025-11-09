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

namespace TaskManager.Api.Controllers
{
    /// <summary>
    /// Controlador para la gestión de usuarios, que utiliza DAPPER para gets.
    /// </summary>
    /// <remarks>
    /// Este controlador contiene los endpoints relacionados con la entidad <see cref="User"/>, 
    /// implementados utilizando servicios, validaciones y mapeadores automáticos.
    /// </remarks>
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IValidationService _validationService;

        public UserController(IUserService userService, IMapper mapper, IValidationService validationService)
        {
            _userService = userService;
            _mapper = mapper;
            _validationService = validationService;
        }

        /// <summary>
        /// Recupera todos los usuarios con Entity Framework Core y paginación.
        /// </summary>
        /// <remarks>
        /// Este método obtiene una lista de usuarios filtrada según los parámetros enviados en <see cref="UserQueryFilter"/>.
        /// Convierte los resultados a DTOs y devuelve la información junto con la paginación.
        /// </remarks>
        /// <param name="userQueryFilter">Filtro de búsqueda y paginación para los usuarios.</param>
        /// <returns>Colección de usuarios en formato <see cref="UserDto"/> con paginación.</returns>
        /// <response code="200">Usuarios recuperados correctamente.</response>
        /// <response code="400">Solicitud incorrecta o datos de filtro inválidos.</response>
        /// <response code="500">Error interno del servidor.</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<UserDto>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [HttpGet("get/efc")]
        public async Task<IActionResult> GetAllUsers([FromQuery] UserQueryFilter userQueryFilter)
        {
            try
            {
                var users = await _userService.GetAllUsersAsync(userQueryFilter);
                var usersDto = _mapper.Map<IEnumerable<UserDto>>(users.Pagination);

                var pagination = new Pagination
                {
                    TotalCount = users.Pagination.TotalCount,
                    PageSize = users.Pagination.PageSize,
                    CurrentPage = users.Pagination.CurrentPage,
                    TotalPages = users.Pagination.TotalPages,
                    HasNextPage = users.Pagination.HasNextPage,
                    HasPreviousPage = users.Pagination.HasPreviousPage
                };
                var response = new ApiResponse<IEnumerable<UserDto>>(usersDto)
                {
                    Pagination = pagination,
                    Messages = users.Messages
                };

                return StatusCode((int)users.StatusCode, response);
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
        /// Recupera un usuario por su Id utilizando Entity Framework Core.
        /// </summary>
        /// <remarks>
        /// Este método obtiene la información del usuario según el Id proporcionado y la devuelve como DTO.
        /// </remarks>
        /// <param name="id">Identificador del usuario</param>
        /// <returns>Objeto <see cref="UserDto"/> con los datos del usuario.</returns>
        /// <response code="200">Usuario encontrado correctamente.</response>
        /// <response code="404">Usuario no encontrado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<UserDto>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [HttpGet("get/efc/{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            try
            {
                var user = await _userService.GetUserAsync(id);
                var userDto = _mapper.Map<UserDto>(user);
                var response = new ApiResponse<UserDto>(userDto);
                return Ok(response);
            }
            catch (Exception err)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Recupera todos los usuarios registrados en la base de datos usando Dapper.
        /// </summary>
        /// <remarks>
        /// Este método realiza la consulta usando Dapper y retorna la información estandarizada en <see cref="ApiResponse{T}"/>.
        /// </remarks>
        /// <param name="filters">Filtros de búsqueda y paginación.</param>
        /// <returns>Colección de usuarios con paginación.</returns>
        /// <response code="200">Usuarios recuperados exitosamente.</response>
        /// <response code="404">No se encontraron usuarios.</response>
        /// <response code="500">Error interno del servidor.</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<UserDto>>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [HttpGet("get/dapper")]
        public async Task<IActionResult> GetAllUsersDapper([FromQuery] UserQueryFilter filters)
        {
            try
            {
                var users = await _userService.GetAllUsersDapperAsync(filters);
                var usersDto = _mapper.Map<IEnumerable<UserDto>>(users.Pagination);

                var pagination = new Pagination
                {
                    TotalCount = users.Pagination.TotalCount,
                    PageSize = users.Pagination.PageSize,
                    CurrentPage = users.Pagination.CurrentPage,
                    TotalPages = users.Pagination.TotalPages,
                    HasNextPage = users.Pagination.HasNextPage,
                    HasPreviousPage = users.Pagination.HasPreviousPage
                };
                var response = new ApiResponse<IEnumerable<UserDto>>(usersDto)
                {
                    Pagination = pagination,
                    Messages = users.Messages
                };

                return StatusCode((int)users.StatusCode, response);
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
        /// Recupera la información de un usuario por su Id usando Dapper.
        /// </summary>
        /// <remarks>
        /// Consulta la tabla User con Dapper y devuelve un objeto estandarizado con la información del usuario.
        /// </remarks>
        /// <param name="id">Identificador del usuario.</param>
        /// <returns>Objeto <see cref="ApiResponse{UserResponse}"/> con los datos del usuario.</returns>
        /// <response code="200">Usuario encontrado correctamente.</response>
        /// <response code="404">Usuario no encontrado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<UserResponse>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [HttpGet("get/dapper/{id}")]
        public async Task<IActionResult> GetUserByIdDapper(int id)
        {
            try
            {
                var result = await _userService.GetUserByIdDapperAsync(id);
                var response = new ApiResponse<object>(result.Data)
                {
                    Messages = result.Messages
                };
                return StatusCode((int)result.StatusCode, response);
            }
            catch (Exception err)
            {
                var responseError = new ResponseData()
                {
                    Messages = new[] { new Message { Type = "Error", Description = err.Message } },
                };
                return StatusCode(500, responseError);
            }
        }

        /// <summary>
        /// Inserta un nuevo usuario en la base de datos.
        /// </summary>
        /// <remarks>
        /// Valida la información del usuario antes de insertar y retorna el registro insertado en formato estandarizado.
        /// </remarks>
        /// <param name="userDto">Objeto DTO con los datos del usuario a insertar.</param>
        /// <returns>Objeto <see cref="ApiResponse{User}"/> con la información del usuario creado.</returns>
        /// <response code="200">Usuario insertado correctamente.</response>
        /// <response code="400">Datos inválidos o falló la validación.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpPost("insert")]
        public async Task<IActionResult> InsertUser([FromBody] UserDto userDto)
        {
            try
            {
                var validationResult = await _validationService.ValidateAsync(userDto);
                if (!validationResult.IsValid)
                    return BadRequest(new { Errors = validationResult.Errors });

                var user = _mapper.Map<User>(userDto);
                await _userService.InsertUserAsync(user);

                var response = new ApiResponse<User>(user);
                return Ok(response);
            }
            catch (Exception err)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Actualiza un usuario existente.
        /// </summary>
        /// <remarks>
        /// Valida la información proporcionada y actualiza el registro en la base de datos.
        /// </remarks>
        /// <param name="id">Identificador del usuario a actualizar.</param>
        /// <param name="userDto">Objeto DTO con los datos actualizados del usuario.</param>
        /// <returns>Objeto <see cref="ApiResponse{User}"/> con la información actualizada.</returns>
        /// <response code="200">Usuario actualizado correctamente.</response>
        /// <response code="400">Id no coincide o datos inválidos.</response>
        /// <response code="404">Usuario no encontrado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDto userDto)
        {
            try
            {
                if (id != userDto.Id)
                    return BadRequest("El Id no coincide");

                var validationResult = await _validationService.ValidateAsync(userDto);
                if (!validationResult.IsValid)
                    return BadRequest(new { Errors = validationResult.Errors });

                var existingUser = await _userService.GetUserAsync(id);
                if (existingUser == null)
                    return NotFound("Usuario no encontrado");

                _mapper.Map(userDto, existingUser);
                await _userService.UpdateUserAsync(existingUser);

                var response = new ApiResponse<User>(existingUser);
                return Ok(response);
            }
            catch (Exception err)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Elimina un usuario existente.
        /// </summary>
        /// <remarks>
        /// Este método elimina el registro del usuario por su Id en la base de datos.
        /// </remarks>
        /// <param name="id">Identificador del usuario a eliminar.</param>
        /// <response code="204">Usuario eliminado correctamente.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                await _userService.DeleteUserAsync(id);
                return NoContent();
            }
            catch (Exception err)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, err.Message);
            }
        }
    }
}
