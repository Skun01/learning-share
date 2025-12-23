using Application.DTOs.Deck;
using Application.DTOs.User;
using Application.IRepositories;
using Application.IServices;
using Domain.Constants;

namespace Application.Services;

public class DeckService : IDeckService
{
    private readonly IUnitOfWork _unitOfWork;

    public DeckService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<DeckSummaryDTO>> GetMyDecksAsync(int userId)
    {   
        // 1. Get user's decks
        var decks = await _unitOfWork.Decks.GetByUserId(userId);
        if(!decks.Any())
            return new List<DeckSummaryDTO>();

        var deckIds = decks.Select(d => d.Id).ToList();

        // 2. Load all related data
        var allCards = await _unitOfWork.Cards.GetByListDeckId(deckIds);
        var allDeckTags = await _unitOfWork.DeckTags.FindAsync(dt => deckIds.Contains(dt.DeckId));
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        
        // 3. Load progress for all cards (filtered by user)
        var cardIds = allCards.Select(c => c.Id).ToList();
        var allProgresses = (await _unitOfWork.UserCardProgresses.GetByListCardId(cardIds))
            .Where(p => p.UserId == userId)  // Filter by current user
            .ToList();

        // 4. Group tags by deck
        var tagsByDeck = allDeckTags
            .GroupBy(dt => dt.DeckId)
            .ToDictionary(g => g.Key, g => g.Select(dt => dt.TagName).ToList());

        // 5. Map to DTOs with stats calculation
        var result = decks.Select(deck =>
        {
            var cardsInDeck = allCards.Where(c => c.DeckId == deck.Id);
            var cardsIdsInDeck = cardsInDeck.Select(c => c.Id).ToList();
            var progresses = allProgresses.Where(p => cardsIdsInDeck.Contains(p.CardId));

            // tính toán các giá trị
            int totalCards = cardsInDeck.Count();
            int learned = progresses.Count();
            int due = progresses.Count(p => p.NextReviewDate <= DateTime.UtcNow);
            double progressPercent = totalCards == 0 ? 0 : Math.Round((double)learned / totalCards * 100, 1);

            return new DeckSummaryDTO
            {
                Id = deck.Id,
                Name = deck.Name,
                Description = deck.Description,
                Type = deck.Type.ToString(),
                Author = new AuthorDTO
                {
                    Id = user.Id,
                    Name = user.Username,
                    AvatarUrl = user.AvatarUrl
                },
                Stats = new DeckStatsDTO
                {
                    TotalCards = totalCards,
                    Downloads = deck.Downloads,
                    Learned = learned,
                    Progress = progressPercent,
                    CardsDue = due
                },
                Tags = tagsByDeck.ContainsKey(deck.Id) 
                    ? tagsByDeck[deck.Id] 
                    : new List<string>(),
                IsPublic = deck.IsPublic,
                SourceDeckId = deck.ParentDeckId,
                CreatedAt = deck.CreatedAt
            };
        }).ToList();

        return result.OrderByDescending(d => d.Stats.CardsDue)
            .ThenByDescending(d => d.CreatedAt);
    }

    public async Task<DeckDetailDTO> CreateDeckAsync(int userId, CreateDeckRequest request)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
            throw new ApplicationException(MessageConstant.CommonMessage.NOT_FOUND);

        if (!Enum.TryParse<Domain.Enums.DeckType>(request.Type, true, out var deckType))
            throw new ApplicationException(MessageConstant.DeckMessage.INVALID_DECK_TYPE);

        if (request.ParentDeckId.HasValue)
        {
            var parentDeck = await _unitOfWork.Decks.GetByIdAsync(request.ParentDeckId.Value);
            if (parentDeck == null)
                throw new ApplicationException(MessageConstant.DeckMessage.PARENT_DECK_NOT_FOUND);
            
            if (parentDeck.UserId != userId)
                throw new ApplicationException(MessageConstant.DeckMessage.PARENT_DECK_PERMISSION_DENIED);
        }

        var newDeck = new Domain.Entities.Deck
        {
            UserId = userId,
            Name = request.Name,
            Description = request.Description,
            Type = deckType,
            IsPublic = request.IsPublic,
            ParentDeckId = request.ParentDeckId,
            TotalCards = 0,
            Downloads = 0
        };

        await _unitOfWork.Decks.AddAsync(newDeck);
        await _unitOfWork.SaveChangesAsync();

        var processedTags = new List<string>();
        if (request.Tags != null && request.Tags.Any())
        {
            var deckTags = request.Tags
                .Where(tag => !string.IsNullOrWhiteSpace(tag))
                .Select(tag => tag.Trim())
                .Distinct()
                .Select(tag => new Domain.Entities.DeckTag
                {
                    DeckId = newDeck.Id,
                    TagName = tag
                })
                .ToList();

            foreach (var deckTag in deckTags)
            {
                await _unitOfWork.DeckTags.AddAsync(deckTag);
                processedTags.Add(deckTag.TagName);
            }
            
            await _unitOfWork.SaveChangesAsync();
        }

        return new DeckDetailDTO
        {
            Id = newDeck.Id,
            Name = newDeck.Name,
            Description = newDeck.Description,
            Type = newDeck.Type.ToString(),
            IsPublic = newDeck.IsPublic,
            ParentDeckId = newDeck.ParentDeckId,
            Tags = processedTags,
            TotalCards = 0,
            Downloads = 0,
            CreatedAt = newDeck.CreatedAt
        };
    }

    public async Task<DeckDetailDTO> UpdateDeckAsync(int userId, int deckId, UpdateDeckRequest request)
    {
        var deck = await _unitOfWork.Decks.GetByIdAsync(deckId);
        if (deck == null)
            throw new ApplicationException(MessageConstant.DeckMessage.DECK_NOT_FOUND);

        if (deck.UserId != userId)
            throw new ApplicationException(MessageConstant.DeckMessage.DECK_PERMISSION_DENIED);

        deck.Name = request.Name;
        deck.Description = request.Description;
        deck.IsPublic = request.IsPublic;

        _unitOfWork.Decks.UpdateAsync(deck);

        var existingTags = await _unitOfWork.DeckTags.FindAsync(dt => dt.DeckId == deckId);
        foreach (var tag in existingTags)
        {
            _unitOfWork.DeckTags.DeleteAsync(tag);
        }

        var processedTags = new List<string>();
        if (request.Tags != null && request.Tags.Any())
        {
            var newTags = request.Tags
                .Where(tag => !string.IsNullOrWhiteSpace(tag))
                .Select(tag => tag.Trim())
                .Distinct()
                .Select(tag => new Domain.Entities.DeckTag
                {
                    DeckId = deckId,
                    TagName = tag
                })
                .ToList();

            foreach (var tag in newTags)
            {
                await _unitOfWork.DeckTags.AddAsync(tag);
                processedTags.Add(tag.TagName);
            }
        }

        await _unitOfWork.SaveChangesAsync();

        return new DeckDetailDTO
        {
            Id = deck.Id,
            Name = deck.Name,
            Description = deck.Description,
            Type = deck.Type.ToString(),
            IsPublic = deck.IsPublic,
            ParentDeckId = deck.ParentDeckId,
            Tags = processedTags,
            TotalCards = deck.TotalCards,
            Downloads = deck.Downloads,
            CreatedAt = deck.CreatedAt
        };
    }
}
