using System.Diagnostics.CodeAnalysis;
using System.DirectoryServices;

namespace Auth.ActiveDirectory;

public record ActiveDirectorySettings(
    string ServerName,
    string UserName,
    string Password,
    int TimeOutInMilliSeconds);

public record OrganizationalUnit(
    string Name,
    string ActiveDirectoryServicePath,
    string DistinguishName)
{
    [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
    public static OrganizationalUnit Map(SearchResult result) => new(
        Name: result.Properties.GetValueOrDefault("ou"),
        ActiveDirectoryServicePath: result.Properties.GetValueOrDefault("adspath"),
        DistinguishName: result.Properties.GetValueOrDefault("distinguishedName")
    );
}