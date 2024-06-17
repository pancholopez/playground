namespace Auth.ActiveDirectory;

public record ActiveDirectorySettings(string Address, string User, string Password);

public record OrganizationalUnit(
    string Name, 
    string ActiveDirectoryServicePath, 
    string DistinguishName)
{
    public List<OrganizationalUnit> OrganizationalUnits { get; init; } = [];
}