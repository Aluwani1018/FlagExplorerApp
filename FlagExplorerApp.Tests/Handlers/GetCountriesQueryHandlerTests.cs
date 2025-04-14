
using AutoMapper;
using FlagExplorerApp.Application.Countries.GetCountries;
using FlagExplorerApp.Application.Country;
using FlagExplorerApp.Domain.Entities;
using FlagExplorerApp.Domain.Repositories;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace FlagExplorerApp.Tests.Handlers;


[TestFixture]
public class GetCountriesQueryHandlerTests
{
    private Mock<ICountryRepository> _countryRepositoryMock;
    private Mock<IMapper> _mapperMock;
    private GetCountriesQueryHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _countryRepositoryMock = new Mock<ICountryRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetCountriesQueryHandler(_countryRepositoryMock.Object, _mapperMock.Object);
    }

    [Test]
    public async Task Handle_ShouldReturnMappedCountryDtos_WhenCountriesExist()
    {
        // Arrange
        var countries = new List<Country>
        {
            new Country { Id = Guid.NewGuid().ToString(), Name = "Country1", Flag = "Flag1" },
            new Country { Id = Guid.NewGuid().ToString(), Name = "Country2", Flag = "Flag2" }
        };

        var countryDtos = new List<CountryDto>
        {
            new CountryDto { Name = "Country1", Flag = "Flag1" },
            new CountryDto { Name = "Country2", Flag = "Flag2" }
        };

        _countryRepositoryMock
            .Setup(repo => repo.FindAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(countries);

        _mapperMock
            .Setup(mapper => mapper.Map<List<CountryDto>>(countries))
            .Returns(countryDtos);

        var query = new GetCountriesQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(countryDtos);

        _countryRepositoryMock.Verify(repo => repo.FindAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mapperMock.Verify(mapper => mapper.Map<List<CountryDto>>(countries), Times.Once);
    }

    [Test]
    public async Task Handle_ShouldReturnEmptyList_WhenNoCountriesExist()
    {
        // Arrange
        var countries = new List<Country>();
        var countryDtos = new List<CountryDto>();

        _countryRepositoryMock
            .Setup(repo => repo.FindAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(countries);

        _mapperMock
            .Setup(mapper => mapper.Map<List<CountryDto>>(countries))
            .Returns(countryDtos);

        var query = new GetCountriesQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();

        _countryRepositoryMock.Verify(repo => repo.FindAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mapperMock.Verify(mapper => mapper.Map<List<CountryDto>>(countries), Times.Once);
    }

    [Test]
    public void Handle_ShouldThrowException_WhenRepositoryThrowsException()
    {
        // Arrange
        _countryRepositoryMock
            .Setup(repo => repo.FindAllAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        var query = new GetCountriesQuery();

        // Act
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        act.Should().ThrowAsync<Exception>().WithMessage("Database error");

        _countryRepositoryMock.Verify(repo => repo.FindAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mapperMock.Verify(mapper => mapper.Map<List<CountryDto>>(It.IsAny<List<Country>>()), Times.Never);
    }
}