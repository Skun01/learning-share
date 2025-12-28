using Application.Common;
using Application.DTOs.Common;
using Application.DTOs.Deck;
using Application.DTOs.User;
using Application.IRepositories;
using Application.IServices;
using Domain.Constants;
using Domain.Enums;

namespace Application.Services;

public class DeckService : IDeckService
{
    private readonly IUnitOfWork _unitOfWork;

    public DeckService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<DeckSummaryDTO>> GetMyDecksByFilterAsync(QueryDTO<GetMyDecksRequest> request)
    {   
        var decks = await _unitOfWork.Decks.GetMyDecksByFilterAsync(request);

        var deckIds = decks.Select(d => d.Id).ToList();

        // Load related data
        var allCards = await _unitOfWork.Cards.GetByListDeckId(deckIds);
        var allDeckTags = await _unitOfWork.DeckTags.FindAsync(dt => deckIds.Contains(dt.DeckId));
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        
        var cardIds = allCards.Select(c => c.Id).ToList();
        var allProgresses = await _unitOfWork.UserCardProgresses.GetByListCardId(cardIds, request.UserId);

        var tagsByDeck = allDeckTags
            .GroupBy(dt => dt.DeckId)
            .ToDictionary(g => g.Key, g => g.Select(dt => dt.TagName).ToList());

        // Map to DTOs with stats
        var result = decks.Select(deck =>
        {
            var cardsInDeck = allCards.Where(c => c.DeckId == deck.Id);
            var cardsIdsInDeck = cardsInDeck.Select(c => c.Id).ToList();
            var progresses = allProgresses.Where(p => cardsIdsInDeck.Contains(p.CardId));

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
                    Id = user!.Id,
                    Name = user.Username,
                    AvatarUrl = user.AvatarMedia != null ? "/" + user.AvatarMedia.FilePath : null
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

        return result;
    }

    public async Task<DeckDetailDTO> GetDeckByIdAsync(int userId, int deckId)
    {
        var deck = await _unitOfWork.Decks.GetByIdAsync(deckId);
        if (deck == null)
            throw new ApplicationException(MessageConstant.DeckMessage.DECK_NOT_FOUND);
        
        if (deck.UserId != userId && !deck.IsPublic)
            throw new ApplicationException(MessageConstant.DeckMessage.DECK_PERMISSION_DENIED);

        var tags = await _unitOfWork.DeckTags.FindAsync(dt => dt.DeckId == deckId);
        
        return new DeckDetailDTO
        {
            Id = deck.Id,
            Name = deck.Name,
            Description = deck.Description,
            Type = deck.Type.ToString(),
            IsPublic = deck.IsPublic,
            ParentDeckId = deck.ParentDeckId,
            Tags = tags.Select(t => t.TagName).ToList(),
            TotalCards = deck.TotalCards,
            Downloads = deck.Downloads,
            CreatedAt = deck.CreatedAt
        };
    }

    public async Task<DeckStatisticsDTO> GetDeckStatisticsAsync(int userId)
    {
        var decks = await _unitOfWork.Decks.GetByUserId(userId);
        var deckIds = decks.Select(d => d.Id).ToList();
        
        if (!deckIds.Any())
        {
            return new DeckStatisticsDTO
            {
                TotalDecks = 0,
                TotalCards = 0,
                TotalLearned = 0,
                TotalDue = 0,
                OverallProgress = 0,
                PublicDecks = 0,
                PrivateDecks = 0,
                DecksByType = new Dictionary<string, int>()
            };
        }

        var allCards = await _unitOfWork.Cards.GetByListDeckId(deckIds);
        var cardIds = allCards.Select(c => c.Id).ToList();
        var progresses = await _unitOfWork.UserCardProgresses.GetByListCardId(cardIds, userId);
        
        return new DeckStatisticsDTO
        {
            TotalDecks = decks.Count(),
            TotalCards = allCards.Count(),
            TotalLearned = progresses.Count(),
            TotalDue = progresses.Count(p => p.NextReviewDate <= DateTime.UtcNow),
            OverallProgress = allCards.Any() 
                ? Math.Round((double)progresses.Count() / allCards.Count() * 100, 1) 
                : 0,
            PublicDecks = decks.Count(d => d.IsPublic),
            PrivateDecks = decks.Count(d => !d.IsPublic),
            DecksByType = decks.GroupBy(d => d.Type.ToString())
                .ToDictionary(g => g.Key, g => g.Count())
        };
    }

    public async Task<DeckDetailDTO> CreateDeckAsync(int userId, CreateDeckRequest request)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
            throw new ApplicationException(MessageConstant.CommonMessage.NOT_FOUND);

        if (!Enum.TryParse<DeckType>(request.Type, true, out var deckType))
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

    public async Task<DeckDetailDTO> TogglePublishAsync(int userId, int deckId)
    {
        var deck = await _unitOfWork.Decks.GetByIdAsync(deckId);
        if (deck == null)
            throw new ApplicationException(MessageConstant.DeckMessage.DECK_NOT_FOUND);
        
        if (deck.UserId != userId)
            throw new ApplicationException(MessageConstant.DeckMessage.DECK_PERMISSION_DENIED);

        deck.IsPublic = !deck.IsPublic;
        _unitOfWork.Decks.UpdateAsync(deck);
        await _unitOfWork.SaveChangesAsync();

        return await GetDeckByIdAsync(userId, deckId);
    }

    public async Task<bool> DeleteDeckAsync(int userId, int deckId)
    {
        var deck = await _unitOfWork.Decks.GetByIdAsync(deckId);
        if (deck == null)
            throw new ApplicationException(MessageConstant.DeckMessage.DECK_NOT_FOUND);
        
        if (deck.UserId != userId)
            throw new ApplicationException(MessageConstant.DeckMessage.DECK_PERMISSION_DENIED);

        // Xử lý child decks (set ParentDeckId = null)
        var childDecks = await _unitOfWork.Decks.FindAsync(d => d.ParentDeckId == deckId);
        foreach (var childDeck in childDecks)
        {
            childDeck.ParentDeckId = null;
            _unitOfWork.Decks.UpdateAsync(childDeck);
        }

        // Lấy tất cả cards trong deck
        var cards = await _unitOfWork.Cards.FindAsync(c => c.DeckId == deckId);
        var cardIds = cards.Select(c => c.Id).ToList();

        // Xóa theo thứ tự: Progress -> Examples -> GrammarDetails -> Cards -> Tags -> Deck
        var progresses = await _unitOfWork.UserCardProgresses.GetByListCardId(cardIds, userId);
        foreach (var progress in progresses)
        {
            _unitOfWork.UserCardProgresses.DeleteAsync(progress);
        }

        var examples = await _unitOfWork.CardExamples.FindAsync(e => cardIds.Contains(e.CardId));
        foreach (var example in examples)
        {
            _unitOfWork.CardExamples.DeleteAsync(example);
        }

        var grammarDetails = await _unitOfWork.GrammarDetails.FindAsync(g => cardIds.Contains(g.CardId));
        foreach (var detail in grammarDetails)
        {
            _unitOfWork.GrammarDetails.DeleteAsync(detail);
        }

        foreach (var card in cards)
        {
            _unitOfWork.Cards.DeleteAsync(card);
        }

        var tags = await _unitOfWork.DeckTags.FindAsync(dt => dt.DeckId == deckId);
        foreach (var tag in tags)
        {
            _unitOfWork.DeckTags.DeleteAsync(tag);
        }

        _unitOfWork.Decks.DeleteAsync(deck);

        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ResetDeckProgressAsync(int userId, int deckId)
    {
        var deck = await _unitOfWork.Decks.GetByIdAsync(deckId);
        if (deck == null)
            throw new ApplicationException(MessageConstant.DeckMessage.DECK_NOT_FOUND);
        
        if (deck.UserId != userId)
            throw new ApplicationException(MessageConstant.DeckMessage.DECK_PERMISSION_DENIED);

        // Lấy tất cả cards trong deck
        var cards = await _unitOfWork.Cards.FindAsync(c => c.DeckId == deckId);
        var cardIds = cards.Select(c => c.Id).ToList();

        // Xóa progress của user hiện tại
        var progresses = await _unitOfWork.UserCardProgresses.GetByListCardId(cardIds, userId);
        
        foreach (var progress in progresses)
        {
            _unitOfWork.UserCardProgresses.DeleteAsync(progress);
        }

        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
