using Domain.Entities;

namespace Application.IRepositories;

public interface IUserCardProgressRepository : IRepository<UserCardProgress>
{
    Task<IEnumerable<UserCardProgress>> GetByUserId(int userId);
    Task<IEnumerable<UserCardProgress>> GetByListCardId(IEnumerable<int> cardIds, int userId);
    
    // SRS methods
    Task<UserCardProgress?> GetByUserAndCardAsync(int userId, int cardId);
    Task<IEnumerable<UserCardProgress>> GetDueReviewsAsync(int userId, int? deckId, int limit);
    Task<IEnumerable<UserCardProgress>> GetGhostCardsAsync(int userId, int? deckId, int limit);
    Task<int> CountDueReviewsAsync(int userId, int? deckId);
    Task<int> CountGhostsAsync(int userId, int? deckId);
    Task<int> CountNewCardsAsync(int userId, int deckId);
    Task<int> CountAllNewCardsAsync(int userId);
    Task<IEnumerable<int>> GetLearnedCardIdsAsync(int userId, int deckId);
    
    // Stats methods
    Task<Dictionary<int, int>> GetLevelDistributionAsync(int userId, int? deckId);
    Task<Dictionary<DateTime, int>> GetForecastAsync(int userId, int days);
}
