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
    public class TaskEntityController : ControllerBase
    {
        private readonly ITaskEntityService _taskService; // Servicio principal para tareas
        private readonly IMapper _mapper;
        private readonly IValidationService _validationService;

        public TaskEntityController(ITaskEntityService taskService, IMapper mapper, IValidationService validationService)
        {
            _taskService = taskService;
            _mapper = mapper;
            _validationService = validationService;
        }

        [HttpGet("dto/mapper")]
        public async Task<IActionResult> GetTasksDtoMapper()
        {
            try
            {
                var tasks = await _taskService.GetAllTasksAsync();
                var tasksDto = _mapper.Map<IEnumerable<TaskEntityDto>>(tasks);

                var response = new ApiResponse<IEnumerable<TaskEntityDto>>(tasksDto);
                return Ok(response);
            }
            catch (Exception err)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, err.Message);
                throw;
            }
        }

        [HttpGet("dto/mapper/{id}")]
        public async Task<IActionResult> GetTaskEntityDtoMapperById(int id)
        {
            try
            {
                var task = await _taskService.GetTaskAsync(id);
                var TaskEntityDto = _mapper.Map<TaskEntityDto>(task);

                var response = new ApiResponse<TaskEntityDto>(TaskEntityDto);
                return Ok(task); // para ver la informacion completa con proyecto y status de actividad, cambiar?
            }
            catch (Exception err)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, err.Message);
                throw;
            }
        }
        // insercion con validacion
        /*
        {
            "ProjectId": 1,
            "StatusId": 1,
            "Title": "Actualizar documentación",
            "Description": "Revisar y actualizar el manual de usuario",
            "DueDate": "28-12-2025"
        }
         */
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
                throw;
            }
        }

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
                throw;
            }
        }

        [HttpDelete("dto/mapper/{id}")]
        public async Task<IActionResult> DeleteTaskEntityDtoMapper(int id)
        {
            try
            {
                var task = await _taskService.GetTaskAsync(id);

                await _taskService.DeleteTaskAsync(task);
                return NoContent();
            }
            catch (Exception err)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, err.Message);
                throw;
            }
        }

        [HttpGet("dto/mapper/search")]
        public async Task<IActionResult> SearchTasks([FromQuery] string text) // buscar una tarea por texto
        {
            try
            {
                var tasks = await _taskService.SearchTasksAsync(text);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("dto/mapper/projecttasks/{projectId}")]
        public async Task<IActionResult> GetAllProjectTasks(int projectId, [FromQuery] int? statusId) // buscar tareas de un proyecto, opcionalmente filtrar por estado
        {
            try
            {
                var tasks = await _taskService.GetAllProjectTasksAsync(projectId, statusId);
                var tasksDto = _mapper.Map<IEnumerable<TaskEntityDto>>(tasks);
                var response = new ApiResponse<IEnumerable<TaskEntityDto>>(tasksDto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
