using FlagExplorerApp.Application.Countries.GetCountries;
using FlagExplorerApp.Application.Country;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FlagExplorerApp.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CountryController : ControllerBase
{
    private readonly IMediator _mediator;

    public CountryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieve all countries.
    /// </summary>
    /// <returns>A list of countries.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CountryDto>>> GetAllCountries(CancellationToken cancellationToken)
    {
        var countries = await _mediator.Send(new GetCountriesQuery(), cancellationToken);
        return Ok(countries);
    }
}
