using Domain.Entities;

namespace Application.IRepositories;

public interface ICardRepository : IRepository<Card>
{
    Task<IEnumerable<Card>> GetByListDeckId(IEnumerable<int> deckIds);
}
