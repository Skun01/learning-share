using Application.DTOs.Common;

namespace Application.DTOs.Card;

public class GetCardsRequest : BaseQueryDTO
{
    public string? Type { get; set; }
}
