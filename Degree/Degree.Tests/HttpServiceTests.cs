using Degree.Models.DTO;
using Degree.Services;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Text.Json;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Degree.Tests
{
    [TestFixture]
    public class HttpServiceTests
    {
        private HttpClient _httpClient;
        private Mock<ILogger<HttpService>> _loggerMock;
        private Mock<IHttpClientFactory> _httpClientFactoryMock;
        private WireMockServer _mockServer;
        private HttpService _httpService;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<HttpService>>();
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _mockServer = WireMockServer.Start();

            var handler = new HttpClientHandler();
            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(_mockServer.Url) 
            };

            _httpClientFactoryMock.Setup(f => f.CreateClient("FakeStoreApiClient")).Returns(_httpClient);

            _httpService = new HttpService(_httpClientFactoryMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task GetCategoriesAsync_ReturnsCategories_WhenApiCallIsSuccessful()
        {
            // Arrange
            var categories = new List<string> { "Test1", "Test2", "Test3" };
            var jsonResponse = JsonSerializer.Serialize(categories);
            var expectedItemsCount = 3;

            _mockServer.Given(
                Request.Create().WithPath("/products/categories").UsingGet()
            )
            .RespondWith(
                Response.Create().WithBody(jsonResponse).WithHeader("Content-Type", "application/json")
            );

            // Act
            var result = await _httpService.GetCategoriesAsync(CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.EqualTo(true));
                Assert.That(result.Value.Count, Is.EqualTo(expectedItemsCount));
            });
        }

        [Test]
        public async Task GetCategoriesAsync_ReturnsFailure_WhenApiCallFails()
        {
            // Arrange
            var expectedErrorMessage = "Error: NotFound";

            _mockServer.Given(
                Request.Create().WithPath("/products/categories").UsingGet()
            )
            .RespondWith(
                Response.Create().WithStatusCode(404).WithHeader("Content-Type", "application/json")
            );

            // Act
            var result = await _httpService.GetCategoriesAsync(CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsFailure, Is.EqualTo(true));
                Assert.That(result.Error, Is.EqualTo(expectedErrorMessage));
            });
        }

        [Test]
        public async Task GetProductsAsync_ReturnsProducts_WhenApiCallIsSuccessful()
        {
            // Arrange
            var products = new List<ProductDto>
            {
                new() { Id = 1, Title = "Product 1", Price = 10 },
                new() { Id = 2, Title = "Product 2", Price = 20 }
            };
            var jsonResponse = JsonSerializer.Serialize(products);
            var expectedItemsCount = 2;

            _mockServer.Given(
                Request.Create().WithPath("/products").UsingGet()
            )
            .RespondWith(
                Response.Create().WithBody(jsonResponse).WithHeader("Content-Type", "application/json")
            );

            // Act
            var result = await _httpService.GetProductsAsync(CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.EqualTo(true));
                Assert.That(result.Value.Count(), Is.EqualTo(expectedItemsCount));
            });
        }

        [Test]
        public async Task GetProductsAsync_ReturnsFailure_WhenApiCallFails()
        {
            // Arrange
            var expectedErrorMessage = "Error: NotFound";

            _mockServer.Given(
                Request.Create().WithPath("/products").UsingGet()
            )
            .RespondWith(
                Response.Create().WithStatusCode(404).WithHeader("Content-Type", "application/json")
            );

            // Act
            var result = await _httpService.GetProductsAsync(CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsFailure, Is.EqualTo(true));
                Assert.That(result.Error, Is.EqualTo(expectedErrorMessage));
            });
        }

        [TearDown]
        public void TearDown()
        {
            _mockServer.Stop();
        }
    }
}
