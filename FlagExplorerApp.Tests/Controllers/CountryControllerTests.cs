
using FlagExplorerApp.Api.Controllers;
using FlagExplorerApp.Application.Countries.GetCountries;
using FlagExplorerApp.Application.Country;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

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
    public async Task GetAllCountries_ShouldReturnOkWithListOfCountries()
    {
        // Arrange
        var countries = new List<CountryDto>
            {
                new CountryDto { Name = "Country1", Flag = "Flag1" },
                new CountryDto { Name = "Country2", Flag = "Flag2" }
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

        _mediatorMock.Verify(m => m.Send(It.IsAny<GetCountriesQuery>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GetAllCountries_ShouldHandleEmptyList()
    {
        // Arrange
        var countries = new List<CountryDto>();

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

        _mediatorMock.Verify(m => m.Send(It.IsAny<GetCountriesQuery>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
