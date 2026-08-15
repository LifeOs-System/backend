using Domain.Entities.Habits;
using FluentValidation;

namespace Application.Services.Habits.Create;


public class CreateHabitRequestValidator
    : AbstractValidator<CreateHabitRequest>
{
    public CreateHabitRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("El nombre del hábito es obligatorio.")
            .MaximumLength(100)
            .WithMessage("El nombre del hábito no puede superar los 100 caracteres.");

        RuleFor(x => x.Area)
            .IsInEnum()
            .WithMessage("El área seleccionada no es válida.");

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("El tipo de hábito no es válido.");

        RuleFor(x => x.Days)
            .NotEmpty()
            .WithMessage("El hábito debe tener al menos un día.");

        RuleForEach(x => x.Days)
            .IsInEnum()
            .WithMessage("El día seleccionado no es válido.");

        // Time / Quantity
        When(x => x.Type is HabitType.Time or HabitType.Quantity, () =>
        {
            RuleFor(x => x.Target)
                .NotNull()
                .WithMessage("El objetivo es obligatorio para hábitos de tiempo o cantidad.")
                .GreaterThan(0)
                .WithMessage("El objetivo debe ser mayor que 0.");

            RuleFor(x => x.Unit)
                .NotEmpty()
                .WithMessage("La unidad es obligatoria para hábitos de tiempo o cantidad.")
                .MaximumLength(20)
                .WithMessage("La unidad no puede superar los 20 caracteres.");
        });

        // Binary
        When(x => x.Type == HabitType.Binary, () =>
        {
            RuleFor(x => x.Target)
                .Null()
                .WithMessage("Un hábito binario no puede tener un objetivo.");

            RuleFor(x => x.Unit)
                .Null()
                .WithMessage("Un hábito binario no puede tener una unidad.");
        });
    }
}