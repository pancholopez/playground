using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.AzureApp()
    .CreateBootstrapLogger();

Log.Information("Starting Application");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host
        .UseSerilog((context, loggerConfiguration) => loggerConfiguration
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
            .ReadFrom.Configuration(context.Configuration));

    var app = builder.Build();

    app.MapGet("/", () => "Hello World");

    if (app.Environment.IsDevelopment())
    {
    }

    app.Run();
}
catch (Exception exception) when (exception.GetType().Name is not "StopTheHostException" &&
                                  exception.GetType().Name is not "HostAbortedException")
{
    Log.Fatal(exception, "Unhandled exception");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}