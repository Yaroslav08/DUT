using System.Text;
using System.Text.Json;

namespace DUT.Resources
{
    public class ResourceManager : IAsyncDisposable
    {
        private const string _dot = ".";
        private const string _name = "resources";
        private const string _extension = "json";
        private string _path;
        private Language _language;
        private Dictionary<string, string> _values;

        public ResourceManager(Language language)
        {
            _language = language;
            var lang = language.ToString().ToLower().Substring(0, 2);
            StringBuilder sb = new StringBuilder(_name);
            sb.Append(_dot);
            sb.Append(lang);
            sb.Append(_dot);
            sb.Append(_extension);
            _path = sb.ToString();

            _values = LoadValues();
        }

        private Dictionary<string, string> LoadValues()
        {
            try
            {
                if (File.Exists(_path))
                {
                    var content = File.ReadAllText(_path);
                    var res = JsonSerializer.Deserialize<Dictionary<string, string>>(content);
                    if (res == null)
                        res = new Dictionary<string, string>();
                    return res;
                }
                return new Dictionary<string, string>();
            }
            catch (Exception ex)
            {
                return new Dictionary<string, string>();
            }
        }

        public string this[string key] => _values[key];

        public string GetValue(string key) => _values[key];

        public string GetKey(string value)
        {
            var result = _values.FirstOrDefault(x => x.Value == value);
            if (result.Equals(default(KeyValuePair<string, string>)))
                return null;
            return result.Key;
        }

        public Language Language => _language;

        public async ValueTask DisposeAsync()
        {
            if (File.Exists(_path))
            {
                await File.AppendAllTextAsync(_path, JsonSerializer.Serialize(_values));
            }
            else
            {
                await File.Create(_path).DisposeAsync();
                await File.AppendAllTextAsync(_path, JsonSerializer.Serialize(_values));
            }
        }
    }

    public enum Language
    {
        Ukranian,
        English,
        Russian
    }
}