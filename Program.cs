namespace VKTask2;

public class Program
{

    static async Task Main(string[] args)
    {
        string filePath = args.Length > 0 ? args[0] : "Cities.txt";
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"File not found: {filePath}");
            return;
        }

        var cities = File.ReadAllLines(filePath);
        HashSet<string> citySet = [.. cities.Select(c => c.Trim()).Where(c => !string.IsNullOrEmpty(c))];
        Console.WriteLine($"Load {citySet.Count} cities.");

        List<WeatherData> weatherDataList = [];

        foreach (var city in citySet)
        {
            try
            {
                Console.WriteLine($"Request data for: {city}...");
                WeatherData data = await Wttr.GetWeatherDataAsync(city);
                weatherDataList.Add(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error when receiving data for {city}: {ex.Message}");
            }
        }

        Console.WriteLine("\n--- City data ---");
        foreach (WeatherData data in weatherDataList)
        {
            Console.WriteLine(data);
        }

        Console.WriteLine("\n--- Country data ---");
        var groupedByCountry = weatherDataList
            .GroupBy(w => w.Country)
            .Select(g => new
            {
                Country = g.Key,
                CityCount = g.Count(),
                AverageTemp = g.Average(w => w.TemperatureC),
                MinTemp = g.Min(w => w.TemperatureC),
                MaxTemp = g.Max(w => w.TemperatureC)
            })
            .OrderBy(g => g.Country);

        foreach (var group in groupedByCountry)
        {
            Console.WriteLine(
                $"{group.Country} — {group.CityCount} cities, " +
                $"avg: {group.AverageTemp:+#;-#;0} °C, " +
                $"min: {group.MinTemp:+#;-#;0} °C, " +
                $"max: {group.MaxTemp:+#;-#;0} °C"
            );
        }
    }
}