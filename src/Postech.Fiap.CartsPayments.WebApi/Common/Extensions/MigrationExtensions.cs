using Microsoft.EntityFrameworkCore;
using Postech.Fiap.CartsPayments.WebApi.Persistence;

namespace Postech.Fiap.CartsPayments.WebApi.Common.Extensions;

[ExcludeFromCodeCoverage]
public static class MigrationExtensions
{
    public static void ApplyMigrations(this WebApplication app)
    {
        IServiceScope scope = null;

        try
        {
            scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();
            var pendingMigrations = dbContext.Database.GetPendingMigrations();

            if (pendingMigrations.Any())
            {
                logger.LogInformation("Migrações pendentes encontradas: {Migrations}",
                    string.Join(", ", pendingMigrations));
                dbContext.Database.Migrate();
                logger.LogInformation("Migrações aplicadas com sucesso.");
            }
            else
            {
                logger.LogInformation("Nenhuma migração pendente encontrada.");
            }
        }
        catch (Exception e)
        {
            var logger = scope?.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();
            logger?.LogError(e, "Ocorreu um erro ao aplicar migrações");
        }
        finally
        {
            scope?.Dispose();
        }
    }
}