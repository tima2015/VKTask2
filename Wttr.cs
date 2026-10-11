using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VKTask2;

public class Wttr
{
    private static readonly HttpClient httpClient = new();

    private static readonly JsonSerializerOptions options = new()
    {
        PropertyNameCaseInsensitive = true,
        NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
    };

    public static async Task<WeatherData> GetWeatherDataAsync(string city)
    {
        string url = $"https://wttr.in/{Uri.EscapeDataString(city)}?format=j1";
        using var response = await httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        string jsonString = await response.Content.ReadAsStringAsync();
        WttrResponse wttrData = JsonSerializer.Deserialize<WttrResponse>(jsonString, options);

        if (wttrData?.CurrentCondition?.Count > 0 && wttrData.NearestArea?.Count > 0)
        {
            double tempC = double.Parse(wttrData.CurrentCondition[0].Temp_C, CultureInfo.InvariantCulture);
            string cityName = wttrData.NearestArea[0].AreaName?.FirstOrDefault()?.Value ?? city;
            string countryName = wttrData.NearestArea[0].Country?.FirstOrDefault()?.Value ?? "Unknown";

            return new WeatherData
            {
                City = cityName,
                Country = countryName,
                TemperatureC = tempC
            };
        }

        throw new Exception("Can't parse response!");
    }
}

public class WttrResponse
{
    [JsonPropertyName("current_condition")]
    public List<CurrentCondition> CurrentCondition { get; set; } = [];
    [JsonPropertyName("nearest_area")]
    public List<NearestArea> NearestArea { get; set; } = [];
}

public class CurrentCondition
{
    [JsonPropertyName("temp_C")]
    public string Temp_C { get; set; } = "0";
}

public class NearestArea
{
    [JsonPropertyName("areaName")]
    public List<AreaName> AreaName { get; set; } = [];
    [JsonPropertyName("country")]
    public List<Country> Country { get; set; } = [];
}

public class AreaName
{
    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;
}

public class Country
{
    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;
}

