using Microsoft.EntityFrameworkCore;
using Postech.Fiap.CartsPayments.WebApi.Features.Carts.Entities;
using Postech.Fiap.CartsPayments.WebApi.Features.Payments.Emun;
using Postech.Fiap.CartsPayments.WebApi.Persistence;

namespace Postech.Fiap.CartsPayments.WebApi.Features.Carts.Repositories;

public class CartRepository(ApplicationDbContext context) : ICartRepository
{
    public async Task<Cart?> GetByCustomerIdAsync(Guid customerId)
    {
        return await context.Carts
            .AsNoTracking()
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.CustomerId == customerId);
    }

    public async Task<Cart?> GetByIdAsync(CartId cartId)
    {
        return await context.Carts
            .AsNoTracking()
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == cartId);
    }

    public async Task<bool> ExistsAsync(CartId cartId)
    {
        return await context.Carts
            .AsNoTracking()
            .Include(c => c.Items)
            .AnyAsync(c => c.Id == cartId);
    }

    public async Task AddAsync(Cart cart)
    {
        await context.Carts.AddAsync(cart);
        await context.SaveChangesAsync();
    }

    public async Task UpdateStatusAsync(Cart cart)
    {
        await context.Carts.Where(c => c.Id == cart.Id)
            .ExecuteUpdateAsync(
                c => c
                    .SetProperty(p => p.PaymentStatus, cart.PaymentStatus)
                    .SetProperty(p => p.TransactionId, cart.TransactionId)
            );

    }

    public async Task UpdateAsync(Cart cart)
    {
        var cartInContext = await context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == cart.Id);

        if (cartInContext == null)
        {
            // Se o carrinho não estiver sendo rastreado, anexe-o e marque como modificado
            context.Attach(cart);
            context.Entry(cart).State = EntityState.Modified;
        }
        else
        {
            // Se o carrinho já está sendo rastreado, atualize apenas os itens
            foreach (var item in cart.Items)
            {
                var existingItem = cartInContext.Items.FirstOrDefault(i => i.Id == item.Id);
                if (existingItem != null)
                {
                    // Se o item já existe, atualize-o
                    context.Entry(existingItem).CurrentValues.SetValues(item);
                    context.Entry(existingItem).State = EntityState.Modified;
                }
                else
                {
                    // Se o item é novo, adicione-o
                    cartInContext.Items.Add(item);
                    context.Entry(item).State = EntityState.Added;
                }
            }
        }

        await context.SaveChangesAsync();
    }

    public async Task DeleteUnpaidCartsOlderThanAsync(DateTime threshold)
    {
        var cartsToDelete = await context.Carts
            .Where(c => c.CreatedAt < threshold
                        && (c.PaymentStatus == PaymentStatus.NotStarted || c.PaymentStatus == PaymentStatus.Pending))
            .ToListAsync();

        context.Carts.RemoveRange(cartsToDelete);
        await context.SaveChangesAsync();
    }

    public Task<Cart?> GetByTransactionIdAsync(string transactionId)
    {
        return context.Carts
            .AsNoTracking()
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.TransactionId == transactionId);
    }

    public Task Delete(Cart cart)
    {
        context.Carts.Remove(cart);
        return context.SaveChangesAsync();
    }
}