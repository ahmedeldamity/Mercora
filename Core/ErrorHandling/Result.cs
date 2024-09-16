namespace Core.ErrorHandling;
public class Result
{
    public Result(bool isSuccess, Error? error)
    {
        if ((isSuccess && error != Error.None) || (!isSuccess && error == Error.None))
            throw new InvalidOperationException();

        IsSuccess = isSuccess;
        Error = error;
    }

    public Result(bool isSuccess, string successMessage)
    {
        if (!isSuccess || successMessage is null)
            throw new InvalidOperationException();

        IsSuccess = isSuccess;
        Error = Error.None;
        SuccessMessage = successMessage;
    }

    public bool IsSuccess { get; }
    public Error? Error { get; }

    public string? SuccessMessage;

    public static Result Success() => new(true, Error.None);
    public static Result Success(string successMessage) => new(true, successMessage);
    public static Result Failure(Error? error) => new(false, error);

    public static Result<TValue> Success<TValue>(TValue value) => new(value, true, Error.None);
    public static Result<TValue> Failure<TValue>(Error? error) => new(default, false, error);
}


public class Result<TValue>(TValue? value, bool isSuccess, Error? error) : Result(isSuccess, error)
{
    public TValue Value => IsSuccess ? value! : throw new InvalidOperationException("Failure results cannot have value");
}