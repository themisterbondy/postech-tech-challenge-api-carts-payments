using Postech.Fiap.CartsPayments.WebApi.Features.Carts.Contracts;
using Postech.Fiap.CartsPayments.WebApi.Features.Carts.Entities;
using Postech.Fiap.CartsPayments.WebApi.Features.Carts.Repositories;
using Postech.Fiap.CartsPayments.WebApi.Features.Products.Entities;

namespace Postech.Fiap.CartsPayments.WebApi.Features.Carts.Services;

public class CartService(ICartRepository cartRepository) : ICartService
{
    public async Task<CartResponse> AddToCartAsync(string? customerId, List<CartItemDto> cartItems)
    {
        var customer = customerId ?? Guid.NewGuid().ToString();
        var cart = await cartRepository.GetByCustomerIdAsync(customerId) ?? Cart.Create(CartId.New(), customer);

        foreach (var cartItem in cartItems)
        {
            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == new ProductId(cartItem.ProductId));
            if (existingItem != null)
            {
                existingItem.Quantity += cartItem.Quantity;
            }
            else
            {


                var newItem = CartItem.Create(
                    CartItemId.New(),
                    new ProductId(cartItem.ProductId),
                    cartItem.ProductName,
                    cartItem.UnitPrice,
                    cartItem.Quantity,
                    cartItem.Category);
                newItem.CartId = cart.Id;
                cart.AddItem(newItem);
            }
        }

        if (await cartRepository.ExistsAsync(cart.Id))
            await cartRepository.UpdateAsync(cart);
        else
            await cartRepository.AddAsync(cart);

        var totalAmount = cart.Items.Sum(item => item.UnitPrice * item.Quantity);

        return new CartResponse
        {
            CartId = cart.Id.Value,
            CustomerId = cart.CustomerId,
            TotalAmount = totalAmount,
            PaymentStatus = cart.PaymentStatus,
            Items = cart.Items.Select(i => new CartItemDto
            {
                ProductId = i.ProductId.Value,
                ProductName = i.ProductName,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity
            }).ToList()
        };
    }

    public async Task<CartResponse> GetCartByCustomerIdAsync(string customerId)
    {
        var cart = await cartRepository.GetByCustomerIdAsync(customerId);
        if (cart == null) return null;

        return new CartResponse
        {
            CartId = cart.Id.Value,
            CustomerId = cart.CustomerId,
            Items = cart.Items.Select(i => new CartItemDto
            {
                ProductId = i.ProductId.Value,
                ProductName = i.ProductName,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity
            }).ToList()
        };
    }

    public async Task<CartResponse> RemoveFromCartAsync(string customerId, Guid productId)
    {
        var cart = await cartRepository.GetByCustomerIdAsync(customerId);
        if (cart == null) return null;

        var item = cart.Items.Find(i => i.ProductId == new ProductId(productId));
        if (item != null)
        {
            cart.RemoveItem(item.Id);
            await cartRepository.UpdateAsync(cart);
        }

        return new CartResponse
        {
            CartId = cart.Id.Value,
            CustomerId = cart.CustomerId,
            Items = cart.Items.Select(i => new CartItemDto
            {
                ProductId = i.ProductId.Value,
                ProductName = i.ProductName,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity
            }).ToList()
        };
    }

    public async Task<CartResponse> ClearCartAsync(Guid cartId)
    {
        var cart = await cartRepository.GetByIdAsync(new CartId(cartId));
        if (cart == null) return null;

        cart.Items.Clear();
        await cartRepository.Delete(cart);

        return new CartResponse
        {
            CartId = cart.Id.Value,
            CustomerId = cart.CustomerId,
            Items = cart.Items.Select(i => new CartItemDto
            {
                ProductId = i.ProductId.Value,
                ProductName = i.ProductName,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity
            }).ToList()
        };
    }
}