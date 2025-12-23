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
}
