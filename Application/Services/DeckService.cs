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
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if(user == null)
            throw new ApplicationException(MessageConstant.CommonMessage.NOT_FOUND);

        var decks = await _unitOfWork.Decks.GetByUserId(userId);
        if(!decks.Any())
            return new List<DeckSummaryDTO>();

        var decksId = decks.Select(d => d.Id).ToList();
        var allCards = await _unitOfWork.Cards.GetByListDeckId(decksId);
        
        var cardIds = allCards.Select(c => c.Id).ToList();
        var allProgresses = await _unitOfWork.UserCardProgresses.GetByListCardId(cardIds);

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
                    Downloads = 0,
                    Learned = learned,
                    Progress = progressPercent,
                    CardsDue = due
                },
                Tags = new List<string>(),
                IsPublic = deck.IsPublic,
                SourceDeckId = null,
                CreatedAt = deck.CreatedAt
            };
        }).ToList();

        return result.OrderByDescending(d => d.Stats.CardsDue)
            .ThenByDescending(d => d.CreatedAt);
    }

    public async Task<DeckDetailDTO> CreateDeckAsync(int userId, CreateDeckRequest request)
    {
        // Validate user exists
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
            throw new ApplicationException(MessageConstant.CommonMessage.NOT_FOUND);

        // Validate DeckType
        if (!Enum.TryParse<Domain.Enums.DeckType>(request.Type, true, out var deckType))
            throw new ApplicationException(MessageConstant.DeckMessage.INVALID_DECK_TYPE);

        // Validate ParentDeckId if provided
        if (request.ParentDeckId.HasValue)
        {
            var parentDeck = await _unitOfWork.Decks.GetByIdAsync(request.ParentDeckId.Value);
            if (parentDeck == null)
                throw new ApplicationException(MessageConstant.DeckMessage.PARENT_DECK_NOT_FOUND);
            
            if (parentDeck.UserId != userId)
                throw new ApplicationException(MessageConstant.DeckMessage.PARENT_DECK_PERMISSION_DENIED);
        }

        // Create Deck entity
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

        // Create DeckTags if tags provided
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

        // Return DeckDetailDTO
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
}
