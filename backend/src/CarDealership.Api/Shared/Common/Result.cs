namespace CarDealership.Api.Shared.Common;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string? Error { get; }

    protected Result(bool isSuccess, string? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, null);

    public static Result Failure(string error)
    {
        if (string.IsNullOrWhiteSpace(error))
        {
            throw new ArgumentException("Error message must be provided for failure.", nameof(error));
        }

        return new Result(false, error);
    }
}

public sealed class Result<T> : Result
{
    public T? Value { get; }

    private Result(T? value, bool isSuccess, string? error)
        : base(isSuccess, error)
    {
        Value = value;
    }

    public static Result<T> Success(T value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value), "Success value cannot be null.");
        }

        return new Result<T>(value, true, null);
    }

    public new static Result<T> Failure(string error) => new(default, false, error);
}
