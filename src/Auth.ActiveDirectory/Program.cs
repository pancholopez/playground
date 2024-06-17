using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using System.Text.Json;
using Auth.ActiveDirectory;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddUserSecrets<Program>()
    .Build();

var adSettings = configuration.GetSection("ActiveDirectory").Get<ActiveDirectorySettings>()!;
var serializerOptions = new JsonSerializerOptions { WriteIndented = true };

#pragma warning disable CA1416

// Connect(adSettings);
// GetDomainDetails(adSettings);

// var ouCollection = GetOrganizationalUnits(adSettings);
// Console.WriteLine(JsonSerializer.Serialize(ouCollection, serializerOptions));

var adPath = "LDAP://ec2-3-68-80-219.eu-central-1.compute.amazonaws.com/OU=TEST_QFR-Users,DC=Cert,DC=Local";
var searchResults = SearchUser("gonzalez", adPath, adSettings);
Console.WriteLine(JsonSerializer.Serialize(searchResults, serializerOptions));

void Connect(ActiveDirectorySettings settings)
{
    using (var context = new PrincipalContext(
               contextType: ContextType.Domain,
               name: settings!.Address,
               userName: settings.User,
               password: settings.Password))
    {
        if (context.ConnectedServer is not null)
        {
            Console.WriteLine($"Connected to active directory: {JsonSerializer
                .Serialize(context, serializerOptions)}");
            // here list all Domain Controllers
        }
        else
        {
            Console.WriteLine("Failed to connect to Active Directory.");
        }
    }
}

void GetDomainDetails(ActiveDirectorySettings activeDirectorySettings)
{
    try
    {
        var context = new DirectoryContext(
            DirectoryContextType.DirectoryServer,
            activeDirectorySettings.Address,
            activeDirectorySettings.User,
            activeDirectorySettings.Password
        );

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
}

static List<OrganizationalUnit> GetOrganizationalUnits(ActiveDirectorySettings settings)
{
    var organizationalUnits = new List<OrganizationalUnit>();
    try
    {
        string ldapPath = $"LDAP://{settings.Address}";
        DirectoryEntry entry = new DirectoryEntry(ldapPath, settings.User, settings.Password);

        DirectorySearcher searcher = new DirectorySearcher(entry)
        {
            Filter = "(objectClass=organizationalUnit)"
        };

        searcher.PropertiesToLoad.Add("ou");
        searcher.PropertiesToLoad.Add("distinguishedName");

        SearchResultCollection results = searcher.FindAll();

        foreach (SearchResult result in results)
        {
            string name = "N/A";
            string path = "N/A";
            if (result.Properties.Contains("ou"))
            {
                name = result.Properties["ou"][0].ToString()!;
                path = result.Properties["adspath"][0].ToString()!;
            }

            var dname = result.Properties.Contains("distinguishedName")
                ? result.Properties["distinguishedName"][0].ToString()!
                : "N/A";

            organizationalUnits.Add(new OrganizationalUnit(name, path, dname));
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("An error occurred while searching for organizational units: " + ex.Message);
    }

    return organizationalUnits;
}

List<string> SearchUser(string name, string adServicePath, ActiveDirectorySettings settings)
{
    var userList = new List<string>();
    try
    {
        DirectoryEntry entry = new DirectoryEntry(adServicePath, settings.User, settings.Password);

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