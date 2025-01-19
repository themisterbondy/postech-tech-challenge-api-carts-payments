using Postech.Fiap.CartsPayments.WebApi.Features.Carts.Contracts;
using Postech.Fiap.CartsPayments.WebApi.Features.Products.Entities;

namespace Postech.Fiap.CartsPayments.WebApi.Features.Carts.Services;

public interface ICartService
{
    Task<CartResponse> AddToCartAsync(Guid customerId, List<CartItemDto> cartItems);
    Task<CartResponse> GetCartByCustomerIdAsync(Guid customerId);
    Task<CartResponse> RemoveFromCartAsync(Guid customerId, Guid productId);
    Task<CartResponse> ClearCartAsync(Guid cartId);
}