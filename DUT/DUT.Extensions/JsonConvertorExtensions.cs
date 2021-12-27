using System.Text.Json;

namespace DUT.Extensions
{
    public static class JsonConvertorExtensions
    {
        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            
        };
        public static string ToJson(this object data)
        {
            return JsonSerializer.Serialize(data, _options);
        }
        public static T FromJson<T>(this string json)
        {
            return JsonSerializer.Deserialize<T>(json, _options);
        }
    }
}
