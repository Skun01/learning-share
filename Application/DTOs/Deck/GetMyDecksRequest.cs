using Application.DTOs.Common;

namespace Application.DTOs.Deck;

public class GetMyDecksRequest : BaseQueryRequest
{
    public string? Type { get; set; }
    public bool? IsPublic { get; set; }
    
    public GetMyDecksRequest()
    {
        SortBy = "CardsDue";
    }
}
