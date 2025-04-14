using System.Net.Http.Json;
using System.Net;
using FlagExplorerApp.Application.CountryDetail;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using NUnit.Framework;

namespace FlagExplorerApp.IntegrationTests.Controllers
{
    [TestFixture]
    public class CountryDetailControllerIntegrationTests
    {
        private WebApplicationFactory<Program> _factory;
        private HttpClient _client;

        [SetUp]
        public void SetUp()
        {
            _factory = new CustomWebApplicationFactory<Program>();
            _client = _factory.CreateClient();
        }

        [Test]
        public async Task GetCountryDetails_ShouldReturnOkWithCountryDetails_WhenCountryExists()
        {
            // Arrange
            var countryName = "TestCountry";

            // Act
            var response = await _client.GetAsync($"/CountryDetail/{countryName}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var countryDetail = await response.Content.ReadFromJsonAsync<CountryDetailDto>();
            countryDetail.Should().NotBeNull();
            countryDetail!.Name.Should().Be(countryName);
        }

        [Test]
        public async Task GetCountryDetails_ShouldReturnNotFound_WhenCountryDoesNotExist()
        {
            // Arrange
            var countryName = "NonExistentCountry";

            // Act
            var response = await _client.GetAsync($"/CountryDetail/{countryName}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task GetCountryDetails_ShouldReturnBadRequest_WhenCountryNameIsEmpty()
        {
            // Arrange
            var countryName = string.Empty;

            // Act
            var response = await _client.GetAsync($"/CountryDetail/{countryName}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
            _factory.Dispose();
        }
    }
}
