using System.ComponentModel.DataAnnotations;

namespace Postech.Fiap.CartsPayments.WebApi.Settings;

public class MyFoodProductsHttpClientSettings
{
    internal static string SettingsKey = "MyFoodProductsHttpClientSettings";
    internal static string ClientName = "MyFoodProductsClient";

    [Required(ErrorMessage = "BaseUrl is required")]
    [Url(ErrorMessage = "BaseUrl must be a valid URL")]
    [MinLength(1, ErrorMessage = "BaseUrl must be at least 1 character")]
    public string BaseUrl { get; set; }
}