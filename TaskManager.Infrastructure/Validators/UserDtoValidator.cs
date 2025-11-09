using FluentValidation;
using TaskManager.Infrastructure.DTOs;

namespace TaskManager.Infrastructure.Validators
{
    public class UserDtoValidator : AbstractValidator<UserDto>
    {
        public UserDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("El nombre es obligatorio")
                .MaximumLength(100).WithMessage("El nombre no puede superar 100 caracteres");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("El nombre es obligatorio")
                .MaximumLength(100).WithMessage("El nombre no puede superar 100 caracteres");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El correo electrónico es obligatorio")
                .EmailAddress().WithMessage("Debe ingresar un correo electrónico válido")
                .MaximumLength(150).WithMessage("El correo electrónico no puede superar 150 caracteres");
        }
    }
}
