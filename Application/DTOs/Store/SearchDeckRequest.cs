using Application.DTOs.Common;

namespace Application.DTOs.Store;

public class SearchDeckRequest : BaseQueryDTO
{
    public string? Type { get; set; }
    public List<string>? Tags { get; set; }
}
