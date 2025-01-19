using Microsoft.EntityFrameworkCore;
using Postech.Fiap.CartsPayments.WebApi.Features.Products.Entities;
using Postech.Fiap.CartsPayments.WebApi.Persistence;

namespace Postech.Fiap.CartsPayments.WebApi.Features.Products.Repositories;

public class ProductRepository(ApplicationDbContext context) : IProductRepository
{
    public async Task<Product?> FindByIdAsync(ProductId id, CancellationToken cancellationToken)
    {
        return await context.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<List<Product?>> FindByCategoryAsync(ProductCategory? category, CancellationToken cancellationToken)
    {
        var query = context.Products.AsQueryable();

        if (category.HasValue) query = query.Where(x => x.Category == category);

        return query.ToListAsync(cancellationToken);
    }

    public async Task<Product?> CreateAsync(Product? product, CancellationToken cancellationToken)
    {
        await context.Products.AddAsync(product, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return product;
    }

    public async Task<Product?> UpdateAsync(Product? product, CancellationToken cancellationToken)
    {
        context.Products.Update(product);
        await context.SaveChangesAsync(cancellationToken);
        return product;
    }

    public Task DeleteAsync(Product? product, CancellationToken cancellationToken)
    {
        context.Products.Remove(product);
        return context.SaveChangesAsync(cancellationToken);
    }
}