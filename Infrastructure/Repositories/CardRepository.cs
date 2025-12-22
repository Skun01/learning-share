using Application.IRepositories;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CardRepository : Repository<Card>, ICardRepository
{
    public CardRepository(AppDbContext context) : base(context) {}
    
    public async Task<IEnumerable<Card>> GetByListDeckId(IEnumerable<int> deckIds)
    {
        return await _context.Cards.Where(c => deckIds.Contains(c.DeckId)).ToListAsync();
    }
}
