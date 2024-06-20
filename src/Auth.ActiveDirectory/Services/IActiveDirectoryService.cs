using Auth.ActiveDirectory.Helpers;
using Auth.ActiveDirectory.Models;

namespace Auth.ActiveDirectory.Services;

/// <summary>
/// Defines the contract for services interacting with Active Directory.
/// </summary>
public interface IActiveDirectoryService
{
    /// <summary>
    /// Validates the connection to the Active Directory.
    /// </summary>
    /// <returns>A result indicating success or failure of the connection validation.</returns>
    Result ValidateConnection();

    /// <summary>
    /// Retrieves details about the domain from Active Directory.
    /// </summary>
    /// <returns>A result containing domain details or an error message.</returns>
    Result<DomainDetails> GetDomainDetails();

    /// <summary>
    /// Retrieves a list of organizational units from Active Directory.
    /// </summary>
    /// <returns>A result containing an enumerable list of organizational units or an error message.</returns>
    Result<IEnumerable<OrganizationalUnit>> GetOrganizationalUnits();
}
