using Postech.Fiap.CartsPayments.WebApi.Features.Products.Contracts;
using Postech.Fiap.CartsPayments.WebApi.Features.Products.Entities;

namespace Postech.Fiap.CartsPayments.WebApi.Features.Products.Services;

public interface IProductHttpClient
{
    Task<Result<ProductResponse>> FindByIdAsync(ProductId productId, CancellationToken cancellationToken);
}