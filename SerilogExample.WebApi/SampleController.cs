using Microsoft.AspNetCore.Mvc;
using Serilog;
using SerilogExample.WebApi.Services;

namespace SerilogExample.WebApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class SampleController : ControllerBase
    {
        private readonly ILogger<SampleController> _logger;
        private readonly ISampleService _sampleService;

        public SampleController(ILogger<SampleController> logger, ISampleService sampleService)
        {
            _logger = logger;
            _sampleService = sampleService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogInformation("SampleController: Get method called.");

            try
            {   
                throw new Exception("Test exception");
                _logger.LogDebug("Simulating work...");
                return Ok("Hello from SampleController!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the Get method.");
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}
