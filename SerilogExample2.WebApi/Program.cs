using Serilog;
using SerilogExample2.WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt",
        rollingInterval: RollingInterval.Hour,
        fileSizeLimitBytes: 1024855760,
        retainedFileCountLimit: 24)
    .WriteTo.MSSqlServer(builder.Configuration.GetConnectionString("DbConnection"),
        tableName: "Logs",
        autoCreateSqlTable: true)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddScoped<OrderService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

try
{
    Log.Information("Application is starting...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application startup failed");
}
finally
{
    Log.CloseAndFlush();
}