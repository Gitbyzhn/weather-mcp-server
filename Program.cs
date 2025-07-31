using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WeatherMcpServer.Tools;

var builder = Host.CreateApplicationBuilder(args);

// Configure all logs to go to stderr (stdout is used for the MCP protocol messages).
builder.Logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Trace);

// Add the MCP services: the transport to use (stdio) and the tools to register.
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<RandomNumberTools>()
    .WithTools<WeatherTools>();

// HttpClient + WeatherService
builder.Services.AddHttpClient<WeatherService>();

// Register WeatherTools для тестов
builder.Services.AddTransient<WeatherTools>();

var app = builder.Build();


// --------------- ТЕСТОВЫЙ БЛОК ---------------
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var weatherTools = services.GetRequiredService<WeatherTools>();

    var forecast = await weatherTools.GetWeatherForecast("London");
    var forecastData = System.Text.Json.JsonDocument.Parse(forecast);

    Console.WriteLine("3-Day Forecast:");
    foreach (var day in forecastData.RootElement.GetProperty("daily").EnumerateArray().Take(3))
    {
        var date = DateTimeOffset.FromUnixTimeSeconds(day.GetProperty("dt").GetInt64()).DateTime;
        var temp = day.GetProperty("temp").GetProperty("day").GetDouble();
        var desc = day.GetProperty("weather")[0].GetProperty("description").GetString();

        Console.WriteLine($"{date:yyyy-MM-dd}: {temp}°C, {desc}");
    }
}
// ---------------------------------------------


await app.RunAsync();
