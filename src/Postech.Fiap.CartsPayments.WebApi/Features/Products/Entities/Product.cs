namespace Postech.Fiap.CartsPayments.WebApi.Features.Products.Entities;

public class Product
{
    private Product(ProductId id, string name, string description, decimal price, ProductCategory category,
        string? imageUrl)
    {
        Id = id;
        Name = name;
        Description = description;
        Price = price;
        Category = category;
        ImageUrl = imageUrl;
    }

    private Product()
    {
    }

    public ProductId Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public ProductCategory Category { get; set; }
    public string? ImageUrl { get; set; }

    public static Product? Create(ProductId id, string name, string description, decimal price,
        ProductCategory category,
        string? imageUrl)
    {
        return new Product(id, name, description, price, category, imageUrl);
    }
}