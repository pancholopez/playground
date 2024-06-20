using Auth.ActiveDirectory.Helpers;
using Auth.ActiveDirectory.Models;

namespace Auth.ActiveDirectory.Services;

public interface IActiveDirectoryService
{
    Result ValidateConnection(ActiveDirectorySettings settings);
    Result<DomainDetails> GetDomainDetails(ActiveDirectorySettings settings);
    Result<IEnumerable<OrganizationalUnit>> GetOrganizationalUnits(ActiveDirectorySettings settings);
}