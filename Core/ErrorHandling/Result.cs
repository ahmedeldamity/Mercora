namespace Core.ErrorHandling;
public class Result
{
    public Result(bool isSuccess, ApiResponse error)
    {
        if (isSuccess && error.StatusCode != 200 || !isSuccess && error.StatusCode == 200)
            throw new InvalidOperationException();

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public ApiResponse Error { get; } = default!;

    public static Result Success(string? title) => new(true, new ApiResponse(200, title));
    public static Result Failure(int statusCode, string? title) => new(false, new ApiResponse(statusCode, title));

    public static Result<TValue> Success<TValue>(TValue value) => new(value, true, new ApiResponse(200));
    public static Result<TValue> Failure<TValue>(int statusCode, string title) => new(default, false, new ApiResponse(statusCode, title));
}


public class Result<TValue>(TValue? value, bool isSuccess, ApiResponse error) : Result(isSuccess, error)
{
    private readonly TValue? _value = value;

    public TValue Value => IsSuccess ? _value! : throw new InvalidOperationException("Failure results cannot have value");
}