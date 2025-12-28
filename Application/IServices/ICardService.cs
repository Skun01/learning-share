using Application.DTOs.Card;
using Application.DTOs.Common;

namespace Application.IServices;

public interface ICardService
{
    Task<IEnumerable<CardSummaryDTO>> GetCardsByDeckIdAsync(int userId, int deckId);
    Task<IEnumerable<CardSummaryDTO>> GetCardsWithFilterAsync(int deckId, QueryDTO<GetCardsRequest> request);
    Task<CardDetailDTO> GetCardByIdAsync(int userId, int deckId, int cardId);
    Task<CardDetailDTO> CreateCardAsync(int userId, int deckId, CreateCardRequest request);
    Task<CardDetailDTO> UpdateCardAsync(int userId, int deckId, int cardId, UpdateCardRequest request);
    Task<bool> DeleteCardAsync(int userId, int deckId, int cardId);
    Task<BulkCreateCardsResponse> BulkCreateCardsAsync(int userId, int deckId, BulkCreateCardsRequest request);
    Task<BulkDeleteCardsResponse> BulkDeleteCardsAsync(int userId, int deckId, BulkDeleteCardsRequest request);
    Task<BulkUpdateCardsResponse> BulkUpdateCardsAsync(int userId, int deckId, BulkUpdateCardsRequest request);
    
    // Example methods
    Task<CardExampleDTO> AddExampleAsync(int userId, int cardId, CreateExampleRequest request);
    Task<CardExampleDTO> UpdateExampleAsync(int userId, int cardId, int exampleId, UpdateExampleRequest request);
    Task<bool> DeleteExampleAsync(int userId, int cardId, int exampleId);
    
    // Grammar methods
    Task<GrammarDetailsDTO> UpdateGrammarAsync(int userId, int cardId, UpdateGrammarRequest request);
}
