using System.Diagnostics.CodeAnalysis;
using System.DirectoryServices.AccountManagement;
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

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
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
}

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public class ConnectedState : ActiveDirectoryState
{
    public override Result Connect(ActiveDirectorySettings settings)
        => Result.Failure("You are already connected.");

    public override Result Disconnect()
    {
        _principalContext?.Dispose();
        StateContext.SetState(new DisconnectedState(StateContext));
        return Result.Ok();
    }

    public ConnectedState(PrincipalContext principalContext, IStateContext<ActiveDirectoryState> stateContext)
        : base(stateContext)
    {
        _principalContext = principalContext;
    }
}

public interface IStateContext<in T>
{
    void SetState(T state);
}

public interface IActiveDirectoryService
{
    Result Connect();
    void Disconnect();
}

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

    public Result Connect()
    {
        return _state.Connect(_settings);
    }

    public void Disconnect()
    {
        _state.Disconnect();
    }
}