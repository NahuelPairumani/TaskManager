using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TaskManager.Api.Responses;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;
using TaskManager.Infrastructure.DTOs;
using TaskManager.Infrastructure.Validators;

namespace TaskManager.Api.Controllers
{
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

        [HttpGet("get")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                var usersDto = _mapper.Map<IEnumerable<UserDto>>(users);
                var response = new ApiResponse<IEnumerable<UserDto>>(usersDto);
                return Ok(response);
            }
            catch (Exception err)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, err.Message);
                throw;
            }
        }

        [HttpGet("get/{id}")]
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
                throw;
            }
        }

        [HttpPost("insert")]
        public async Task<IActionResult> InsertUser([FromBody] UserDto userDto)
        {
            try
            {
                var validationResult = await _validationService.ValidateAsync(userDto);
                if (!validationResult.IsValid)
                    return BadRequest(new { Errors = validationResult.Errors });

                var user = _mapper.Map<User>(userDto);
                user.PasswordHash = userDto.Password;
                await _userService.InsertUserAsync(user);

                var response = new ApiResponse<User>(user);
                return Ok(response);
            }
            catch (Exception err)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, err.Message);
                throw;
            }
        }

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

                existingUser.PasswordHash = userDto.Password;
                await _userService.UpdateUserAsync(existingUser);

                var response = new ApiResponse<User>(existingUser);
                return Ok(response);
            }
            catch (Exception err)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, err.Message);
                throw;
            }
        }


        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var user = await _userService.GetUserAsync(id);
                await _userService.DeleteUserAsync(user);
                return NoContent();
            }
            catch (Exception err)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, err.Message);
                throw;
            }
        }
    }
}
