using FlagExplorerApp.Application.Common.Middleware;
using FlagExplorerApp.Application.Countries.GetCountries;
using FlagExplorerApp.Application.Country;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FlagExplorerApp.Api.Controllers;

[ApiController]
[Route("countries")]
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
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<CountryDto>>> GetAllCountries(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return BadRequest(new ErrorResponse { Message = "The operation was canceled by the client." });
        }

        var countries = await _mediator.Send(new GetCountriesQuery(), cancellationToken);
        return Ok(countries);
    }
}
