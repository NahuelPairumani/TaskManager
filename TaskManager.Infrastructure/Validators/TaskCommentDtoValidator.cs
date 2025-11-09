using FluentValidation;
using TaskManager.Infrastructure.DTOs;

namespace TaskManager.Infrastructure.Validators
{
    /// <summary>
    /// Validador para TaskCommentDto.
    /// </summary>
    public class TaskCommentDtoValidator : AbstractValidator<TaskCommentDto>
    {
        public TaskCommentDtoValidator()
        {
            RuleFor(x => x.TaskId)
                .GreaterThan(0).WithMessage("Debe asignarse una tarea válida");

            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("Debe asignarse un usuario válido");

            RuleFor(x => x.Comment)
                .NotEmpty().WithMessage("El comentario es obligatorio")
                .MaximumLength(500).WithMessage("El comentario no puede exceder 500 caracteres");
        }
    }
}
