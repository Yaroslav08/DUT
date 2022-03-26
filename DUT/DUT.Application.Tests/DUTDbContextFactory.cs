using DUT.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
namespace DUT.Application.Tests
{
    public class DUTDbContextFactory
    {
        public static DUTDbContext CreateDbContext(DbContextOptions<DUTDbContext> options = null)
        {
            options = options ?? new DbContextOptionsBuilder<DUTDbContext>()
                .UseInMemoryDatabase(databaseName: "DUTDatabase")
                .Options;
            return new DUTDbContext(options);
        }
    }
}