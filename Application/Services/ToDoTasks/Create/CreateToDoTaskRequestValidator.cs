using FluentValidation;

namespace Application.Services.ToDoTasks.Create;


public class CreateToDoTaskRequestValidator : AbstractValidator<CreateToDoTaskRequest>
{
    public CreateToDoTaskRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre de la tarea es obligatorio.")
            .NotNull().WithMessage("El nombre de la tarea no puede ser nulo.")
            .MaximumLength(100).WithMessage("El nombre de la tarea no puede exceder los 100 caracteres.")
            .Matches(@"^\S.*\S$").WithMessage("El nombre no puede ser solo espacios en blanco.");

        RuleFor(x => x.Date)
            .Must(date => date == null || date >= DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("La fecha de la tarea no puede ser en el pasado.");
    }
}
