using Application.DTOs.Deck;

namespace Application.IServices;

public interface IDeckService
{
    Task<IEnumerable<DeckSummaryDTO>> GetMyDecksAsync(int userId);
    Task<DeckDetailDTO> CreateDeckAsync(int userId, CreateDeckRequest request);
}
