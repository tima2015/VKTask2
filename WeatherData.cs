namespace VKTask2;

public class WeatherData
{
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public double TemperatureC { get; set; }

    public override string ToString()
    {
        return $"{City}, {Country} {TemperatureC:+#;-#;0} °C";
    }
}
