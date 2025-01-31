using System.ComponentModel.DataAnnotations;

namespace Postech.Fiap.CartsPayments.WebApi.Settings;

[ExcludeFromCodeCoverage]
public class MyFoodProductsHttpClientSettings
{
    public const string SettingsKey = "MyFoodProductsHttpClientSettings";
    public const string ClientName = "MyFoodProductsClient";

    [Required(ErrorMessage = "BaseUrl is required")]
    [Url(ErrorMessage = "BaseUrl must be a valid URL")]
    [MinLength(1, ErrorMessage = "BaseUrl must be at least 1 character")]
    public string BaseUrl { get; set; }
}