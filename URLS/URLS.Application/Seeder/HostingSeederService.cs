using URLS.Application.Extensions;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Context;

namespace URLS.Application.Seeder
{
    public class HostingSeederService : ISeederService
    {
        private readonly URLSDbContext _db;
        public HostingSeederService(URLSDbContext db)
        {
            _db = db;
        }

        public async Task SeedSystemAsync()
        {
            throw new NotImplementedException();
        }
    }
}
