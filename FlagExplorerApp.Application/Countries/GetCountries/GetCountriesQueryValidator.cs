using FluentValidation;

namespace FlagExplorerApp.Application.Countries.GetCountries;

public class GetCountriesQueryValidator : AbstractValidator<GetCountriesQuery>
{
    public GetCountriesQueryValidator()
    {
        ConfigureValidationRules();
    }

    private void ConfigureValidationRules()
    {
    }
}
