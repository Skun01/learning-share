using Application.DTOs.Common;

namespace Application.DTOs.Deck;

public class GetMyDecksRequest : BaseQueryDTO
{
    public string? Type { get; set; }
    public bool? IsPublic { get; set; }
}
