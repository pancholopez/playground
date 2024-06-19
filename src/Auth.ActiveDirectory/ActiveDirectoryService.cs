using System.Diagnostics.CodeAnalysis;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using Microsoft.Extensions.Logging;

namespace Auth.ActiveDirectory;

public record Result(bool IsSuccess, string ErrorMessage)
{
    private readonly string _errorMessage = ErrorMessage;

    public bool IsFailure => !IsSuccess;

    public string ErrorMessage
    {
        get => _errorMessage;
        init => _errorMessage = IsFailure && !string.IsNullOrWhiteSpace(value)
            ? value
            : throw new InvalidOperationException();
    }

    public static Result Ok() => new Result(true, string.Empty);
    public static Result<T> Ok<T>(T value) => new(true, string.Empty, value);
    public static Result Failure(string errorMessage) => new(false, errorMessage);
    public static Result<T> Failure<T>(string errorMessage) => new(false, errorMessage, default!);
}

public record Result<T>(bool IsSuccess, string ErrorMessage, T Value) : Result(IsSuccess, ErrorMessage)
{
    private readonly T _value = Value;

    public T Value
    {
        get => _value;
        init => _value = IsSuccess ? value : throw new InvalidOperationException();
    }
}

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public abstract class ActiveDirectoryState : IDisposable
{
    private bool _disposed;
    private protected IStateContext<ActiveDirectoryState> StateContext;
    private protected PrincipalContext? _principalContext;

    protected ActiveDirectoryState(IStateContext<ActiveDirectoryState> stateContext)
    {
        StateContext = stateContext;
    }

    public abstract Result Connect(ActiveDirectorySettings settings);
    public abstract Result Disconnect();
    public abstract Result<DomainDetails> GetDomainDetails(ActiveDirectorySettings settings);
    public abstract Result<IEnumerable<OrganizationalUnit>> GetOrganizationalUnits(ActiveDirectorySettings settings);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
        {
            _principalContext?.Dispose();
            _principalContext = null;
        }

        _disposed = true;
    }
}

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public class DisconnectedState : ActiveDirectoryState
{
    public DisconnectedState(IStateContext<ActiveDirectoryState> stateContext) : base(stateContext)
    {
    }

    public override Result Connect(ActiveDirectorySettings settings)
    {
        try
        {
            var principalContext = new PrincipalContext(
                contextType: ContextType.Domain,
                name: settings.ServerName,
                userName: settings.UserName,
                password: settings.Password);

            if (principalContext.ConnectedServer is null)
                return Result.Failure($"Connecting to {settings.ServerName} failed.");

            StateContext.SetState(new ConnectedState(principalContext, StateContext));
            return Result.Ok();
        }
        catch (Exception exception)
        {
            return Result.Failure($"Connecting to {settings.ServerName} failed.{exception.Message}");
        }
    }

    public override Result Disconnect()
        => Result.Failure("You are already disconnected.");

    public override Result<DomainDetails> GetDomainDetails(ActiveDirectorySettings settings)
        => Result.Failure<DomainDetails>("You are not connected to any server.");

    public override Result<IEnumerable<OrganizationalUnit>> GetOrganizationalUnits(ActiveDirectorySettings settings)
        => Result.Failure<IEnumerable<OrganizationalUnit>>("You are not connected to any server.");
}

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public class ConnectedState : ActiveDirectoryState
{
    public ConnectedState(PrincipalContext principalContext, IStateContext<ActiveDirectoryState> stateContext)
        : base(stateContext)
    {
        _principalContext = principalContext;
    }

    public override Result Connect(ActiveDirectorySettings settings)
        => Result.Failure("You are already connected.");

    public override Result Disconnect()
    {
        Dispose();
        StateContext.SetState(new DisconnectedState(StateContext));
        return Result.Ok();
    }

    public override Result<DomainDetails> GetDomainDetails(ActiveDirectorySettings settings)
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

    public override Result<IEnumerable<OrganizationalUnit>> GetOrganizationalUnits(ActiveDirectorySettings settings)
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
        if (propertyValue is byte[] bytes)  // if the value is a GUID
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

public interface IStateContext<in T>
{
    void SetState(T state);
}

public interface IActiveDirectoryService
{
    Result Connect();
    void Disconnect();
    Result<ActiveDirectoryDetails> GetServerDetails();
}

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public class ActiveDirectoryService : IActiveDirectoryService, IStateContext<ActiveDirectoryState>
{
    private readonly ActiveDirectorySettings _settings;
    private readonly ILogger<ActiveDirectoryService> _logger;
    private ActiveDirectoryState _state;

    public ActiveDirectoryService(ActiveDirectorySettings settings, ILogger<ActiveDirectoryService> logger)
    {
        _settings = settings;
        _logger = logger;
        _state = new DisconnectedState(this);
    }

    public void SetState(ActiveDirectoryState state)
    {
        _logger.LogInformation("Transition from {oldState} to {newState}",
            _state.GetType().Name, state.GetType().Name);
        _state = state;
    }

    public Result<ActiveDirectoryDetails> GetServerDetails()
    {
        var directoryDetails = ActiveDirectoryDetails.Null;

        var detailsResult = _state.GetDomainDetails(_settings);
        if (detailsResult.IsSuccess)
        {
            directoryDetails = new ActiveDirectoryDetails(detailsResult.Value, []);
        }
        else
        {
            _logger.LogError("{method} failed. {reason}",
                nameof(_state.GetDomainDetails), detailsResult.ErrorMessage);
        }

        var ouResult = _state.GetOrganizationalUnits(_settings);
        if (ouResult.IsFailure)
        {
            _logger.LogError("{method} failed. {reason}",
                nameof(_state.GetOrganizationalUnits), ouResult.ErrorMessage);
            return Result.Failure<ActiveDirectoryDetails>($"{nameof(GetServerDetails)} failed.");
        }

        return Result.Ok(directoryDetails with { OrganizationalUnits = ouResult.Value.ToList() });
    }

    // SearchUser

    // CreateNewUser

    // ResetPassword

    // DeleteUser

    public Result Connect() => _state.Connect(_settings);

    public void Disconnect() => _state.Disconnect();
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