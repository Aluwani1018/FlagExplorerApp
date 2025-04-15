﻿using FlagExplorerApp.Api.Models;
using FlagExplorerApp.Application.CountryDetail;
using FlagExplorerApp.Application.CountryDetails.GetCountryDetailByName;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FlagExplorerApp.Api.Controllers;

[ApiController]
[Route("countries")]
public class CountryDetailController : ControllerBase
{
    private readonly IMediator _mediator;

    public CountryDetailController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieve details about a specific country.
    /// </summary>
    /// <param name="name">The name of the country to retrieve details for.</param>
    /// <returns>Details about the country.</returns>
    [HttpGet("{name}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    public async Task<ActionResult<CountryDetailDto>> GetCountryDetails(string name, CancellationToken cancellationToken)
    {
        //For error handling I would suggest using a middleware or a global exception handler
        //to avoid repeating the same code in every controller action.
        if (string.IsNullOrWhiteSpace(name))
        {
            return BadRequest(new ErrorResponse { Message = "The country name cannot be null or empty." });
        }

        try
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return BadRequest(new ErrorResponse { Message = "The operation was canceled by the client." });
            }

            var countryDetail = await _mediator.Send(new GetCountryDetailByNameQuery(name), cancellationToken);

            if (countryDetail == null)
            {
                return NotFound(new ErrorResponse { Message = $"Country with name '{name}' was not found." });
            }

            return Ok(countryDetail);
        }
        catch (OperationCanceledException)
        {
            return BadRequest(new ErrorResponse { Message = "The operation was canceled by the client." });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse
            {
                Message = "An unexpected error occurred while processing your request.",
                Details = ex.Message 
            });
        }
    }
}
