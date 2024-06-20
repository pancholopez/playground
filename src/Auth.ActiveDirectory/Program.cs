using System.DirectoryServices;
using System.Text.Json;
using Auth.ActiveDirectory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// configuration setup
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddUserSecrets<Program>()
    .Build();

var adSettings = configuration.GetSection("ActiveDirectory").Get<ActiveDirectorySettings>()!;

// setup DI Container and build service provider
var serviceProvider = new ServiceCollection()
    .AddLogging(options =>
    {
        options.AddConfiguration(configuration.GetSection("Logging"));
        options.AddConsole();
    })
    .AddSingleton(adSettings)
    .AddTransient<IActiveDirectoryService, ActiveDirectoryService>()
    .BuildServiceProvider();

var serializerOptions = new JsonSerializerOptions { WriteIndented = true };

var adService = serviceProvider.GetRequiredService<IActiveDirectoryService>();

var connectResult = adService.ValidateConnection(adSettings);

Console.WriteLine($"Connection {(connectResult.IsSuccess ? "SUCCEEDED!" : "FAILED")}");

var detailsResult = adService.GetOrganizationalUnits(adSettings);
var testOU = detailsResult.Value.Single(x => x.Name.Contains("TEST_QFR", StringComparison.OrdinalIgnoreCase));
Console.WriteLine(JsonSerializer.Serialize(testOU, serializerOptions));

// var searchResult = adService.SearchUserAccount("gonzalez", testOU.ActiveDirectoryServicePath, adSettings);
// Console.WriteLine(JsonSerializer.Serialize(searchResult, serializerOptions));

var newAccount = new NewUserAccount(
    UserName: "JDoe",
    Email: "jdoe@example.com",
    FirstName: "John",
    LastName: "Doe");

var createResult = adService.CreateAccount(testOU.ActiveDirectoryServicePath, newAccount, adSettings);
Console.WriteLine(JsonSerializer.Serialize(createResult, serializerOptions));
var account = createResult.Value;

// var passwordResult = adService.ResetPassword(account, "aP4@zB1", adSettings);
// Console.WriteLine(
//     $"Password reset for {account.SecurityAccountManagerName} {(passwordResult.IsSuccess ? 
//         "Succeeded!" : $"FAILED. {passwordResult.ErrorMessage}")}");

var groupResult = adService.AddUserToGroup(account.SecurityAccountManagerName, "TestGroup", adSettings);
if (groupResult.IsFailure)
{
    Console.WriteLine($"Failed to add user to group. {groupResult.ErrorMessage}");
}

var searchResult = adService.SearchUserAccount("jdoe", testOU.ActiveDirectoryServicePath, adSettings);
Console.WriteLine(JsonSerializer.Serialize(searchResult, serializerOptions));

var samAccountName = newAccount.UserName;
var deleteResult = adService.DeleteAccount(samAccountName, adSettings);
Console.WriteLine(
    $"Account deletion for {samAccountName} {(deleteResult.IsSuccess ? "Succeeded!" : $"FAILED: {deleteResult.ErrorMessage}")}");