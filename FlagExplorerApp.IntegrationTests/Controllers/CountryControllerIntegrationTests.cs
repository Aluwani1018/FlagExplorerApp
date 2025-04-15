
using System.Net.Http.Json;
using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using FlagExplorerApp.Application.Country;
using FluentAssertions;
using NUnit.Framework;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace FlagExplorerApp.IntegrationTests.Controllers
{
    public class CountryControllerIntegrationTests
    {
        private WebApplicationFactory<Program> _factory;
        private HttpClient _client;

        [SetUp]
        public void SetUp()
        {
            _factory = new CustomWebApplicationFactory<Program>();
            _client = _factory.CreateClient();
        }

        //[Test]
        public async Task GetAllCountries_ShouldReturnOkWithCountries()
        {
            // Act
            var response = await _client.GetAsync("/countries");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var countries = await response.Content.ReadFromJsonAsync<List<CountryDto>>();
            countries.Should().NotBeNull();
        }

        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
            _factory.Dispose();
        }
    }
}
