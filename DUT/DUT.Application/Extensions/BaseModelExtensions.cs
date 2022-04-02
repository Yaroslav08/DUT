using DUT.Application.Services.Interfaces;
using DUT.Constants;
using DUT.Domain.Models;
namespace DUT.Application.Extensions
{
    public static class BaseModelExtensions
    {
        public static BaseModel PrepareToCreate(this BaseModel baseModel, IIdentityService identityService = null)
        {
            baseModel.LastUpdatedAt = null;
            baseModel.LastUpdatedBy = null;
            baseModel.LastUpdatedFromIP = null;
            baseModel.CreatedAt = DateTime.Now;
            baseModel.CreatedBy = identityService is null ? Defaults.CreatedBy : identityService.GetIdentityData();
            baseModel.CreatedFromIP = identityService is null ? Defaults.IP : identityService.GetIP();
            baseModel.Version = 1;
            return baseModel;
        }

        public static BaseModel PrepareToUpdate(this BaseModel baseModel, IIdentityService identityService = null)
        {
            baseModel.LastUpdatedAt = DateTime.Now;
            baseModel.LastUpdatedBy = identityService is null ? Defaults.CreatedBy : identityService.GetIdentityData();
            baseModel.LastUpdatedFromIP = identityService is null ? Defaults.IP : identityService.GetIP();
            baseModel.Version++;
            return baseModel;
        }
    }
}