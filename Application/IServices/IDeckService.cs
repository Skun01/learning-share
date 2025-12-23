using Application.DTOs.Deck;

namespace Application.IServices;

public interface IDeckService
{
    Task<IEnumerable<DeckSummaryDTO>> GetMyDecksAsync(int userId);
    Task<DeckDetailDTO> CreateDeckAsync(int userId, CreateDeckRequest request);
    Task<DeckDetailDTO> UpdateDeckAsync(int userId, int deckId, UpdateDeckRequest request);
    Task<bool> DeleteDeckAsync(int userId, int deckId);
    Task<bool> ResetDeckProgressAsync(int userId, int deckId);
}
