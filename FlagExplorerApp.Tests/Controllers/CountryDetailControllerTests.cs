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
    private Mock<IMediator> _mediatorMock = null!;
    private CountryDetailController _controller = null!;

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

        _mediatorMock
            .Setup(m => m.Send(It.Is<GetCountryDetailByNameQuery>(q => q.Name == countryName), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CountryDetailDto?)null);

        // Act
        var result = await _controller.GetCountryDetails(countryName, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }
}
