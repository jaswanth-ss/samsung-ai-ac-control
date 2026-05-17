using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace backend;

public class DeviceStats
{
    public bool isOn {get;set;}
    public int temperature {get;set;}
    public string? mode {get;set;}
    public string? fanSpeed {get;set;}
    public int currentTemperature {get;set;}
}
public class DeviceControl
{
    private readonly ILogger<DeviceControl> _logger;

    public DeviceControl(ILogger<DeviceControl> logger)
    {
        _logger = logger;
    }

    [Function("DeviceControl")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult("Welcome to Azure Functions!");
    }

    [Function("DeviceStats")]
    public async Task<IActionResult> GetDeviceStats([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "device/stats")] HttpRequest req, FunctionContext context)
    {
        var logger = context.GetLogger("DeviceStats");
        logger.LogInformation("GetDeviceStats function was called.");

        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Environment.GetEnvironmentVariable("PAT"));

        var deviceId = Environment.GetEnvironmentVariable("DeviceID");
        var apiURL = $"https://api.smartthings.com/v1/devices/{deviceId}/status";
        
        logger.LogInformation("Sending request to SmartThings API.");
        try
        {
            var response = await client.GetAsync(apiURL);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                logger.LogInformation("Successfully retrieved device stats.");
                return new OkObjectResult(content);
            }
            else
            {
                logger.LogError("Failed to retrieve device stats. Status Code: {StatusCode}", response.StatusCode);
                return new ObjectResult(new { error = "Failed to retrieve device stats", statusCode = response.StatusCode }) { StatusCode = (int)response.StatusCode };
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while retrieving device stats.");
            return new ObjectResult(new { error = "Failed to retrieve device stats", details = ex.Message }) { StatusCode = 500 };
        }
    }

    // [Function("ControlDevice")]
    // public async Task<IActionResult> ControlDevice([HttpTrigger(AuthorizationLevel.Anonymous,"get", Route = "device/control")] HttpRequest req, FunctionContext context)
    // {
    //     var logger = context.GetLogger("ControlDevice");
    //     logger.LogInformation("ControlDevice function was called.");
        
    // }
}
