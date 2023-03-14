using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up");
try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Host
        .UseSerilog((context, configuration)
            => configuration
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .WriteTo.SQLite(
                    sqliteDbPath: @"Logs\audit.db",
                    tableName: "Logs",
                    restrictedToMinimumLevel: LogEventLevel.Information,
                    storeTimestampInUtc: true,
                    batchSize: 1)
                .WriteTo.SQLite(
                    sqliteDbPath: @"Logs\logs.db",
                    tableName: "Logs",
                    restrictedToMinimumLevel: LogEventLevel.Warning,
                    storeTimestampInUtc: true)
                .Enrich.WithEnvironmentName());

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseSerilogRequestLogging();

    app.UseHttpsRedirection();

    app.MapGet("/alive", (ILogger<Program> logger) =>
        {
            logger.LogInformation("Some info here!");
            logger.LogDebug("This is a debug message");
            logger.LogTrace("This is a trace log");
            logger.LogWarning("This is a warning!");
            logger.LogError("This is an error message");
            logger.LogError(new Exception("BOOM!"), "This is an exception!!!");
            logger.LogCritical("This is a critical error!");
            return "I am alive!";
        })
        .WithName("alive")
        .WithOpenApi();

    app.Run();
}
catch (Exception ex) when (ex.GetType().Name is not "StopTheHostException")
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}