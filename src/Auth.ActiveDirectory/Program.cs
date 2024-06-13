using Auth.ActiveDirectory;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddUserSecrets<Program>()
    .Build();

var adSettings = configuration.GetSection("ActiveDirectory").Get<ActiveDirectory>();

Console.WriteLine($"Hello, World! {adSettings!.Address}, {adSettings.User}, {adSettings.Password}");