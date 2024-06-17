namespace Auth.ActiveDirectory;

public record ActiveDirectorySettings(string ServerName, string UserName, string Password);

public record OrganizationalUnit(
    string Name, 
    string ActiveDirectoryServicePath, 
    string DistinguishName)
{
    public List<OrganizationalUnit> OrganizationalUnits { get; init; } = [];
}