using FluentValidation;
using System.Globalization;
using TaskManager.Infrastructure.DTOs;

namespace TaskManager.Infrastructure.Validators
{
    public class TaskEntityDtoValidator : AbstractValidator<TaskEntityDto>
    {
        public TaskEntityDtoValidator() // Para Validar la estructura, formato y consistencia básica de los datos recibidos.
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("El título es obligatorio")
                .MaximumLength(100).WithMessage("El título no puede superar 100 caracteres");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("La descripción es obligatoria")
                .MaximumLength(500).WithMessage("La descripción no puede exceder 500 caracteres");

            RuleFor(x => x.ProjectId) // projectId mayor a 0
                .GreaterThan(0).WithMessage("Debe asignarse un proyecto válido");

            When(x => !string.IsNullOrEmpty(x.DueDate), () =>
            {
                RuleFor(x => x.DueDate)
                    .Must(BeValidDateFormat).WithMessage("La fecha debe tener el formato dd-MM-yyyy")
                    .Must(BeGreaterOrEqualThanToday).WithMessage("La fecha no puede ser anterior a la fecha actual"); //fue aplicado en el servicio
            });
        }

        private bool BeValidDateFormat(string fecha)
        {
            return DateTime.TryParseExact(
                fecha,
                "dd-MM-yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out _
            );
        }

        private bool BeGreaterOrEqualThanToday(string fecha)
        {
            if (DateTime.TryParseExact(
                    fecha,
                    "dd-MM-yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime result))
            {
                return result.Date >= DateTime.Now.Date;
            }
            return false;
        }
    }
}
