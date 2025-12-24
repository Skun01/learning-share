namespace Application.DTOs.Common;

public class BaseRequest
{
    public int UserId { get; set; }
}

public class RequestDTO<T> : BaseRequest
{
    public T? Request { get; set; }
}
