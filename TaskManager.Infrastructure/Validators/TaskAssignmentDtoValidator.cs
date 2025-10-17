using FluentValidation;
using TaskManager.Infrastructure.DTOs;

namespace TaskManager.Infrastructure.Validators
{
    public class TaskAssignmentDtoValidator : AbstractValidator<TaskAssignmentDto>
    {
        public TaskAssignmentDtoValidator()
        {
            RuleFor(x => x.TaskId)
                .GreaterThan(0).WithMessage("Debe especificarse una tarea válida")
                .NotEmpty().WithMessage("El TaskId es obligatorio.");

            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("Debe especificarse un usuario válido")
                .NotEmpty().WithMessage("El UserId es obligatorio.");
        }
    }
}