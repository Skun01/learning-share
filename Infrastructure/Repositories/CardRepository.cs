using Application.DTOs.Common;
using Application.DTOs.Study;
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

    public async Task<IEnumerable<Card>> GetSampleCardsByDeckIdAsync(int deckId, int count = 5)
    {
        return await _context.Cards
            .Where(c => c.DeckId == deckId)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<Card>> GetCardsByDeckIdWithDetailsAsync(int deckId)
    {
        return await _context.Cards
            .Where(c => c.DeckId == deckId)
            .Include(c => c.Examples)
                .ThenInclude(e => e.AudioMedia)
            .Include(c => c.GrammarDetails)
            .Include(c => c.ImageMedia)
            .ToListAsync();
    }

    public async Task<IEnumerable<Card>> GetByDeckIdAsync(int deckId)
    {
        return await _context.Cards
            .Where(c => c.DeckId == deckId)
            .Include(c => c.Examples)
                .ThenInclude(e => e.AudioMedia)
            .Include(c => c.GrammarDetails)
            .Include(c => c.ImageMedia)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Card?> GetByIdWithDetailsAsync(int cardId)
    {
        return await _context.Cards
            .Include(c => c.Examples)
                .ThenInclude(e => e.AudioMedia)
            .Include(c => c.GrammarDetails)
            .Include(c => c.ImageMedia)
            .FirstOrDefaultAsync(c => c.Id == cardId);
    }

    public async Task<IEnumerable<Card>> GetNewCardsAsync(QueryDTO<GetNewLessonsRequest> request)
    {
        var userId = request.UserId;
        var deckId = request.Query!.DeckId;
        var limit = request.Query.Limit;

        // Lấy cardIds đã học bởi user này
        var learnedCardIds = _context.UserCardProgresses
            .Where(ucp => ucp.UserId == userId && ucp.Card.DeckId == deckId)
            .Select(ucp => ucp.CardId);

        // Query cards mới chưa học
        var query = _context.Cards
            .Where(c => c.DeckId == deckId && !learnedCardIds.Contains(c.Id))
            .Include(c => c.Deck)
            .Include(c => c.Examples)
                .ThenInclude(e => e.AudioMedia)
            .Include(c => c.GrammarDetails)
            .Include(c => c.ImageMedia)
            .AsNoTracking()
            .Take(limit);

        return await query.ToListAsync();
    }
}
