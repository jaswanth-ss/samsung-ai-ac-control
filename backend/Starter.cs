using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend;

public class Starter
{
    private readonly ILogger<Starter> _logger;

    public Starter(ILogger<Starter> logger)
    {
        _logger = logger;
    }

    [Function("Starter")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult("Welcome to Azure Functions!");
    }

    [Function("HealthCheck")]
    public async Task<IActionResult> HealthCheck([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route ="health")] HttpRequest req, FunctionContext context)
    {
        var logger = context.GetLogger("HealthCheck");
        logger.LogInformation("Health check was called.");
        return new OkObjectResult(new
        {
            status = "Healthy",
            timestamp = DateTime.UtcNow
        });
    }
}
