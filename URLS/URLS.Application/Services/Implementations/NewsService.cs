using System.Text.Json;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels;
using URLS.Application.ViewModels.News;

namespace URLS.Application.Services.Implementations
{
    public class NewsService : INewsService
    {
        private readonly HttpClient _httpClient;
        private readonly string[] _warTypes = new[] {
            "people",
            "tanks",
            "artilery",
            "auto",
            "bbm",
            "bpla",
            "helicopters",
            "planes",
            "ppo",
            "rszv",
            "ships",
        };

        private readonly string[] _covidTypes = new[]
        {
            "total-cases",
            "totla-deaths",
            "persons-vaccinated",
            "persons-fully-vaccinated",
            "persons-with-booster"
        };

        public NewsService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("UaData");
        }

        public async Task<Result<CovidNewsReponse>> GetCovidInfoAsync(DateTime? date = null)
        {
            try
            {
                var result = new CovidNewsReponse();
                result.Disease = new List<CommonNewsResponse>();


                foreach (var type in _covidTypes)
                {
                    var httpResponse = await _httpClient.GetAsync($"coronavirus-in-ukraine/{type}.json");

                    var stringJson = await httpResponse.Content.ReadAsStringAsync();

                    var item = JsonSerializer.Deserialize<CommonNewsResponse>(stringJson);

                    item.Sort(date);

                    result.Disease.Add(item);
                }

                return Result<CovidNewsReponse>.SuccessWithData(result);
            }
            catch (Exception ex)
            {
                return Result<CovidNewsReponse>.Exception(ex);
            }
        }

        public async Task<Result<WarNewsResponse>> GetWarInfoAsync(DateTime? date = null)
        {
            try
            {
                var result = new WarNewsResponse();
                result.Losses = new List<CommonNewsResponse>();


                foreach (var type in _warTypes)
                {
                    var httpResponse = await _httpClient.GetAsync($"ukraine-russia-war-2022/{type}.json");

                    var stringJson = await httpResponse.Content.ReadAsStringAsync();

                    var item = JsonSerializer.Deserialize<CommonNewsResponse>(stringJson);

                    item.Sort(date);

                    result.Losses.Add(item);
                }

                return Result<WarNewsResponse>.SuccessWithData(result);
            }
            catch (Exception ex)
            {
                return Result<WarNewsResponse>.Exception(ex);
            }
        }
    }
}