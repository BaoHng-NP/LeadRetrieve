using LeadRetrieve.Controllers;
using LeadRetrieve.Models;
using LeadRetrieve.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq.Protected;
using Moq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using LeadRetrieve;
using System.Net;

public class LeadControllerTests
{
    private readonly Mock<ILogger<PageTokenService>> _mockLogger;
    private readonly HttpClient _mockHttpClient;
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly Mock<TestablePageTokenService> _mockPageTokenService;
    private readonly Mock<LeadAdContext> _mockContext;
    private readonly Mock<LeadRepository> _mockLeadRepository;
    private readonly Mock<LeadFieldDataRepository> _mockLeadFieldDataRepository;
    private LeadController _controller;

    public LeadControllerTests()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _mockHttpClient = new HttpClient(_mockHttpMessageHandler.Object);
        _mockLogger = new Mock<ILogger<PageTokenService>>();

        _mockPageTokenService = new Mock<TestablePageTokenService>(_mockHttpClient, _mockLogger.Object)
        {
            CallBase = true
        };

        _mockContext = new Mock<LeadAdContext>();
        _mockLeadRepository = new Mock<LeadRepository>(_mockContext.Object);
        _mockLeadFieldDataRepository = new Mock<LeadFieldDataRepository>(_mockContext.Object);

        _controller = new LeadController(_mockContext.Object, _mockPageTokenService.Object);
    }

    [Fact]
    public async Task FetchLeads_ReturnsOkResult()
    {
        // Arrange
        string mockToken = "mock_access_token";

        // Mock environment variables
        Environment.SetEnvironmentVariable("USER_ACCESS_TOKEN", "mock_user_access_token");
        Environment.SetEnvironmentVariable("APP_ID", "mock_app_id");
        Environment.SetEnvironmentVariable("APP_SECRET", "mock_app_secret");

        // Mock GetPageTokenAsync to return the mock token
        _mockPageTokenService.Setup(service => service.GetPageTokenAsync())
                             .ReturnsAsync(mockToken);

        var leadResponse = new LeadResponse
        {
            Data = new List<LeadData>
            {
                new LeadData
                {
                    Id = "123",
                    CreatedTime = DateTime.UtcNow.ToString(),
                    FieldData = new List<FieldData>
                    {
                        new FieldData
                        {
                            Name = "Name",
                            Values = new List<string> { "John Doe" }
                        }
                    }
                }
            },
            Paging = new PagingInfo
            {
                Cursors = new Cursors
                {
                    Before = "before",
                    After = "after"
                }
            }
        };

        // Mock response from Facebook Graph API
        var jsonResponse = JsonConvert.SerializeObject(leadResponse);
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse),
            });

        // Act
        var result = await _controller.FetchLeads();

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.NotNull(okResult);
        Assert.Equal("Leads fetched and stored successfully.", okResult.Value.GetType().GetProperty("Message")?.GetValue(okResult.Value));

        // Verify methods were called
        _mockLeadRepository.Verify(repo => repo.GetByLeadId("123"), Times.Never);
        _mockLeadRepository.Verify(repo => repo.AddLead(It.IsAny<Lead>()), Times.Once);
        _mockLeadFieldDataRepository.Verify(repo => repo.AddLeadFieldData(It.IsAny<Leadfielddata>()), Times.Once);
    }
}

public class TestablePageTokenService : PageTokenService
{
    public TestablePageTokenService(HttpClient httpClient, ILogger<PageTokenService> logger)
        : base(httpClient, logger)
    {
    }

    public virtual Task<string> GetPageTokenAsync()
    {
        return base.GetPageTokenAsync(); // Gọi phương thức gốc nếu cần
    }
}
