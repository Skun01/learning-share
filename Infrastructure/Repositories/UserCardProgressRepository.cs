using Application.IRepositories;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserCardProgressRepository : Repository<UserCardProgress>, IUserCardProgressRepository
{
    public UserCardProgressRepository(AppDbContext context) : base(context){}

    public async Task<IEnumerable<UserCardProgress>> GetByUserId(int userId)
    {
        return await _context.UserCardProgresses.Where(ucp => ucp.UserId == userId).ToListAsync();
    }

    public async Task<IEnumerable<UserCardProgress>> GetByListCardId(IEnumerable<int> cardIds, int userId)
    {
        return await _context.UserCardProgresses
            .Where(ucp => ucp.UserId == userId && cardIds.Contains(ucp.CardId))
            .ToListAsync();
    }

    // SRS methods
    public async Task<UserCardProgress?> GetByUserAndCardAsync(int userId, int cardId)
    {
        return await _context.UserCardProgresses
            .FirstOrDefaultAsync(ucp => ucp.UserId == userId && ucp.CardId == cardId);
    }

    public async Task<IEnumerable<UserCardProgress>> GetDueReviewsAsync(int userId, int? deckId, int limit)
    {
        var now = DateTime.UtcNow;
        var query = _context.UserCardProgresses
            .Include(ucp => ucp.Card)
                .ThenInclude(c => c.Deck)
            .Include(ucp => ucp.Card)
                .ThenInclude(c => c.Examples)
                    .ThenInclude(e => e.AudioMedia)
            .Include(ucp => ucp.Card)
                .ThenInclude(c => c.GrammarDetails)
            .Include(ucp => ucp.Card)
                .ThenInclude(c => c.ImageMedia)
            .Where(ucp => ucp.UserId == userId 
                && ucp.NextReviewDate != null 
                && ucp.NextReviewDate <= now);

        if (deckId.HasValue)
        {
            query = query.Where(ucp => ucp.Card.DeckId == deckId.Value);
        }

        // Prioritize ghost cards first, then by NextReviewDate
        return await query
            .OrderByDescending(ucp => ucp.GhostLevel)
            .ThenBy(ucp => ucp.NextReviewDate)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserCardProgress>> GetGhostCardsAsync(int userId, int? deckId, int limit)
    {
        var query = _context.UserCardProgresses
            .Include(ucp => ucp.Card)
            .Where(ucp => ucp.UserId == userId && ucp.GhostLevel > 0);

        if (deckId.HasValue)
        {
            query = query.Where(ucp => ucp.Card.DeckId == deckId.Value);
        }

        return await query
            .OrderByDescending(ucp => ucp.GhostLevel)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<int> CountDueReviewsAsync(int userId, int? deckId)
    {
        var now = DateTime.UtcNow;
        var query = _context.UserCardProgresses
            .Where(ucp => ucp.UserId == userId 
                && ucp.NextReviewDate != null 
                && ucp.NextReviewDate <= now);

        if (deckId.HasValue)
        {
            query = query.Where(ucp => ucp.Card.DeckId == deckId.Value);
        }

        return await query.CountAsync();
    }

    public async Task<int> CountGhostsAsync(int userId, int? deckId)
    {
        var query = _context.UserCardProgresses
            .Where(ucp => ucp.UserId == userId && ucp.GhostLevel > 0);

        if (deckId.HasValue)
        {
            query = query.Where(ucp => ucp.Card.DeckId == deckId.Value);
        }

        return await query.CountAsync();
    }

    public async Task<int> CountNewCardsAsync(int userId, int deckId)
    {
        var learnedCardIds = await GetLearnedCardIdsAsync(userId, deckId);
        
        return await _context.Cards
            .Where(c => c.DeckId == deckId && !learnedCardIds.Contains(c.Id))
            .CountAsync();
    }

    public async Task<int> CountAllNewCardsAsync(int userId)
    {
        var userDeckIds = await _context.Decks
            .Where(d => d.UserId == userId)
            .Select(d => d.Id)
            .ToListAsync();

        var learnedCardIds = await _context.UserCardProgresses
            .Where(ucp => ucp.UserId == userId)
            .Select(ucp => ucp.CardId)
            .ToListAsync();

        return await _context.Cards
            .Where(c => userDeckIds.Contains(c.DeckId) && !learnedCardIds.Contains(c.Id))
            .CountAsync();
    }

    public async Task<IEnumerable<int>> GetLearnedCardIdsAsync(int userId, int deckId)
    {
        return await _context.UserCardProgresses
            .Where(ucp => ucp.UserId == userId && ucp.Card.DeckId == deckId)
            .Select(ucp => ucp.CardId)
            .ToListAsync();
    }

    public async Task<Dictionary<int, int>> GetLevelDistributionAsync(int userId, int? deckId)
    {
        var query = _context.UserCardProgresses
            .Where(ucp => ucp.UserId == userId);

        if (deckId.HasValue)
        {
            query = query.Where(ucp => ucp.Card.DeckId == deckId.Value);
        }

        var distribution = await query
            .GroupBy(ucp => (int)ucp.SRSLevel)
            .Select(g => new { Level = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Level, x => x.Count);

        return distribution;
    }

    public async Task<Dictionary<DateTime, int>> GetForecastAsync(int userId, int days)
    {
        var now = DateTime.UtcNow.Date;
        var endDate = now.AddDays(days);

        var forecast = await _context.UserCardProgresses
            .Where(ucp => ucp.UserId == userId 
                && ucp.NextReviewDate.HasValue 
                && ucp.NextReviewDate.Value.Date >= now 
                && ucp.NextReviewDate.Value.Date <= endDate)
            .GroupBy(ucp => ucp.NextReviewDate!.Value.Date)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Date, x => x.Count);

        return forecast;
    }
}
