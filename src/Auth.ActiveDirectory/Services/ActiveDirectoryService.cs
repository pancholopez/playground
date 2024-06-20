using System.Diagnostics.CodeAnalysis;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using Auth.ActiveDirectory.Helpers;
using Auth.ActiveDirectory.Models;

namespace Auth.ActiveDirectory.Services;

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
}