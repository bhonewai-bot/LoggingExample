using Serilog;

namespace SerilogExample2.WebApi.Services;

public class OrderService
{
    private readonly ILogger<OrderService> _logger;

    public OrderService(ILogger<OrderService> logger)
    {
        _logger = logger;
    }

    public void ProcessOrder(int orderId, string customerName)
    {
        _logger.LogInformation("Processing order {OrderId} for customer {CustomerName}", orderId, customerName);

        try
        {
            if (orderId > 1000)
            {
                _logger.LogWarning("Large order ID detected: {OrderID}", orderId);
            }
            
            _logger.LogInformation("Order {OrderId} processed successfully", orderId);
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex, "Failed to process order {OrderId}", orderId);
            throw;
        }
    }
    
    public void CancelOrder(int orderId, string reason)
    {
        Log.Warning("Cancelling order {OrderId}. Reason: {CancellationReason}", orderId, reason);
        
        Log.Information("Order {OrderId} cancelled successfully", orderId);
    }
}