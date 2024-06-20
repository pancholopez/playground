using System.Diagnostics.CodeAnalysis;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;

namespace Auth.ActiveDirectory;

public interface IActiveDirectoryService
{
    Result ValidateConnection(ActiveDirectorySettings settings);
    Result<DomainDetails> GetDomainDetails(ActiveDirectorySettings settings);
    Result<IEnumerable<OrganizationalUnit>> GetOrganizationalUnits(ActiveDirectorySettings settings);

    Result<IEnumerable<UserAccount>> SearchUserAccount(string name, string adServicePath,
        ActiveDirectorySettings settings);

    public Result<UserAccount> CreateAccount(string ldapPath, NewUserAccount account, ActiveDirectorySettings settings);

    public Result ResetPassword(UserAccount account, string password, ActiveDirectorySettings settings);
    public Result AddUserToGroup(string samAccountName, string groupName, ActiveDirectorySettings settings);
    public Result DeleteAccount(string samAccountName, ActiveDirectorySettings settings);
}

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public class ActiveDirectoryService : IActiveDirectoryService
{
    public Result ValidateConnection(ActiveDirectorySettings settings)
    {
        try
        {
            var principalContext = new PrincipalContext(
                contextType: ContextType.Domain,
                name: settings.ServerName,
                userName: settings.UserName,
                password: settings.Password);

            return principalContext.ConnectedServer is null
                ? Result.Failure($"Connecting to {settings.ServerName} failed.")
                : Result.Ok();
        }
        catch (Exception exception)
        {
            return Result.Failure($"{nameof(ValidateConnection)} operation failed.{exception.Message}");
        }
    }

    public Result<DomainDetails> GetDomainDetails(ActiveDirectorySettings settings)
    {
        var cst = new CancellationTokenSource();

        try
        {
            var context = new DirectoryContext(
                contextType: DirectoryContextType.DirectoryServer,
                name: settings.ServerName,
                username: settings.UserName,
                password: settings.Password
            );

            var domainTask = Task.Run(() => Domain.GetDomain(context), cst.Token);

            if (!domainTask.Wait(TimeSpan.FromMilliseconds(settings.TimeOutInMilliSeconds), cst.Token))
            {
                cst.Cancel();
                return Result.Failure<DomainDetails>($"{nameof(Domain.GetDomain)} operation timeout.");
            }

            var domain = domainTask.Result;

            var details = new DomainDetails(
                ForestName: domain.Forest.Name,
                DomainControllers: domain.DomainControllers.Cast<DomainController>()
                    .Select(x => x.Name).ToList());

            return Result.Ok(details);
        }
        catch (Exception exception)
        {
            return Result.Failure<DomainDetails>($"{nameof(GetDomainDetails)} failed. {exception.StackTrace}");
        }
    }

    public Result<IEnumerable<OrganizationalUnit>> GetOrganizationalUnits(ActiveDirectorySettings settings)
    {
        var ldapPath = $"LDAP://{settings.ServerName}";
        try
        {
            var entry = new DirectoryEntry(ldapPath, settings.UserName, settings.Password);

            var searcher = new DirectorySearcher(entry)
            {
                Filter = "(objectClass=organizationalUnit)"
            };

            searcher.PropertiesToLoad.Add("ou");
            searcher.PropertiesToLoad.Add("adspath");
            searcher.PropertiesToLoad.Add("distinguishedName");

            var results = searcher.FindAll();

            var organizationalUnits = results.Cast<SearchResult>().Select(OrganizationalUnit.Map);

            return Result.Ok(organizationalUnits);
        }
        catch (Exception exception)
        {
            return Result.Failure<IEnumerable<OrganizationalUnit>>(
                $"{nameof(GetOrganizationalUnits)} failed. {exception.Message}");
        }
    }

    public Result<IEnumerable<UserAccount>> SearchUserAccount(string name, string adServicePath,
        ActiveDirectorySettings settings)
    {
        try
        {
            var entry = new DirectoryEntry(adServicePath, settings.UserName, settings.Password);

            // todo: add other filters like sAMAccount
            var searcher = new DirectorySearcher(entry)
            {
                Filter = $"(&(objectClass=user)(mail=*{name}*))"
            };

            var results = searcher.FindAll();

            var accountCollection = results.Cast<SearchResult>().Select(UserAccount.Map);

            return Result.Ok(accountCollection);
        }
        catch (Exception exception)
        {
            return Result
                .Failure<IEnumerable<UserAccount>>($"{nameof(DirectorySearcher)} failed. {exception.Message}");
        }
    }

    public Result<UserAccount> CreateAccount(
        string ldapPath, NewUserAccount account, ActiveDirectorySettings settings)
    {
        try
        {
            using var directoryEntry = new DirectoryEntry(ldapPath, settings.UserName, settings.Password);
            // Create a new user.
            var newUser = directoryEntry.Children.Add($"CN={account.FullName}", "user");

            newUser.Properties["userAccountControl"].Value = 0x20; // Password Not Required

            newUser.Properties["mail"].Add(account.Email);
            newUser.Properties["cn"].Value = account.FullName;
            newUser.Properties["samAccountName"].Value = account.UserName;
            newUser.Properties["userPrincipalName"].Value = $"{account.UserName}@Cert.Local";
            newUser.Properties["description"].Value = "QFR";
            newUser.Properties["givenName"].Value = account.FirstName;
            newUser.Properties["sn"].Value = account.LastName;
            newUser.Properties["displayName"].Value = account.FullName;

            newUser.Properties["accountExpires"].Add(account.ExpirationDate.HasValue
                ? account.ExpirationDate.Value.ToFileTime().ToString()
                : DateTime.Now.AddDays(1).ToFileTime().ToString());

            // Enable the account by setting the userAccountControl flag.
            int val = (int)newUser.Properties["userAccountControl"].Value!;
            newUser.Properties["userAccountControl"].Value = val & ~0x2; // Clear the ACCOUNT_DISABLE flag

            // Save the new user to the directory.
            newUser.CommitChanges();

            return Result.Ok(UserAccount.Map(newUser));
        }
        catch (Exception exception)
        {
            return Result.Failure<UserAccount>($"{nameof(CreateAccount)} failed. {exception.Message}");
        }
    }

    public Result ResetPassword(UserAccount account, string password, ActiveDirectorySettings settings)
    {
        try
        {
            using var principalContext = new PrincipalContext(ContextType.Domain,
                settings.ServerName, settings.UserName, settings.Password);
            var user = UserPrincipal.FindByIdentity(principalContext, account.SecurityAccountManagerName);

            if (user == null) return Result.Failure("User not found.");

            // this requires credentials with explicit privilege to change password
            user.SetPassword(password);
            user.Save();

            return Result.Ok();
        }
        catch (Exception exception)
        {
            return Result.Failure($"UserPrincipal.SetPassword failed. {exception.Message}");
        }
    }

    public Result AddUserToGroup(string samAccountName, string groupName, ActiveDirectorySettings settings)
    {
        try
        {
            using (var context = new PrincipalContext(ContextType.Domain,
                       settings.ServerName, settings.UserName, settings.Password))
            {
                var user = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, samAccountName);
                if(user is null) return Result.Failure($"User {samAccountName} not found.");
                
                var group = GroupPrincipal.FindByIdentity(context, groupName);
                if (group is null) return Result.Failure($"Group {groupName} not found.");

                group.Members.Add(user);

                group.Save();
            }

            return Result.Ok();
        }
        catch (Exception exception)
        {
            return Result.Failure($"{nameof(AddUserToGroup)} failed. {exception.Message}");
        }
    }

    public Result DeleteAccount(string samAccountName, ActiveDirectorySettings settings)
    {
        try
        {
            using (var principalContext = new PrincipalContext(ContextType.Domain, settings.ServerName,
                       settings.UserName, settings.Password))
            {
                var user = UserPrincipal.FindByIdentity(principalContext, IdentityType.SamAccountName, samAccountName);

                if (user is null)
                    return Result.Failure($"User {samAccountName} not found.");

                user.Delete();
                return Result.Ok();
            }
        }
        catch (Exception exception)
        {
            return Result.Failure($"{nameof(DeleteAccount)} failed. {exception.Message}");
        }
    }
}

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
internal static class SearchResultExtensions
{
    public static string GetValueOrDefault(this ResultPropertyCollection properties,
        string propertyName, string defaultValue = "N/A")
    {
        if (!properties.Contains(propertyName)) return defaultValue;

        var propertyValue = properties[propertyName][0];
        if (propertyValue is byte[] bytes) // if the value is a GUID
        {
            return new Guid(bytes).ToString();
        }

        return propertyValue.ToString()!;
    }

    public static ICollection<string> GetCollectionOrDefault(this ResultPropertyCollection properties,
        string propertyName) => properties.Contains(propertyName)
        ? properties[propertyName].OfType<string>().ToList()
        : [];

    public static string GetValueOrDefault(this PropertyCollection properties,
        string propertyName, string defaultValue = "N/A")
    {
        if (!properties.Contains(propertyName)) return defaultValue;

        var propertyValue = properties[propertyName][0];
        if (propertyValue is byte[] bytes) // if the value is a GUID
        {
            return new Guid(bytes).ToString();
        }

        return propertyValue?.ToString() ?? string.Empty;
    }
}

public record NewUserAccount(
    string UserName,
    string Email,
    string FirstName,
    string LastName,
    DateTime? ExpirationDate = null
)
{
    public string FullName => $"{FirstName} {LastName}";
}

public record DomainDetails(string ForestName, ICollection<string> DomainControllers)
{
    public static readonly DomainDetails Null = new("N/A", []);
}

public record UserAccount(
    string Email,
    string CommonName,
    string UserPrincipalName,
    string SecurityAccountManagerName,
    string DistinguishedName,
    ICollection<string> MemberOf,
    string? Id = null,
    string? Description = null,
    string? FirstName = null,
    string? LastName = null,
    string? DisplayName = null,
    string? AccountCreatedTimeStamp = null,
    string? AccountExpirationFileTime = null,
    string? LastUpdateTimeStamp = null,
    string? Path = null)
{
    [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
    public static UserAccount Map(SearchResult result) => new(Email: result.Properties.GetValueOrDefault("mail"),
        CommonName: result.Properties.GetValueOrDefault("cn"),
        UserPrincipalName: result.Properties.GetValueOrDefault("userPrincipalName"),
        SecurityAccountManagerName: result.Properties.GetValueOrDefault("sAMAccountName"),
        DistinguishedName: result.Properties.GetValueOrDefault("distinguishedName"),
        MemberOf: result.Properties.GetCollectionOrDefault("memberOf"),
        Id: result.Properties.GetValueOrDefault("objectGUID"),
        Description: result.Properties.GetValueOrDefault("description"),
        DisplayName: result.Properties.GetValueOrDefault("displayName"),
        AccountCreatedTimeStamp: result.Properties.GetValueOrDefault("whenCreated"),
        AccountExpirationFileTime: result.Properties.GetValueOrDefault("accountExpires"),
        LastUpdateTimeStamp: result.Properties.GetValueOrDefault("whenChanged"));

    [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
    public static UserAccount Map(DirectoryEntry entry) => new(
        Email: entry.Properties.GetValueOrDefault("mail"),
        CommonName: entry.Properties.GetValueOrDefault("cn"),
        UserPrincipalName: entry.Properties.GetValueOrDefault("userPrincipalName"),
        SecurityAccountManagerName: entry.Properties.GetValueOrDefault("samAccountName"),
        DistinguishedName: entry.Properties["distinguishedName"][0]!.ToString()!,
        MemberOf: [],
        Id: entry.Properties.GetValueOrDefault("objectGUID"),
        Description: entry.Properties.GetValueOrDefault("description"),
        // unable to parse back account expiration date
        FirstName: entry.Properties.GetValueOrDefault("givenName"),
        LastName: entry.Properties.GetValueOrDefault("sn"),
        DisplayName: entry.Properties.GetValueOrDefault("displayName"),
        Path: entry.Path
    );
}