using Postech.Fiap.CartsPayments.WebApi;
using Postech.Fiap.CartsPayments.WebApi.Common;
using Postech.Fiap.CartsPayments.WebApi.Common.Middleware;
using Postech.Fiap.CartsPayments.WebApi.Settings;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var configuration = AppSettings.Configuration();
builder.Services.AddWebApi(configuration);
builder.Services.AddSerilogConfiguration(builder, configuration);


var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.MapOpenApi();
app.UseHttpsRedirection();
app.UseHealthChecksConfiguration();
app.UseExceptionHandler();
app.UseSerilogRequestLogging();
app.UseMiddleware<RequestContextLoggingMiddleware>();
app.MapCarter();
app.Run();

namespace Postech.Fiap.CartsPayments.WebApi
{
    [ExcludeFromCodeCoverage]
    public partial class Program;
}