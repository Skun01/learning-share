using Application.IRepositories;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class DeckRepository : Repository<Deck>, IDeckRepository
{
    public DeckRepository(AppDbContext context) : base(context) {}

    public async Task<IEnumerable<Deck>> GetByUserId(int userId)
    {
        return await _context.Decks.Where(d => d.UserId == userId).ToListAsync();
    }
}
