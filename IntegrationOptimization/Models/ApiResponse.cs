namespace IntegrationOptimization.Models;

public class ApiResponse<T>
{
    public bool IsSuccess { get; private set; }
    public bool IsFailure => !IsSuccess;
    public T? Value { get; private set; }
    public string Error { get; private set; } = string.Empty;

    private ApiResponse(bool isSuccess, T? value, string error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public static ApiResponse<T> Success(T value)
    {
        return new ApiResponse<T>(true, value, string.Empty);
    }

    public static ApiResponse<T> Failure(string error)
    {
        return new ApiResponse<T>(false, default(T), error);
    }

    public static implicit operator ApiResponse<T>(T value)
    {
        return Success(value);
    }
}