using Application.Common;
using Application.DTOs.Common;
using Application.DTOs.Deck;

namespace Application.IServices;

public interface IDeckService
{
    Task<IEnumerable<DeckSummaryDTO>> GetMyDecksByFilterAsync(QueryDTO<GetMyDecksRequest> request);
    Task<DeckDetailDTO> GetDeckByIdAsync(int userId, int deckId);
    Task<DeckStatisticsDTO> GetDeckStatisticsAsync(int userId);
    Task<DeckDetailDTO> CreateDeckAsync(int userId, CreateDeckRequest request);
    Task<DeckDetailDTO> UpdateDeckAsync(int userId, int deckId, UpdateDeckRequest request);
    Task<DeckDetailDTO> TogglePublishAsync(int userId, int deckId);
    Task<bool> DeleteDeckAsync(int userId, int deckId);
    Task<bool> ResetDeckProgressAsync(int userId, int deckId);
}
