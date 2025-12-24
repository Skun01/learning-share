namespace Application.DTOs.Common;

public class QueryDTO : BaseRequest
{
    public int Total { get; set; }
}

public class QueryDTO<T> : QueryDTO
{
    public T? Query { get; set; }
}