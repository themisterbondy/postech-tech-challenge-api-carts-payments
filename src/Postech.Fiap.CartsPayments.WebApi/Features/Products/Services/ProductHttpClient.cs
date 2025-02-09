using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Postech.Fiap.CartsPayments.WebApi.Features.Products.Contracts;
using Postech.Fiap.CartsPayments.WebApi.Features.Products.Entities;
using Postech.Fiap.CartsPayments.WebApi.Settings;

namespace Postech.Fiap.CartsPayments.WebApi.Features.Products.Services;

public class ProductHttpClient(
    IHttpClientFactory httpClientFactory,
    IOptions<MyFoodProductsHttpClientSettings> storageSettingsOptions,
    ILogger<ProductHttpClient> logger) : IProductHttpClient
{
    private readonly MyFoodProductsHttpClientSettings _dochubStorageHttpClientSettings = storageSettingsOptions.Value;

    public async Task<Result<ProductResponse>> FindByIdAsync(ProductId productId, CancellationToken cancellationToken)
    {
        try
        {
            var httpClient = httpClientFactory.CreateClient(MyFoodProductsHttpClientSettings.ClientName);
            var url = $"{_dochubStorageHttpClientSettings.BaseUrl}/products/{productId.Value}";

            var response = await httpClient.GetAsync(url, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync(cancellationToken);
                var product = JsonConvert.DeserializeObject<ProductResponse>(jsonResponse);
                return Result.Success(product);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                logger.LogWarning("Product with ID {ProductId} not found.", productId);
                return Result.Failure<ProductResponse>(Error.NotFound("ProductHttpClient.FindByIdAsync",
                    $"Product with ID {productId.Value} not found."));
            }

            // Tratar erros de status HTTP
            logger.LogError("Error finding product by id: {ProductId}. Status Code: {StatusCode}",
                productId, response.StatusCode);
            return Result.Failure<ProductResponse>(Error.Failure("ProductHttpClient.FindByIdAsync",
                $"Unexpected status code: {response.StatusCode}"));
        }
        catch (Exception e)
        {
            // Tratar erros gerais
            logger.LogError(e, "Error finding product by id: {ProductId}", productId);
            return Result.Failure<ProductResponse>(Error.Failure("ProductHttpClient.FindByIdAsync", e.Message));
        }
    }
}