using Microsoft.EntityFrameworkCore;
using Postech.Fiap.CartsPayments.WebApi.Persistence;

namespace Postech.Fiap.CartsPayments.WebApi.UnitTests.Mocks;

public static class ApplicationDbContextMock
{
    public static ApplicationDbContext Create()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new ApplicationDbContext(options);

        return context;
    }
}