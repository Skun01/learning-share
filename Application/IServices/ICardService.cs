using Application.DTOs.Card;

namespace Application.IServices;

public interface ICardService
{
    Task<IEnumerable<CardSummaryDTO>> GetCardsByDeckIdAsync(int userId, int deckId);
    Task<CardDetailDTO> GetCardByIdAsync(int userId, int deckId, int cardId);
    Task<CardDetailDTO> CreateCardAsync(int userId, int deckId, CreateCardRequest request);
    Task<CardDetailDTO> UpdateCardAsync(int userId, int deckId, int cardId, UpdateCardRequest request);
    Task<bool> DeleteCardAsync(int userId, int deckId, int cardId);
}
