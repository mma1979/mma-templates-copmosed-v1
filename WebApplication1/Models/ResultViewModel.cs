namespace WebApplication1.Models;

public class ResultViewModel<T>
{
    public long Total { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public string? Filter { get; set; }
    public T? Data { get; set; }
    public List<string>? Messages { get; set; } = new();
    public bool IsSuccess { get; set; }
    public int StatusCode { get; set; }

    public static ResultViewModel<T> Error(List<string>? messages=default)
    {
        return new ResultViewModel<T>
        {
            Messages = messages,
            IsSuccess = false,
            StatusCode = 500
        };
    }

    public static ResultViewModel<T> Success(T data,long total=default,int pageNumber=1, int pageSize=10 ,List<string>? messages = default)
    {
        return new ResultViewModel<T>
        {
            Data  = data,
            Total = total,
            PageNumber = pageNumber,
            PageSize = pageSize,
            Messages = messages,
            IsSuccess = true,
            StatusCode = 200
        };
    }
}