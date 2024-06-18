using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
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

var connectResult = adService.Connect();

Console.WriteLine($"Connection {(connectResult.IsSuccess ? "SUCCEEDED!" : "FAILED")}");

var detailsResult = adService.GetServerDetails();
Console.WriteLine(JsonSerializer.Serialize(detailsResult.Value, serializerOptions));

adService.Disconnect();

#pragma warning disable CA1416

// Connect(adSettings);
// GetDomainDetails(adSettings);

// var ouCollection = GetOrganizationalUnits(adSettings);
// Console.WriteLine(JsonSerializer.Serialize(ouCollection, serializerOptions));

// var adPath = "LDAP://ec2-3-68-80-219.eu-central-1.compute.amazonaws.com/OU=TEST_QFR-Users,DC=Cert,DC=Local";
// var searchResults = SearchUser("gonzalez", adPath, adSettings);
// Console.WriteLine(JsonSerializer.Serialize(searchResults, serializerOptions));

void GetDomainDetails(ActiveDirectorySettings activeDirectorySettings)
{
    CancellationTokenSource cts = new CancellationTokenSource();
    Task task = Task.Run(() =>
    {
        try
        {
            var context = new DirectoryContext(
                DirectoryContextType.DirectoryServer,
                activeDirectorySettings.ServerName,
                activeDirectorySettings.UserName,
                activeDirectorySettings.Password
            );

            Console.WriteLine("Fetching Domain details..");
            var domain = Domain.GetDomain(context);
            Console.WriteLine("Domain Forest Name: " + domain.Forest.Name);
            foreach (DomainController dc in domain.DomainControllers)
            {
                Console.WriteLine("Domain Controller: " + dc.Name);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }
    }, cts.Token);

    if (!task.Wait(TimeSpan.FromSeconds(5)))
    {
        cts.Cancel();
        Console.WriteLine("Operation cancelled due to timeout.");
    }
}

List<string> SearchUser(string name, string adServicePath, ActiveDirectorySettings settings)
{
    var userList = new List<string>();
    try
    {
        DirectoryEntry entry = new DirectoryEntry(adServicePath, settings.UserName, settings.Password);

        DirectorySearcher searcher = new DirectorySearcher(entry)
        {
            Filter = $"(&(objectClass=user)(mail=*{name}*))"
        };

        // Optionally add more properties to load
        // searcher.PropertiesToLoad.Add("mail");
        // searcher.PropertiesToLoad.Add("sAMAccountName");
        // searcher.PropertiesToLoad.Add("userPrincipalName");
        // searcher.PropertiesToLoad.Add("cn");
        // searcher.PropertiesToLoad.Add("department");
        // searcher.PropertiesToLoad.Add("memberOf");

        SearchResultCollection results = searcher.FindAll();

        foreach (SearchResult result in results)
        {
            if (result.Properties.Contains("mail"))
            {
                string email = result.Properties["mail"][0].ToString()!;
                string accountName = (result.Properties.Contains("sAMAccountName")
                    ? result.Properties["sAMAccountName"][0].ToString()
                    : "N/A")!;
                userList.Add($"{accountName} ({email})");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("An error occurred while searching for users: " + ex.Message);
    }

    return userList;
}

static void CreateUserAccount()
{
    try
    {
        // Set the path to the OU (Organizational Unit) where you want to create the user.
        string ldapPath = "LDAP://OU=yourOU,DC=yourDomain,DC=com";
        string username = "newUsername";
        string password = "newPassword";

        // Connect to the Active Directory.
        using (DirectoryEntry ou = new DirectoryEntry(ldapPath))
        {
            // Create a new user.
            DirectoryEntry newUser = ou.Children.Add($"CN={username}", "user");

            // Set some properties.
            newUser.Properties["sAMAccountName"].Value = username;
            newUser.Properties["userAccountControl"].Value = 0x200; // NORMAL_ACCOUNT

            // Set the password.
            newUser.Invoke("SetPassword", new object[] { password });

            // Save the new user to the directory.
            newUser.CommitChanges();

            Console.WriteLine("User created successfully.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

static void DeleteUserAccount()
{
    // Set up domain context
    using (PrincipalContext ctx = new PrincipalContext(ContextType.Domain))
    {
        // Find the user you want to delete
        UserPrincipal user = UserPrincipal.FindByIdentity(ctx, "usernameToDelete");

        // If found, delete the user
        if (user != null)
        {
            user.Delete();
            Console.WriteLine("User deleted successfully.");
        }
        else
        {
            Console.WriteLine("User not found.");
        }
    }
}

static void ResetUserPassword()
{
    try
    {
        // The path to the user you want to reset the password for.
        string userDn = "LDAP://CN=UserName,OU=Users,DC=YourDomain,DC=com";
        string newPassword = "newPassword123";

        // Connect to the user's DirectoryEntry.
        using (DirectoryEntry user = new DirectoryEntry(userDn))
        {
            // Reset the password.
            user.Invoke("SetPassword", new object[] { newPassword });

            // If you want to force the user to change password at next logon, uncomment the next line.
            // user.Properties["pwdLastSet"].Value = 0;

            // Commit the changes.
            user.CommitChanges();

            Console.WriteLine("Password reset successfully.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}