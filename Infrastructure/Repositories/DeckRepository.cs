using Application.DTOs.Common;
using Application.DTOs.Deck;
using Application.DTOs.Store;
using Application.IRepositories;
using Azure;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class DeckRepository : Repository<Deck>, IDeckRepository
{
    public DeckRepository(AppDbContext context) : base(context) {}

    public async Task<Deck> CreateCloneAsync(Deck originDeck, int userId, string? customName)
    {
        var cloneDeck = new Deck
        {
            Name = string.IsNullOrEmpty(customName) ? originDeck.Name + " (Clone)" : customName,
            Description = originDeck.Description,
            Type = originDeck.Type,
            IsPublic = false,
            UserId = userId,
            ParentDeckId = originDeck.Id,
            CreatedAt = DateTime.UtcNow,
            DeckTags = originDeck.DeckTags.Select(dt => new DeckTag { TagName = dt.TagName }).ToList()
        };

        await _context.Decks.AddAsync(cloneDeck);
        return cloneDeck;
    }

    public async Task<IEnumerable<Deck>> GetByUserId(int userId)
    {
        return await _context.Decks.Where(d => d.UserId == userId).ToListAsync();
    }

    public async Task<IEnumerable<Deck>> GetMyDecksByFilterAsync(QueryDTO<GetMyDecksRequest> request)
    {
        var qr = _context.Decks.Include(d => d.User).Include(d => d.DeckTags)
            .Where(d => d.UserId == request.UserId)
            .AsNoTracking().AsQueryable();

        if(request.Query != null)
        {
            if (!string.IsNullOrEmpty(request.Query.Keyword))
                qr = qr.Where(d => d.Name.Contains(request.Query.Keyword));

            if(Enum.TryParse<DeckType>(request.Query.Type, out var deckType))
                qr = qr.Where(d => d.Type == deckType);

            request.Total = await qr.CountAsync();
            qr = qr.Skip((request.Query.Page - 1) * request.Query.PageSize)
                .Take(request.Query.PageSize);
        }

        return await qr.ToListAsync();
    }

    public async Task<IEnumerable<Deck>> GetPublicByFilterAsync(QueryDTO<SearchDeckRequest> request)
    {
        var qr = _context.Decks.Include(d => d.User).Include(d => d.DeckTags)
            .Where(d => d.IsPublic).AsNoTracking().AsQueryable();

        if(request.Query != null)
        {
            if (!string.IsNullOrEmpty(request.Query.Keyword))
                qr = qr.Where(d => d.Name.Contains(request.Query.Keyword));

            if(Enum.TryParse<DeckType>(request.Query.Type, out var deckType))
                qr = qr.Where(d => d.Type == deckType);

            if (request.Query.Tags != null && request.Query.Tags.Any())
                qr = qr.Where(d => d.DeckTags.Any(dt => request.Query.Tags.Contains(dt.TagName)));

            request.Total = await qr.CountAsync();
            qr = qr.Skip((request.Query.Page - 1) * request.Query.PageSize)
                .Take(request.Query.PageSize);
        }

        return await qr.ToListAsync();
    }

    public async Task<Deck?> GetPublicByIdAsync(int deckId)
    {
        return await _context.Decks
            .Include(d => d.User)
            .Include(d => d.DeckTags)
            .FirstOrDefaultAsync(d => d.Id == deckId && d.IsPublic);
    }
}
