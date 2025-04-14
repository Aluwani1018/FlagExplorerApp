using FluentValidation;

namespace FlagExplorerApp.Application.CountryDetails.GetCountryDetailByName;

public class GetCountryDetailByNameQueryValidator : AbstractValidator<GetCountryDetailByNameQuery>
{
    public GetCountryDetailByNameQueryValidator()
    {
        ConfigureValidationRules();
    }

    private void ConfigureValidationRules()
    {
        RuleFor(v => v.Name)
            .NotNull().WithMessage("Country name must not be null.")
            .NotEmpty().WithMessage("Country name must not be empty.")
            .MinimumLength(2)
            .MaximumLength(100);
    }
}
