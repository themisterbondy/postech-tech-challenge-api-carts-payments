using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NSubstitute.ExceptionExtensions;
using Postech.Fiap.CartsPayments.WebApi.Features.Products.Contracts;
using Postech.Fiap.CartsPayments.WebApi.Features.Products.Entities;
using Postech.Fiap.CartsPayments.WebApi.Features.Products.Services;
using Postech.Fiap.CartsPayments.WebApi.Settings;

namespace Postech.Fiap.CartsPayments.WebApi.UnitTests.Features.Products.Services;

public class ProductHttpClientTests
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ProductHttpClient> _logger;
    private readonly ProductHttpClient _productHttpClient;
    private readonly IOptions<MyFoodProductsHttpClientSettings> _settings;

    public ProductHttpClientTests()
    {
        _httpClientFactory = Substitute.For<IHttpClientFactory>();
        _logger = Substitute.For<ILogger<ProductHttpClient>>();

        _settings = Options.Create(new MyFoodProductsHttpClientSettings
        {
            BaseUrl = "https://api.example.com"
        });

        _productHttpClient = new ProductHttpClient(_httpClientFactory, _settings, _logger);
    }

    [Fact]
    public async Task FindByIdAsync_Should_Return_ProductResponse_When_Successful()
    {
        // Arrange
        var productId = ProductId.New();
        var productResponse = new ProductResponse
        {
            Id = productId.Value,
            Name = "Pizza",
            Description = "Delicious pizza",
            Price = 25.99m,
            Category = ProductCategory.Lanche,
            ImageUrl = "https://example.com/pizza.jpg"
        };

        var jsonResponse = JsonConvert.SerializeObject(productResponse);
        var httpResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(jsonResponse)
        };

        var httpClient = new HttpClient(new FakeHttpMessageHandler(httpResponse))
        {
            BaseAddress = new Uri(_settings.Value.BaseUrl)
        };

        _httpClientFactory.CreateClient(MyFoodProductsHttpClientSettings.ClientName).Returns(httpClient);

        // Act
        var result = await _productHttpClient.FindByIdAsync(productId, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(productResponse);
    }

    [Fact]
    public async Task FindByIdAsync_Should_Return_NotFound_When_Product_Does_Not_Exist()
    {
        // Arrange
        var productId = ProductId.New();
        var httpResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.NotFound
        };

        var httpClient = new HttpClient(new FakeHttpMessageHandler(httpResponse))
        {
            BaseAddress = new Uri(_settings.Value.BaseUrl)
        };

        _httpClientFactory.CreateClient(MyFoodProductsHttpClientSettings.ClientName).Returns(httpClient);

        // Act
        var result = await _productHttpClient.FindByIdAsync(productId, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("ProductHttpClient.FindByIdAsync");
        result.Error.Message.Should().Contain("not found");
    }

    [Fact]
    public async Task FindByIdAsync_Should_Return_Error_When_Api_Returns_Unexpected_Status()
    {
        // Arrange
        var productId = ProductId.New();
        var httpResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.InternalServerError
        };

        var httpClient = new HttpClient(new FakeHttpMessageHandler(httpResponse))
        {
            BaseAddress = new Uri(_settings.Value.BaseUrl)
        };

        _httpClientFactory.CreateClient(MyFoodProductsHttpClientSettings.ClientName).Returns(httpClient);

        // Act
        var result = await _productHttpClient.FindByIdAsync(productId, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("ProductHttpClient.FindByIdAsync");
        result.Error.Message.Should().Contain("Unexpected status code");
    }

    [Fact]
    public async Task FindByIdAsync_Should_Return_Error_When_Exception_Is_Thrown()
    {
        // Arrange
        var productId = ProductId.New();
        _httpClientFactory.CreateClient(MyFoodProductsHttpClientSettings.ClientName)
            .Throws(new HttpRequestException("Network failure"));

        // Act
        var result = await _productHttpClient.FindByIdAsync(productId, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("ProductHttpClient.FindByIdAsync");
        result.Error.Message.Should().Contain("Network failure");
    }
}

// Fake HttpMessageHandler to mock HTTP responses
public class FakeHttpMessageHandler : HttpMessageHandler
{
    private readonly HttpResponseMessage _response;

    public FakeHttpMessageHandler(HttpResponseMessage response)
    {
        _response = response;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(_response);
    }
}