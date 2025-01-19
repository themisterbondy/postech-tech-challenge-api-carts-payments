using Postech.Fiap.CartsPayments.WebApi.Features.Carts.Contracts;
using Postech.Fiap.CartsPayments.WebApi.Features.Products.Entities;

namespace Postech.Fiap.CartsPayments.WebApi.Features.Carts.Services;

public interface ICartService
{
    Task<CartResponse> AddToCartAsync(string? customerId, CartItemDto cartItem, Product product);
    Task<CartResponse> GetCartByCustomerIdAsync(string customerId);
    Task<CartResponse> RemoveFromCartAsync(string customerId, Guid productId);
    Task<CartResponse> ClearCartAsync(Guid cartId);
}