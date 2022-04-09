using URLS.Infrastructure.Data.Context;

namespace URLS.Application.Seeder
{
    public class DUTSeederService : BaseSeederService
    {
        public DUTSeederService(URLSDbContext db) : base(db) { }

        public override async Task SeedSystemAsync()
        {
            await base.SeedSystemAsync();
        }
    }
}