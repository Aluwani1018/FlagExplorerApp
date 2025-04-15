using AutoMapper;
using FluentAssertions;
using FlagExplorerApp.Application.CountryDetails.GetCountryDetailByName;
using FlagExplorerApp.Domain.Repositories;
using Moq;
using NUnit.Framework;
using FlagExplorerApp.Application.CountryDetail;
using FlagExplorerApp.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace FlagExplorerApp.Tests.Handlers;

[TestFixture]
public class GetCountryDetailByNameQueryHandlerTests
{
    private Mock<ICountryDetailRepository> _countryDetailRepositoryMock;
    private Mock<IMapper> _mapperMock;
    private Mock<IMemoryCache> _memoryCacheMock;
    private GetCountryDetailByNameQueryHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _countryDetailRepositoryMock = new Mock<ICountryDetailRepository>();
        _mapperMock = new Mock<IMapper>();
        _memoryCacheMock = new Mock<IMemoryCache>();

        // Mocking CreateEntry for IMemoryCache
        var cacheEntryMock = new Mock<ICacheEntry>();
        _memoryCacheMock
            .Setup(x => x.CreateEntry(It.IsAny<object>()))
            .Returns(cacheEntryMock.Object);

        // Providing _memoryCacheMock in the handler
        _handler = new GetCountryDetailByNameQueryHandler(_mapperMock.Object, _countryDetailRepositoryMock.Object, _memoryCacheMock.Object);
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

        // Mocking cache hit
        object cachedValue = countryDetailDto;
        _memoryCacheMock
            .Setup(x => x.TryGetValue(It.IsAny<object>(), out cachedValue))
            .Returns(false); // Simulate a cache miss

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
        var countryDetail = new CountryDetail { Id = Guid.NewGuid().ToString(), Name = "TestCountry", Population = 1000, Capital = "TestCapital", Flag = "TestFlag" };

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
