using Domain.Entities;

namespace Application.IRepositories;

public interface ICardRepository : IRepository<Card>
{
    Task<IEnumerable<Card>> GetByListDeckId(IEnumerable<int> deckIds);
    Task<IEnumerable<Card>> GetSampleCardsByDeckIdAsync(int deckId, int count = 5);
    Task<IEnumerable<Card>> GetCardsByDeckIdWithDetailsAsync(int deckId);
    Task<IEnumerable<Card>> GetByDeckIdAsync(int deckId);
    Task<Card?> GetByIdWithDetailsAsync(int cardId);
}
