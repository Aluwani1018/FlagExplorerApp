using System.Net.Http.Json;
using System.Net;
using FlagExplorerApp.Application.CountryDetail;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace FlagExplorerApp.IntegrationTests.Controllers
{
    [TestFixture]
    public class CountryDetailControllerIntegrationTests
    {
        private WebApplicationFactory<Program> _factory;
        private HttpClient _client = null!; 

        [SetUp]
        public void SetUp()
        {
            _factory = new CustomWebApplicationFactory<Program>();
            _client = _factory.CreateClient();
        }

        //[TestCase("TestCountry", HttpStatusCode.OK, true)]
        //[TestCase("NonExistentCountry", HttpStatusCode.NotFound, false)]
        //[TestCase("", HttpStatusCode.BadRequest, false)]
        public async Task GetCountryDetails_ShouldReturnExpectedStatusCodeAndResponse(string countryName, HttpStatusCode expectedStatusCode, bool shouldReturnDetails)
        {
            // Act  
            var response = await _client.GetAsync($"/countries/{countryName}");

            // Assert  
            response.StatusCode.Should().Be(expectedStatusCode);

            if (shouldReturnDetails)
            {
                var countryDetail = await response.Content.ReadFromJsonAsync<CountryDetailDto>();
                countryDetail.Should().NotBeNull();
                countryDetail!.Name.Should().Be(countryName);
            }
        }

        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
        }
    }
}
