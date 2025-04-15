using FlagExplorerApp.Api.Controllers;
using FlagExplorerApp.Application.Common.Middleware;
using FlagExplorerApp.Application.CountryDetail;
using FlagExplorerApp.Application.CountryDetails.GetCountryDetailByName;
using FluentAssertions;
using MediatR;
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
    public async Task GetCountryDetails_ShouldReturnOk_WhenCountryExists()
    {
        // Arrange
        string countryName = "Country1";
        var countryDetail = new CountryDetailDto
        {
            Name = "Country1",
            Capital = "CapitalCity",
            Population = 1000000,
            Flag = "FlagUrl"
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
    public async Task GetCountryDetails_ShouldReturnBadRequest_WhenCancellationTokenIsRequested()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        var result = await _controller.GetCountryDetails("Country1", cancellationTokenSource.Token);

        // Assert
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.StatusCode.Should().Be(400);
        var response = badRequestResult.Value as ErrorResponse;
        response.Should().NotBeNull();
        response.Message.Should().Be("The operation was canceled by the client.");
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
        response.Should().NotBeNull();
        response.Message.Should().Be($"Country with name '{countryName}' was not found.");
    }

    [Test]
    public async Task GetCountryDetails_ShouldReturnBadRequest_WhenNameIsNull()
    {
        // Arrange
        string countryName = null;

        // Act
        var result = await _controller.GetCountryDetails(countryName, CancellationToken.None);

        // Assert
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.StatusCode.Should().Be(400);
        var response = badRequestResult.Value as ErrorResponse;
        response.Should().NotBeNull();
        response.Message.Should().Be("The country name cannot be null or empty.");
    }
}