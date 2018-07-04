using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Frontend.Tests
{
    public class IntegrationTest
    {
        private readonly TestServer _factoryFrontend;

        public IntegrationTest()
        {
            _factoryFrontend = new TestServer(BuildFrontend());
        }

        private IWebHostBuilder BuildFrontend()
        {
            // Create TestServer for every dependency
            var _factoryAPI = new TestServer(new WebHostBuilder().UseStartup<API.Startup>());

            // Use the same name configured in your app and add its test server
            var mockedHttpClientFactory = new MockedHttpClientFactory().AddClient("APIClient", _factoryAPI);

            // Build the Startup using ConfigureTestServices
            return new WebHostBuilder()
                .UseStartup<Startup>().ConfigureTestServices(services =>
                {
                    services.AddSingleton<IHttpClientFactory>(mockedHttpClientFactory);
                });
        }

        [Theory]
        [InlineData("/")]
        [InlineData("/Home/Index")]
        [InlineData("/Home/About")]
        [InlineData("/Home/Privacy")]
        [InlineData("/Home/Contact")]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            var client = _factoryFrontend.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }
    }
}
