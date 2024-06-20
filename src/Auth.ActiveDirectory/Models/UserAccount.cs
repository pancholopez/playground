using System.Diagnostics.CodeAnalysis;
using System.DirectoryServices;
using Auth.ActiveDirectory.Helpers;

namespace Auth.ActiveDirectory.Models;

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