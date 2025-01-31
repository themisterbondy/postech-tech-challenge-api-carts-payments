using System.Reflection;
using System.Text.Json.Serialization;
using Azure.Storage.Queues;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Postech.Fiap.CartsPayments.WebApi.Common;
using Postech.Fiap.CartsPayments.WebApi.Common.Behavior;
using Postech.Fiap.CartsPayments.WebApi.Common.Messaging;
using Postech.Fiap.CartsPayments.WebApi.Features.Carts.Jobs;
using Postech.Fiap.CartsPayments.WebApi.Features.Carts.Repositories;
using Postech.Fiap.CartsPayments.WebApi.Features.Carts.Services;
using Postech.Fiap.CartsPayments.WebApi.Features.Payments.Messaging.Queues;
using Postech.Fiap.CartsPayments.WebApi.Features.Payments.Services;
using Postech.Fiap.CartsPayments.WebApi.Features.Products.Services;
using Postech.Fiap.CartsPayments.WebApi.Infrastructure.Queue;
using Postech.Fiap.CartsPayments.WebApi.Persistence;
using Postech.Fiap.CartsPayments.WebApi.Settings;
using Quartz;
using Quartz.AspNetCore;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Filters;

namespace Postech.Fiap.CartsPayments.WebApi;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    private static readonly Assembly Assembly = typeof(Program).Assembly;

    public static IServiceCollection AddWebApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenApi();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddMediatRConfiguration();
        services.AddSwaggerConfiguration();
        services.AddOpenTelemetryConfiguration();

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddUseHealthChecksConfiguration(configuration);
        services.AddValidatorsFromAssembly(Assembly);

        services.AddProblemDetails();
        services.AddCarter();
        services.AddJsonOptions();

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("SQLConnection")));


        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IPaymentService, PaymentService>();

        services.AddProductsHttpClient(configuration);

        var storageConnectionString = configuration.GetValue<string>("AzureStorageSettings:ConnectionString");
        services.Configure<AzureQueueSettings>(configuration.GetSection("AzureQueueSettings"));
        services.AddScoped(cfg => cfg.GetService<IOptions<AzureQueueSettings>>().Value);
        services.AddSingleton(x => new QueueServiceClient(storageConnectionString));
        services.AddSingleton<IQueue, AzureQueueService>();
        services.AddSingleton<ICreateOrderCommandSubmittedQueueClient, CreateOrderCommandSubmittedQueueClient>();
        services.AddJobs();

        return services;
    }

    private static IServiceCollection AddJobs(this IServiceCollection services)
    {
        services.AddQuartzServer(options => { options.WaitForJobsToComplete = true; });
        services.AddQuartz(q =>
        {
            q.UseMicrosoftDependencyInjectionJobFactory();
            var cartJobKey = new JobKey("CartCleanupJob");
            q.AddJob<CartCleanupJob>(opts => opts.WithIdentity(cartJobKey));
            q.AddTrigger(opts => opts
                .ForJob(cartJobKey)
                .WithIdentity("CartCleanupJob-trigger")
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(5)
                    .RepeatForever())
                .StartAt(DateBuilder.FutureDate(5, IntervalUnit.Minute))
            );
        });

        return services;
    }

    private static void AddProductsHttpClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IProductHttpClient, ProductHttpClient>();

        services.AddOptions<MyFoodProductsHttpClientSettings>()
            .Bind(configuration.GetSection(MyFoodProductsHttpClientSettings.SettingsKey))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddHttpClient(MyFoodProductsHttpClientSettings.ClientName, (serviceProvider, httpClient) =>
        {
            var httpClientSettings =
                serviceProvider.GetRequiredService<IOptions<MyFoodProductsHttpClientSettings>>().Value;
            httpClient.BaseAddress = new Uri(httpClientSettings.BaseUrl);
            httpClient.Timeout = TimeSpan.FromMinutes(5);
        });
    }

    private static void AddMediatRConfiguration(this IServiceCollection services)
    {
        services.AddMediatR(config => config.RegisterServicesFromAssembly(Assembly));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
    }

    private static void AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Description = "PosTech MyFood API - Cart & Payments",
                Version = "v1",
                Title = "PosTech MyFood API - Cart & Payments"
            });
        });
    }

    private static void AddOpenTelemetryConfiguration(this IServiceCollection services)
    {
        var serviceName = $"{Assembly.GetName().Name}-{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}";
        services.AddOpenTelemetry()
            .ConfigureResource(resourceBuilder => resourceBuilder.AddService(serviceName!))
            .WithTracing(tracing =>
            {
                tracing.AddSource(serviceName!);
                tracing.ConfigureResource(resource => resource
                    .AddService(serviceName)
                    .AddTelemetrySdk());
                tracing.AddAspNetCoreInstrumentation();
                tracing.AddEntityFrameworkCoreInstrumentation();
                tracing.AddHttpClientInstrumentation();

                tracing.AddOtlpExporter();
            });
    }

    public static void AddSerilogConfiguration(this IServiceCollection services,
        WebApplicationBuilder builder, IConfiguration configuration)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        var applicationName =
            $"{Assembly.GetName().Name?.ToLower().Replace(".", "-")}-{environment?.ToLower().Replace(".", "-")}";

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("System", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("ApplicationName", applicationName)
            .Enrich.WithCorrelationId()
            .Enrich.WithExceptionDetails()
            .Filter.ByExcluding(Matching.FromSource("Microsoft.AspNetCore.StaticFiles"))
            .CreateLogger();

        builder.Logging.ClearProviders();
        builder.Host.UseSerilog(Log.Logger, true);
    }

    private static void AddJsonOptions(this IServiceCollection services)
    {
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        services.Configure<JsonOptions>(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
    }
}