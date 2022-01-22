using DUT.Application.Services.Interfaces;
using DUT.Application.ViewModels.Location;
using DUT.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DUT.Application.Services.Implementations
{
    public class LocationService : ILocationService
    {
        public async Task<Location> GetDbInfoAsync(string ip)
        {
            var ipInfo = await GetIpInfoAsync(ip);
            if (ipInfo == null)
                return null;
            return new Location
            {
                City = ipInfo.City,
                Country = ipInfo.Country,
                IP = ipInfo.Query,
                Lat = ipInfo.Latitude,
                Lon = ipInfo.Longitude,
                Region = ipInfo.Region
            };
        }

        public async Task<IPGeo> GetIpInfoAsync(string ip)
        {
            using var httpClient = new HttpClient();
            var urlRequest = "http://ip-api.com/json";
            if (string.IsNullOrEmpty(ip))
            {
                urlRequest = urlRequest + "?fields=63700991";
            }
            else
            {
                if (ip.Contains("::1") || ip.Contains("localhost"))
                    urlRequest = urlRequest + "?fields=63700991";
                else
                    urlRequest = urlRequest + $"/{ip}?fields=63700991";
            }
            var resultFromApi = await httpClient.GetAsync(urlRequest);
            if (!resultFromApi.IsSuccessStatusCode)
                return null;
            var content = await resultFromApi.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IPGeo>(content);
        }
    }
}
