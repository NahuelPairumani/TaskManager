using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TaskManager.Api.Responses;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;
using TaskManager.Core.Services;
using TaskManager.Infrastructure.DTOs;
using TaskManager.Infrastructure.Validators;

namespace TaskManager.Api.Controllers
{
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

        [HttpGet("get")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var assignments = await _service.GetAllAssignmentsAsync();
                var dto = _mapper.Map<IEnumerable<TaskAssignmentDto>>(assignments);
                return Ok(new ApiResponse<IEnumerable<TaskAssignmentDto>>(dto));
            }
            catch (Exception err)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, err.Message);
                throw;
            }
        }

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
                throw;
            }
        }

        [HttpGet("tasks/user/{userId}")]
        public async Task<IActionResult> GetTasksByUser(int userId)
        {
            try
            {
                var tasks = await _service.GetTasksByUserAsync(userId);
                var dto = _mapper.Map<IEnumerable<TaskEntityDto>>(tasks);
                return Ok(new ApiResponse<IEnumerable<TaskEntityDto>>(dto));
            }
            catch (Exception err)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, err.Message);
                throw;
            }
        }

        [HttpGet("users/task/{taskId}")]
        public async Task<IActionResult> GetUsersByTask(int taskId)
        {
            try
            {
                var users = await _service.GetUsersByTaskAsync(taskId);
                var dto = _mapper.Map<IEnumerable<UserDto>>(users);
                return Ok(new ApiResponse<IEnumerable<UserDto>>(dto));
            }
            catch (Exception err)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, err.Message);
                throw;
            }
        }
    }
}
