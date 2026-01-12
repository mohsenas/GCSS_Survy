using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MyBuildingBlock.ChangeTracking.Data;
using MyBuildingBlock.ChangeTracking.Extensions;
using MyBuildingBlock.Configurations;
using MyBuildingBlock.Data;
using MyBuildingBlock.Ef;
using MyBuildingBlock.Extensions;
using MyBuildingBlock.Middleware;
using MyBuildingBlock.Security.Data;
using Serilog;
using Serilog.Context;
using GCSS_Survy;
using GCSS_Survy.Data;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Configuration.AddMyBuildingBlockConfiguration(builder.Environment);
bool isBehindProxy = configuration.GetValue<bool>("IsBehindProxy", false);
ConfigureSerilog(configuration);
builder.Host.UseSerilog();
// Configure Serilog
//ConfigureSerilog(configuration);
//builder.Host.UseSerilog();
// Add services to the container.
builder.Services.AddBuildingBlock<ApplicationDbContext>(typeof(Program).Assembly, configuration, "DefaultConnectionString", enableChangeTracking: false);
//builder.Services.AddDbContextWithInterceptors<ApplicationDbContext>(configuration, "LogConnectionString", enableChangeTracking: true);

//builder.Services.AddDbContext<LogDbContext>(options =>
//    options.UseSqlServer(configuration.GetConnectionString("LogConnectionString")),
//    ServiceLifetime.Scoped);

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "GCSS_Survy", Version = "v1" });
    c.ResolveConflictingActions(apiDescriptions =>
    {
        Console.WriteLine("❗ Conflict in Swagger route definitions:");
        foreach (var apiDesc in apiDescriptions)
            Console.WriteLine($" - {apiDesc.HttpMethod} {apiDesc.RelativePath}");
        return apiDescriptions.First();
    });
    c.UseAllOfToExtendReferenceSchemas();
    c.UseOneOfForPolymorphism();
    c.IgnoreObsoleteActions();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
Extensions.ModuleName = "GCSS_Survy";
//app.UseHttpsRedirection();

// Add exception handling middleware early in pipeline (before routing)
app.UseExceptionHandler("/error");
app.UseStatusCodePages();

// Add request logging middleware early
app.UseMiddleware<SerilogRequestLoggingMiddleware>();


await app.UseBuildingBlock<ApplicationDbContext>(configuration.GetConnectionString("DefaultConnectionString"));
// Add middleware to ensure all responses follow ReturnResult pattern
// Must be after UseAuthentication/UseAuthorization but before endpoints

//bool enableSwagger = app.Configuration.GetValue<bool>("EnableSwagger", true);
//if (enableSwagger)
//{
//    app.UseDeveloperExceptionPage();
//    app.UseSwagger();
//    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "GCSS_Survy API v1"));
//}

//var summaries = new[]
//{
//    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
//};

//app.MapGet("/weatherforecast", () =>
//{
//    var forecast = Enumerable.Range(1, 5).Select(index =>
//        new WeatherForecast
//        (
//            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
//            Random.Shared.Next(-20, 55),
//            summaries[Random.Shared.Next(summaries.Length)]
//        ))
//        .ToArray();
//    return forecast;
//});
//await RunDatabaseMigrations(app);

// Add error endpoint for exception handling
app.MapGet("/error", () =>
{
    var result = new ReturnResult<string>
    {
        Code = 500,
        Status = "Failed",
        Title = "Internal Server Error",
        Message = "An error occurred processing your request.",
        Data = "An error occurred processing your request."
    };
    return Results.Json(result, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }, null, 500);
});

// Health check endpoint
app.MapGet("/v1", () => "service is running.");

// Database health check endpoint
app.MapGet("/health/db", async (ApplicationDbContext dbContext) =>
{
    try
    {
        var canConnect = await dbContext.Database.CanConnectAsync();
        if (canConnect)
        {
            var userCount = await dbContext.Set<MyBuildingBlock.Models.Users>().CountAsync();
            var healthResponse = new HealthCheckResponse()
            {
                Status = "healthy",
                Database = "connected",
                UserCount = userCount,
                Timestamp = DateTime.UtcNow
            };
            var result = new ReturnResult<HealthCheckResponse>(healthResponse)
            {
                Code = 200,
                Status = "Success"
            };
            return Results.Json(result, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }, null, 200);
        }
        
        var errorResult = new ReturnResult<string>
        {
            Code = 500,
            Status = "Failed",
            Title = "Database Health Check Failed",
            Message = "Database connection failed",
            Data = "Database connection failed"
        };
        return Results.Json(errorResult, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }, null, 500);
    }
    catch (Exception ex)
    {
        var errorResult = new ReturnResult<string>
        {
            Code = 500,
            Status = "Failed",
            Title = "Database Health Check Failed",
            Message = $"Database health check failed: {ex.Message}",
            Data = $"Database health check failed: {ex.Message}"
        };
        return Results.Json(errorResult, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }, null, 500);
    }
});

app.Run();
 void ConfigureSerilog(IConfiguration configuration)
{
    var connLog = configuration.GetConnectionString("connStrLog");
    var applicationName = configuration.GetValue<string>("ApplicationName") ?? "DigitalArchiving";

    // SQL Server sink options for application logs
    

    

    Log.Logger = new LoggerConfiguration()
        // Minimum level: Information for debugging
        .MinimumLevel.Information()
        .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Warning)
        .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Information)

        // Enrich logs with contextual information
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Application", applicationName)

        // Console output (formatted for development)
        .WriteTo.Console(
            outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")

        // File output (daily rolling, structured)
        //.WriteTo.File(
        //    path: "Logs/app-.log",
        //    rollingInterval: RollingInterval.Day,
        //    retainedFileCountLimit: 30,
        //    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext}] {Message:lj} {Properties:j}{NewLine}{Exception}",
        //    fileSizeLimitBytes: 100_000_000, // 100 MB
        //    rollOnFileSizeLimit: true)

        // SQL Server output for application logs
        //.WriteTo.MSSqlServer(
        //    connectionString: connLog,
        //    sinkOptions: sqlSinkOptions,
        //    restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning)

        // Separate file for request logs with full details
        .WriteTo.Logger(lc => lc
            .Filter.ByIncludingOnly(e =>
                e.Properties.ContainsKey("RequestPath") ||
                e.Properties.ContainsKey("UserId"))
            //.WriteTo.File(
            //    path: "Logs/requests-.log",
            //    rollingInterval: RollingInterval.Day,
            //    retainedFileCountLimit: 30,
            //    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj} | User: {UserId} | Branch: {BranchId} | Path: {RequestPath} | Method: {RequestMethod} | IP: {IPAddress}{NewLine}{Exception}",
            //    fileSizeLimitBytes: 200_000_000, // 200 MB
            //    rollOnFileSizeLimit: true)
            )

        // SQL Server output for request logs
        .WriteTo.Logger(lc => lc
            .Filter.ByIncludingOnly(e =>
                e.Properties.ContainsKey("RequestPath") ||
                e.Properties.ContainsKey("UserId"))
            //.WriteTo.MSSqlServer(
            //    connectionString: connLog,
            //    sinkOptions: requestLogSinkOptions,
            //    restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information)
            )

        .CreateLogger();

    Log.Information("Serilog configured successfully for {Application}", applicationName);
}

