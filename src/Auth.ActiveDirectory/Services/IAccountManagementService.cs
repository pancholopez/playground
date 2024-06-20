using Auth.ActiveDirectory.Helpers;
using Auth.ActiveDirectory.Models;

namespace Auth.ActiveDirectory.Services;

/// <summary>
/// Provides account management services for Active Directory.
/// </summary>
public interface IAccountManagementService
{
    /// <summary>
    /// Searches for user accounts in Active Directory.
    /// </summary>
    /// <param name="name">The name of the user to search for.</param>
    /// <param name="adServicePath">The Active Directory service path.</param>
    /// <param name="settings">The Active Directory settings.</param>
    /// <returns>A collection of user accounts that match the search criteria.</returns>
    Result<IEnumerable<UserAccount>> SearchAccount(string name, string adServicePath, ActiveDirectorySettings settings);

    /// <summary>
    /// Creates a new user account in Active Directory.
    /// </summary>
    /// <param name="ldapPath">The LDAP path where the account will be created.</param>
    /// <param name="account">The details of the new user account.</param>
    /// <param name="settings">The Active Directory settings.</param>
    /// <returns>The result of the account creation operation.</returns>
    Result<UserAccount> CreateAccount(string ldapPath, NewUserAccount account, ActiveDirectorySettings settings);

    /// <summary>
    /// Resets the password for a specified user account.
    /// </summary>
    /// <param name="account">The user account whose password will be reset.</param>
    /// <param name="password">The new password for the account.</param>
    /// <param name="settings">The Active Directory settings.</param>
    /// <returns>The result of the password reset operation.</returns>
    Result ResetPassword(UserAccount account, string password, ActiveDirectorySettings settings);

    /// <summary>
    /// Adds a user to a specified group in Active Directory.
    /// </summary>
    /// <param name="samAccountName">The SAM account name of the user.</param>
    /// <param name="groupName">The name of the group.</param>
    /// <param name="settings">The Active Directory settings.</param>
    /// <returns>The result of the add to group operation.</returns>
    Result AddUserToGroup(string samAccountName, string groupName, ActiveDirectorySettings settings);

    /// <summary>
    /// Deletes a specified user account from Active Directory.
    /// </summary>
    /// <param name="samAccountName">The SAM account name of the user to be deleted.</param>
    /// <param name="settings">The Active Directory settings.</param>
    /// <returns>The result of the account deletion operation.</returns>
    Result DeleteAccount(string samAccountName, ActiveDirectorySettings settings);
}
