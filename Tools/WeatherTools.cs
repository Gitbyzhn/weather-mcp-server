using System.ComponentModel;
using ModelContextProtocol.Server;

namespace WeatherMcpServer.Tools;

public class WeatherTools
{
    private readonly WeatherService _weatherService;

    public WeatherTools(WeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    [McpServerTool]
    [Description("Gets current weather conditions for the specified city.")]
    public async Task<string> GetCurrentWeather(
        [Description("City name (e.g., 'London')")] string city,
        [Description("Optional country code (e.g., 'US', 'UK')")] string? countryCode = null)
    {
        return await _weatherService.GetCurrentWeatherAsync(city, countryCode);
    }

    [McpServerTool]
    [Description("Gets a 3-day weather forecast for the specified city.")]
    public async Task<string> GetWeatherForecast(
        [Description("City name (e.g., 'Paris')")] string city,
        [Description("Optional country code (e.g., 'FR')")] string? countryCode = null)
    {
        return await _weatherService.GetWeatherForecastAsync(city, countryCode);
    }

    [McpServerTool]
    [Description("Gets weather alerts or warnings for the specified city (bonus).")]
    public async Task<string> GetWeatherAlerts(
        [Description("City name (e.g., 'Tokyo')")] string city,
        [Description("Optional country code (e.g., 'JP')")] string? countryCode = null)
    {
        return await _weatherService.GetWeatherAlertsAsync(city, countryCode);
    }
}
