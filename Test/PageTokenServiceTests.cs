using LeadRetrieve.Models;
using Microsoft.Extensions.Logging;
using Moq.Protected;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using LeadRetrieve;
namespace Test
{
    public class PageTokenServiceTests
    {
        private readonly Mock<ILogger<PageTokenService>> _mockLogger;
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly HttpClient _mockHttpClient;

        public PageTokenServiceTests()
        {
            _mockLogger = new Mock<ILogger<PageTokenService>>();

            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            _mockHttpClient = new HttpClient(_mockHttpMessageHandler.Object);
        }

        [Fact]
        public async Task GetPageTokenAsync_ReturnsToken_WhenValidResponse()
        {
            // Arrange
            var mockTokenResponse = new TokenResponse
            {
                AccessToken = "mock_page_token",
                ExpiresIn = 3600
            };

            var mockResponseContent = new StringContent(JsonConvert.SerializeObject(mockTokenResponse));

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = mockResponseContent
                });

            var pageTokenService = new PageTokenService(_mockHttpClient, _mockLogger.Object);

            Environment.SetEnvironmentVariable("USER_ACCESS_TOKEN", "user_access_token");
            Environment.SetEnvironmentVariable("APP_ID", "app_id");
            Environment.SetEnvironmentVariable("APP_SECRET", "app_secret");

            // Act
            var token = await pageTokenService.GetPageTokenAsync();

            // Assert
            token.Should().Be("mock_page_token");
        }

        [Fact]
        public async Task GetPageTokenAsync_ThrowsException_WhenResponseIsNotSuccessful()
        {
            // Arrange
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest
                });

            var pageTokenService = new PageTokenService(_mockHttpClient, _mockLogger.Object);

            Environment.SetEnvironmentVariable("USER_ACCESS_TOKEN", "user_access_token");
            Environment.SetEnvironmentVariable("APP_ID", "app_id");
            Environment.SetEnvironmentVariable("APP_SECRET", "app_secret");

            // Act
            Func<Task> action = async () => await pageTokenService.GetPageTokenAsync();

            // Assert
            await action.Should().ThrowAsync<Exception>()
                .WithMessage("Failed to retrieve page token.");
        }
    }
}
