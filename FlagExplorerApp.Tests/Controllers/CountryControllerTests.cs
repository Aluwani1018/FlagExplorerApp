
using FlagExplorerApp.Api.Controllers;
using FlagExplorerApp.Application.Countries.GetCountries;
using FlagExplorerApp.Application.Country;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace FlagExplorerApp.Tests.Controllers;

[TestFixture]
public class CountryControllerTests
{
    private Mock<IMediator> _mediatorMock;
    private CountryController _controller;

    [SetUp]
    public void SetUp()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new CountryController(_mediatorMock.Object);
    }

    [Test]
    public async Task GetAllCountries_ShouldReturnOk_WhenCountriesAreRetrievedSuccessfully()
    {
        // Arrange
        var countries = new List<CountryDto>
            {
                new CountryDto {Name = "Country1", Flag = "Flag1"},
                new CountryDto {Name = "Country2", Flag = "Flag2"}
            };
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetCountriesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(countries);

        // Act
        var result = await _controller.GetAllCountries(CancellationToken.None);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(countries);
    }

    [Test]
    public async Task GetAllCountries_ShouldReturnBadRequest_WhenCancellationTokenIsRequested()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        var result = await _controller.GetAllCountries(cancellationTokenSource.Token);

        // Assert
        var badRequestResult = result.Result as BadRequestObjectResult; // Expecting BadRequest
        badRequestResult.Should().NotBeNull();
        badRequestResult.StatusCode.Should().Be(400);
    }

    [Test]
    public async Task GetAllCountries_ShouldHandleEmptyCountryListSuccessfully()
    {
        // Arrange
        var countries = new List<CountryDto>(); // Empty list
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetCountriesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(countries);

        // Act
        var result = await _controller.GetAllCountries(CancellationToken.None);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);
        var responseCountries = okResult.Value as List<CountryDto>;
        responseCountries.Should().NotBeNull();
        responseCountries.Should().BeEmpty();
    }

    [Test]
    public async Task GetAllCountries_ShouldReturnOk_WhenOnlyOneCountryIsRetrieved()
    {
        // Arrange
        var countries = new List<CountryDto>
            {
                new CountryDto { Name = "Country1", Flag = "Flag1" }
            };
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetCountriesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(countries);

        // Act
        var result = await _controller.GetAllCountries(CancellationToken.None);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);
        var responseCountries = okResult.Value as List<CountryDto>;
        responseCountries.Should().NotBeNull();
        responseCountries.Should().HaveCount(1);
        responseCountries[0].Should().BeEquivalentTo(countries[0]);
    }
}