using FlagExplorerApp.Api.Controllers;
using FlagExplorerApp.Application.CountryDetail;
using FlagExplorerApp.Application.CountryDetails.GetCountryDetailByName;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
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
    public async Task GetCountryDetails_ShouldReturnOkResult_WhenCountryExists()
    {
        // Arrange
        var countryName = "TestCountry";
        var countryDetail = new CountryDetailDto
        {
            Name = countryName,
            Population = 1000000,
            Capital = "TestCapital",
            Flag = "TestFlagUrl"
        };

        _mediatorMock
            .Setup(m => m.Send(It.Is<GetCountryDetailByNameQuery>(q => q.Name == countryName), It.IsAny<CancellationToken>()))
            .ReturnsAsync(countryDetail);

        // Act
        var result = await _controller.GetCountryDetails(countryName, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(countryDetail);
    }

    [Test]
    public async Task GetCountryDetails_ShouldReturnNotFound_WhenCountryDoesNotExist()
    {
        // Arrange
        var countryName = "NonExistentCountry";

        _ = _mediatorMock
            .Setup(m => m.Send(It.Is<GetCountryDetailByNameQuery>(q => q.Name == countryName), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CountryDetailDto?)null);

        // Act
        var result = await _controller.GetCountryDetails(countryName, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Test]
    public async Task GetCountryDetails_ShouldHandleException_AndReturnInternalServerError()
    {
        // Arrange
        var countryName = "TestCountry";

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetCountryDetailByNameQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.GetCountryDetails(countryName, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<ObjectResult>();
        var objectResult = result.Result as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        objectResult.Value.Should().Be("An unexpected error occurred."); // Ensure proper error message is returned
    }

    [Test]
    public async Task GetCountryDetails_ShouldReturnBadRequest_WhenCountryNameIsNull()
    {
        // Arrange
        string? countryName = null;

        // Act
        var result = await _controller.GetCountryDetails(countryName!, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.Value.Should().Be("Country name cannot be null or empty."); // Ensure proper error message is returned
    }
}
