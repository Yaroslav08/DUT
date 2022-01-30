using DUT.Application.ViewModels.Location;
using DUT.Domain.Models;

namespace DUT.Application.Services.Interfaces
{
    public interface ILocationService
    {
        Task<Location> GetIpInfoAsync(string ip);
    }
}
