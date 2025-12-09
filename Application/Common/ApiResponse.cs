namespace Application.Common;

public class ApiResponse<T>
{
    public int Code { set; get; } = 0;
    public bool Success { set; get; } = true;
    public string? Message { set; get; }
    public T? Data { set; get; }
    public MetaData? MetaData { set; get; }

    public static ApiResponse<T> SuccessResponse(T data, MetaData? meta = null, string? message = null, int code = 200)
    {
        return new()
        {
            Code = code,
            Data = data,
            MetaData = meta,
            Message = message
        };
    }

    public static ApiResponse<T> FailResponse(string message, int code)
    {
        return new()
        {
            Message = message,
            Code = code,
            Success = false
        };
    }
}

public class MetaData
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public int Total { get; set; } = 0;
    public int TotalPage => PageSize <= 0 ? 0 : (int)Math.Ceiling((double)Total / PageSize);
}