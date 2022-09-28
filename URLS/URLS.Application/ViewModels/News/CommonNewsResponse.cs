using System.Text.Json.Serialization;

namespace URLS.Application.ViewModels.News
{
    public class CommonNewsResponse
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("long_title")]
        public string FullTitle { get; set; }
        [JsonPropertyName("url")]
        public string Url { get; set; }
        [JsonPropertyName("ref_url")]
        public string RefUrl { get; set; }
        [JsonPropertyName("data")]
        public List<CommonItemNewsResponse> Data { get; set; }

        public void Sort(DateTime? date = null, bool fromLastDate = true)
        {
            if (fromLastDate)
                SortDataByDesc();

            if (date != null)
            {
                var itemByDate = Data.FirstOrDefault(s => s.At.Date == date.Value.Date);
                Data = new List<CommonItemNewsResponse> { itemByDate };
            }
        }

        private void SortDataByDesc()
        {
            Data = Data.OrderByDescending(s => s.At).ToList();
        }
    }
}