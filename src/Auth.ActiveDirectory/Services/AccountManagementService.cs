using System.Diagnostics.CodeAnalysis;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using Auth.ActiveDirectory.Helpers;
using Auth.ActiveDirectory.Models;

namespace Auth.ActiveDirectory.Services;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public class AccountManagementService : IAccountManagementService
{
    public Result<IEnumerable<UserAccount>> SearchAccount(string name, string adServicePath,
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