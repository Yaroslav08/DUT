using DUT.Application.Extensions;
using DUT.Domain.Models;
using DUT.Infrastructure.Data.Context;

namespace DUT.Application.Seeder
{
    public class HostingSeederService : ISeederService
    {
        private readonly DUTDbContext _db;
        public HostingSeederService(DUTDbContext db)
        {
            _db = db;
        }

        public async Task SeedSystemAsync()
        {
            throw new NotImplementedException();
        }
    }
}
