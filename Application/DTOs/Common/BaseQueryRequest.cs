namespace Application.DTOs.Common;

public class BaseQueryRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string SortBy { get; set; } = string.Empty;
    public string SortOrder { get; set; } = "desc";
}
