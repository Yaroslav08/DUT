using DUT.Application.Services.Interfaces;
using DUT.Infrastructure.Data.Context;
namespace DUT.Application.Services.Implementations
{
    public class InitialService : IInitialService
    {
        private readonly DUTDbContext _db;
        public InitialService(DUTDbContext db)
        {
            _db = db;
        }

        public async Task InitSystemAsync()
        {

        }
    }
}