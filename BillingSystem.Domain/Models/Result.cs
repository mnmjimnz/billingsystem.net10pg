namespace BillingSystem.Domain.Models;

public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public T Data { get; private set; }
    public string Error { get; private set; }
    public string Message { get; private set; }

    private Result(bool isSuccess, T data, string error, string message)
    {
        IsSuccess = isSuccess;
        Data = data;
        Error = error;
        Message = message;
    }

    public static Result<T> Success(T data, string message = "")
    {
        return new Result<T>(true, data, string.Empty, message);
    }

    public static Result<T> Failure(string error)
    {
        return new Result<T>(false, default!, error, string.Empty);
    }
}
