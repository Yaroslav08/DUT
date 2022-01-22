using DUT.Application.ViewModels.Location;
using DUT.Domain.Models;

namespace DUT.Application.Services.Interfaces
{
    public interface ILocationService
    {
        Task<IPGeo> GetIpInfoAsync(string ip);
        Task<Location> GetDbInfoAsync(string ip);
    }
}
