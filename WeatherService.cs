using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

public class WeatherService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public WeatherService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _apiKey = config["OPENWEATHER_API_KEY"] ?? throw new InvalidOperationException("Missing OPENWEATHER_API_KEY");
    }

    public async Task<string> GetCurrentWeatherAsync(string city, string? countryCode = null)
    {
        var (lat, lon) = await GetCoordinatesAsync(city, countryCode);
        var url = $"https://api.openweathermap.org/data/3.0/onecall?lat={lat}&lon={lon}&exclude=minutely,hourly,daily,alerts&units=metric&appid={_apiKey}";
        return await _httpClient.GetStringAsync(url);
    }

    public async Task<string> GetWeatherForecastAsync(string city, string? countryCode = null)
    {
        var (lat, lon) = await GetCoordinatesAsync(city, countryCode);
        var url = $"https://api.openweathermap.org/data/3.0/onecall?lat={lat}&lon={lon}&exclude=minutely,hourly,alerts&units=metric&appid={_apiKey}";
        return await _httpClient.GetStringAsync(url);
    }

    public async Task<string> GetWeatherAlertsAsync(string city, string? countryCode = null)
    {
        var (lat, lon) = await GetCoordinatesAsync(city, countryCode);
        var url = $"https://api.openweathermap.org/data/3.0/onecall?lat={lat}&lon={lon}&exclude=minutely,hourly,daily&units=metric&appid={_apiKey}";
        return await _httpClient.GetStringAsync(url);
    }

    private async Task<(double lat, double lon)> GetCoordinatesAsync(string city, string? countryCode)
    {
        var location = string.IsNullOrEmpty(countryCode) ? city : $"{city},{countryCode}";
        var geoUrl = $"https://api.openweathermap.org/geo/1.0/direct?q={location}&limit=1&appid={_apiKey}";
        var geoResponse = await _httpClient.GetFromJsonAsync<List<GeoResult>>(geoUrl);
        if (geoResponse == null || geoResponse.Count == 0)
            throw new Exception($"Coordinates not found for {location}");
        return (geoResponse[0].Lat, geoResponse[0].Lon);
    }

    private record GeoResult(double Lat, double Lon);
}
