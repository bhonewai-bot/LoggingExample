namespace SerilogExample.WebApi.Repository;

public interface ISampleRepository
{
    void SaveData();
}

public class SampleRepository : ISampleRepository
{
    private readonly ILogger<SampleRepository> _logger;

    public SampleRepository(ILogger<SampleRepository> logger)
    {
        _logger = logger;
    }

    public void SaveData()
    {
        _logger.LogInformation("SampleRepository: SaveData method called.");
        
        try
        {
            _logger.LogDebug("Saving data to the database...");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured in the SaveData method.");
            throw;
        }
    }
}