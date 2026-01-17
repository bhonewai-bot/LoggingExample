using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace SerilogExample2.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;

        public ProductController(ILogger<ProductController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetProducts()
        {
            _logger.LogInformation("GetProducts method called");

            var products = new List<string>() { "Product 1", "Product 2", "Product 3" };
            
            Log.Information("Retrieved {ProductCount} products", products.Count);
            
            return Ok(products);
        }

        [HttpPost]
        public IActionResult CreateProduct([FromBody] string productName)
        {
            _logger.LogInformation("Creating product: {ProductName}", productName);

            if (string.IsNullOrEmpty(productName))
            {
                Log.Warning("Empty product name provided");
                return BadRequest("Product name cannot be empty");
            }
            
            _logger.LogInformation("Product created successfully: {ProductName}", productName);
            
            return Ok($"Product {productName} created successfully");
        }

        [HttpGet("{id}")]
        public IActionResult GetProduct(int id)
        {
            try
            {
                _logger.LogInformation("Retrieving product with ID: {ProductId}", id);

                if (id <= 0)
                {
                    Log.Warning("Invalid product ID: {ProductId}", id);
                    return BadRequest("Invalid product ID");
                }

                var product = $"Product {id}";
                Log.Information("Product found: {ProductName}", product);

                return Ok(product);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving product with ID: {ProductId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
