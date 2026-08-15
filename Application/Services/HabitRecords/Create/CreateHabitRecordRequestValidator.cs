using Application.Services.HabitRecords.Create;
using FluentValidation;

namespace Application.Features.Habits.Records.Create;

public class CreateHabitRecordRequestValidator : AbstractValidator<CreateHabitRecordRequest>
{
    public CreateHabitRecordRequestValidator()
    {
        RuleFor(x => x.HabitId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("El HabitId es requerido.")
            .Must(id => Guid.TryParse(id, out _))
            .WithMessage("El HabitId debe ser un GUID válido.");

        RuleFor(x => x.Value)
            .Must(value => value is null || value >= 0)
            .WithMessage("El Value no puede ser negativo.");

        When(x => x.Value.HasValue, () =>
        {
            RuleFor(x => x.IsCompleted)
                .Null()
                .WithMessage("Si envías Value, no puedes enviar IsCompleted.");
        });

        When(x => x.IsCompleted.HasValue, () =>
        {
            RuleFor(x => x.Value)
                .Null()
                .WithMessage("Si envías IsCompleted, no puedes enviar Value.");
        });

        RuleFor(x => x)
            .Must(x => x.Value.HasValue || x.IsCompleted.HasValue)
            .WithMessage("Debes enviar al menos Value o IsCompleted.")
            .WithName("CreateHabitRecordRequest");
    }
}