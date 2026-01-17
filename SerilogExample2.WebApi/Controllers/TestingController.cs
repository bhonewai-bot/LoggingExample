using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SerilogExample2.WebApi.Services;

namespace SerilogExample2.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestingController : ControllerBase
    {
        private readonly ILogger<TestingController> _logger;

        public TestingController(ILogger<TestingController> logger)
        {
            _logger = logger;
        }

        [HttpGet("info")]
        public IActionResult TestInfoLog()
        {
            _logger.LogInformation("This is an information level log from ILogger");
            Log.Information("This is an information level log from static Log");
            return Ok("Information log test completed");
        }

        [HttpGet("warning")]
        public IActionResult TestWarningLog()
        {
            _logger.LogInformation("This is a warning level log from ILogger");
            Log.Warning("This is a warning level log from static Log");
            return Ok("Warning log test completed");
        }

        [HttpGet("error")]
        public IActionResult TestErrorLog()
        {
            try
            {
                throw new InvalidOperationException("This is a test exception for error logging");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "This is an error level log with exception from ILogger");
                Log.Error(ex, "This is an error level log with exception from static Log");
                throw;
            }
            
            return Ok("Error log test completed");
        }

        [HttpGet("mixed")]
        public IActionResult TestMixedLogging()
        {
            var orderService = new OrderService(
                LoggerFactory.Create(builder => builder.AddSerilog()).CreateLogger<OrderService>());
            
            orderService.ProcessOrder(1001, "John Doe");
            EmailService.SendEmail("test@gmail.com", "Test email");
            
            return Ok("Mixed logging completed");
        }
    }
}
