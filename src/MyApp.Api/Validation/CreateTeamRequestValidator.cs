using FluentValidation;
using MyApp.Api.Models;
using MyApp.Domain;

namespace MyApp.Api.Validation;

public sealed class CreateTeamRequestValidator : AbstractValidator<CreateTeamRequest>
{
    public CreateTeamRequestValidator()
    {
        RuleFor(x => x.UniverseId)
            .GreaterThan(0);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Attackers)
            .InclusiveBetween(0, 4);

        RuleFor(x => x.Defenders)
            .InclusiveBetween(0, 4);

        RuleFor(x => x)
            .Must(x => x.Attackers + x.Defenders == Constants.NumberOfPlayers - 1)
            .WithMessage($"Attackers and defenders combined should be {Constants.NumberOfPlayers - 1}.");
    }
}