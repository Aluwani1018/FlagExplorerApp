using FlagExplorerApp.Api.Controllers;
using FlagExplorerApp.Api.Models;
using FlagExplorerApp.Application.CountryDetail;
using FlagExplorerApp.Application.CountryDetails.GetCountryDetailByName;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace FlagExplorerApp.Tests.Controllers;

[TestFixture]
public class CountryDetailControllerTests
{
    private Mock<IMediator> _mediatorMock;
    private CountryDetailController _controller;

    [SetUp]
    public void SetUp()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new CountryDetailController(_mediatorMock.Object);
    }

    [Test]
    public async Task GetCountryDetails_ShouldReturnBadRequest_WhenNameIsNullOrEmpty()
    {
        // Arrange
        string invalidName = null;

        // Act
        var result = await _controller.GetCountryDetails(invalidName, CancellationToken.None);

        // Assert
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.StatusCode.Should().Be(400);

        // Cast the Value to the expected type
        var response = badRequestResult.Value as ErrorResponse;
        response.Should().NotBeNull();
        response.Message.Should().Be("The country name cannot be null or empty.");
    }

    [Test]
    public async Task GetCountryDetails_ShouldReturnNotFound_WhenCountryDoesNotExist()
    {
        // Arrange
        string countryName = "NonExistentCountry";
        _mediatorMock
            .Setup(m => m.Send(It.Is<GetCountryDetailByNameQuery>(q => q.Name == countryName), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CountryDetailDto)null);

        // Act
        var result = await _controller.GetCountryDetails(countryName, CancellationToken.None);

        // Assert
        var notFoundResult = result.Result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult.StatusCode.Should().Be(404);
        var response = notFoundResult.Value as ErrorResponse;
        response.Message.Should().Be($"Country with name '{countryName}' was not found.");
    }

    [Test]
    public async Task GetCountryDetails_ShouldReturnOk_WhenCountryExists()
    {
        // Arrange
        string countryName = "TestCountry";
        var countryDetail = new CountryDetailDto
        {
            Name = "TestCountry",
            Capital = "TestCapital",
            Population = 1000,
            Flag = "TestFlag"
        };

        _mediatorMock
            .Setup(m => m.Send(It.Is<GetCountryDetailByNameQuery>(q => q.Name == countryName), It.IsAny<CancellationToken>()))
            .ReturnsAsync(countryDetail);

        // Act
        var result = await _controller.GetCountryDetails(countryName, CancellationToken.None);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(countryDetail);
    }

    [Test]
    public async Task GetCountryDetails_ShouldReturnBadRequest_WhenOperationIsCanceled()
    {
        // Arrange
        string countryName = "TestCountry";
        var canceledTokenSource = new CancellationTokenSource();
        canceledTokenSource.Cancel();

        // Act
        var result = await _controller.GetCountryDetails(countryName, canceledTokenSource.Token);

        // Assert
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.StatusCode.Should().Be(400);

        var response = badRequestResult.Value as ErrorResponse;
        response.Message.Should().Be("The operation was canceled by the client.");
    }

    [Test]
    public async Task GetCountryDetails_ShouldReturnInternalServerError_WhenUnhandledExceptionOccurs()
    {
        // Arrange
        string countryName = "TestCountry";
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetCountryDetailByNameQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await _controller.GetCountryDetails(countryName, CancellationToken.None);

        // Assert
        var statusResult = result.Result as ObjectResult;
        statusResult.Should().NotBeNull();
        statusResult.StatusCode.Should().Be(500);

        var response = statusResult.Value as ErrorResponse;
        response.Message.Should().Be("An unexpected error occurred while processing your request.");
        response.Details.Should().Be("Unexpected error");
    }
}