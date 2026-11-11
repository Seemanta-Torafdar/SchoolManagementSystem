namespace SchoolManagement.Domain.Common;

public class ResultResponse<T>
{
    public bool Succeeded { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public T? Data { get; set; }

    public static ResultResponse<T> Success(T data, string message = "Operation completed successfully.")
    {
        return new ResultResponse<T> { Succeeded = true, Message = message, Data = data };
    }

    public static ResultResponse<T> Failure(string error, string message = "An error occurred.")
    {
        return new ResultResponse<T> { Succeeded = false, Message = message, Errors = new List<string> { error } };
    }

    public static ResultResponse<T> Failure(List<string> errors, string message = "Validation errors occurred.")
    {
        return new ResultResponse<T> { Succeeded = false, Message = message, Errors = errors };
    }
}

public class ResultResponse
{
    public bool Succeeded { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();

    public static ResultResponse Success(string message = "Operation completed successfully.")
    {
        return new ResultResponse { Succeeded = true, Message = message };
    }

    public static ResultResponse Failure(string error, string message = "An error occurred.")
    {
        return new ResultResponse { Succeeded = false, Message = message, Errors = new List<string> { error } };
    }

    public static ResultResponse Failure(List<string> errors, string message = "Validation errors occurred.")
    {
        return new ResultResponse { Succeeded = false, Message = message, Errors = errors };
    }
}

