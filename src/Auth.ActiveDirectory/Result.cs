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