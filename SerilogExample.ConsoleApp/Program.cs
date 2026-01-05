using Microsoft.AspNetCore.Builder;
using Serilog;
using Serilog.Sinks.MSSqlServer;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.MSSqlServer(
        connectionString: "Server=.;Database=DotNetTrainingBatch3Db;User ID=sa;Password=sasa@123;TrustServerCertificate=True;",
        sinkOptions: new MSSqlServerSinkOptions() { TableName = "Logs", AutoCreateSqlTable = true }
        )
    .CreateLogger();
    
Log.Information("Starting up");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();
    
    // throw new Exception("Test fatal exception");

    var app = builder.Build();
    app.MapGet("/", () => "Hello World!");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
    throw;
}
finally
{
    Log.CloseAndFlush();
}