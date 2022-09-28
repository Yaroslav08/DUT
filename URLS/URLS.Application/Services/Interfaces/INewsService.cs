using URLS.Application.ViewModels;
using URLS.Application.ViewModels.News;

namespace URLS.Application.Services.Interfaces
{
    public interface INewsService
    {
        Task<Result<WarNewsResponse>> GetWarInfoAsync(DateTime? date = null);
        Task<Result<CovidNewsReponse>> GetCovidInfoAsync(DateTime? date = null);
    }
}