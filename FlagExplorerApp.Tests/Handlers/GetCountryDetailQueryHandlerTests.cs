using AutoMapper;
using FluentAssertions;
using FlagExplorerApp.Application.CountryDetails.GetCountryDetailByName;
using FlagExplorerApp.Domain.Repositories;
using Moq;
using NUnit.Framework;
using FlagExplorerApp.Application.CountryDetail;
using FlagExplorerApp.Domain.Entities;

namespace FlagExplorerApp.Tests.Handlers;

[TestFixture]
public class GetCountryDetailByNameQueryHandlerTests
{
    private Mock<ICountryDetailRepository> _countryDetailRepositoryMock;
    private Mock<IMapper> _mapperMock;
    private GetCountryDetailByNameQueryHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _countryDetailRepositoryMock = new Mock<ICountryDetailRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetCountryDetailByNameQueryHandler(_mapperMock.Object, _countryDetailRepositoryMock.Object);
    }

    [Test]
    public async Task Handle_ShouldReturnCountryDetailDto_WhenCountryExists()
    {
        // Arrange
        var query = new GetCountryDetailByNameQuery("TestCountry");
        var countryDetail = new CountryDetail { Id = Guid.NewGuid().ToString(), Name = "TestCountry", Population = 1000, Capital = "TestCapital", Flag = "TestFlag" };
        var countryDetailDto = new CountryDetailDto { Name = "TestCountry", Population = 1000, Capital = "TestCapital", Flag = "TestFlag" };

        _countryDetailRepositoryMock
            .Setup(repo => repo.FindByNameAsync(query.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(countryDetail);

        _mapperMock
            .Setup(mapper => mapper.Map<CountryDetailDto>(countryDetail))
            .Returns(countryDetailDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(countryDetailDto);
    }

    [Test]
    public async Task Handle_ShouldThrowKeyNotFoundException_WhenCountryDoesNotExist()
    {
        // Arrange
        var query = new GetCountryDetailByNameQuery("NonExistentCountry");

        _countryDetailRepositoryMock
            .Setup(repo => repo.FindByNameAsync(query.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CountryDetail)null);

        // Act
        var act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Country with name 'NonExistentCountry' was not found.");
    }

    [Test]
    public void Handle_ShouldThrowArgumentNullException_WhenQueryNameIsNull()
    {
        // Arrange
        var query = new GetCountryDetailByNameQuery(null!);

        // Act
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public void Handle_ShouldPropagateException_WhenRepositoryThrowsException()
    {
        // Arrange
        var query = new GetCountryDetailByNameQuery("TestCountry");

        _countryDetailRepositoryMock
            .Setup(repo => repo.FindByNameAsync(query.Name, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Repository error"));

        // Act
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        act.Should().ThrowAsync<Exception>().WithMessage("Repository error");
    }

    [Test]
    public void Handle_ShouldPropagateException_WhenMapperThrowsException()
    {
        // Arrange
        var query = new GetCountryDetailByNameQuery("TestCountry");
        var countryDetail = new CountryDetail { Id= Guid.NewGuid().ToString(), Name = "TestCountry", Population = 1000, Capital = "TestCapital", Flag = "TestFlag" };

        _countryDetailRepositoryMock
            .Setup(repo => repo.FindByNameAsync(query.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(countryDetail);

        _mapperMock
            .Setup(mapper => mapper.Map<CountryDetailDto>(countryDetail))
            .Throws(new Exception("Mapper error"));

        // Act
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        act.Should().ThrowAsync<Exception>().WithMessage("Mapper error");
    }
}
