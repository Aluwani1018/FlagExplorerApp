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
        try
        {
            var countries = await _mediator.Send(new GetCountriesQuery(), cancellationToken);
            return Ok(countries);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Message = ex.Message, Details = ex.StackTrace });
        }
        catch (OperationCanceledException)
        {
            return BadRequest(new { Message = "The operation was canceled by the client." });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                Message = "An unexpected error occurred. Please try again later.",
                Details = ex.Message
            });
        }

    }
}
