namespace Postech.Fiap.CartsPayments.WebApi.Features.Products.Entities;

public class Product
{
    public ProductId Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public ProductCategory Category { get; set; }
    public string? ImageUrl { get; set; }
}