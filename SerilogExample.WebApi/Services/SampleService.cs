namespace SerilogExample.WebApi.Services;

public class SampleService : ISampleService
{
    private readonly ILogger<SampleService> _logger;

    public SampleService(ILogger<SampleService> logger)
    {
        _logger = logger;
    }

    public void DoWork()
    {
        _logger.LogInformation("SampleService: DoWork method called.");

        try
        {
            _logger.LogDebug("Doing some work...");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred in the DoWork method.");
            throw;
        }
    }
}

public interface ISampleService
{
    void DoWork();
}