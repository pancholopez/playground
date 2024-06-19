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

            var organizationalUnits = results.Cast<SearchResult>()
                .Select(result => new OrganizationalUnit(
                    Name: result.Properties.GetValueOrDefault("ou"),
                    ActiveDirectoryServicePath: result.Properties.GetValueOrDefault("adspath"),
                    DistinguishName: result.Properties.GetValueOrDefault("distinguishedName"))
                ).ToList();

            return Result.Ok<IEnumerable<OrganizationalUnit>>(organizationalUnits);
        }
        catch (Exception exception)
        {
            return Result.Failure<IEnumerable<OrganizationalUnit>>(
                $"{nameof(GetOrganizationalUnits)} failed. {exception.Message}");
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
}

public record ActiveDirectoryDetails(DomainDetails DomainDetails, ICollection<OrganizationalUnit> OrganizationalUnits)
{
    public static readonly ActiveDirectoryDetails Null = new(DomainDetails.Null, []);
}

public record DomainDetails(string ForestName, ICollection<string> DomainControllers)
{
    public static readonly DomainDetails Null = new("N/A", []);
}

public record UserSearchResult(
    string Id,
    string Email,
    string CommonName,
    string UserPrincipalName,
    string SecurityAccountManagerName,
    string DistinguishedName,
    ICollection<string> MemberOf,
    string? Description = null,
    string? DisplayName = null,
    string? AccountCreatedTimeStamp = null,
    string? AccountExpirationFileTime = null,
    string? LastUpdateTimeStamp = null);