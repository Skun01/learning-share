using Application.DTOs.Common;
using Application.DTOs.Study;
using Application.IRepositories;
using Application.IServices;
using Application.Mappings;
using Domain.Constants;

namespace Application.Services;

public class CramService : ICramService
{
    private readonly IUnitOfWork _unitOfWork;

    public CramService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CramSessionDTO> StartCramAsync(RequestDTO<StartCramRequest> request)
    {
        var userId = request.UserId;
        var req = request.Request!;
        
        var type = req.Type?.ToLower() ?? CramType.All;
        var limit = req.Limit > 0 ? req.Limit : 20;
        var deckIds = req.DeckIds ?? new List<int>();

        var progresses = await _unitOfWork.UserCardProgresses.GetCramCardsAsync(
            userId, deckIds, type, req.SpecificLevel, limit);

        var cards = progresses.Select(p => p.ToStudyCardDTO()).ToList();
        var sessionId = Guid.NewGuid().ToString("N")[..12];
        var queue = cards.Skip(1).Select(c => c.CardId).ToList();

        return new CramSessionDTO
        {
            SessionId = sessionId,
            Type = type,
            TotalCards = cards.Count,
            CurrentIndex = 0,
            Correct = 0,
            Incorrect = 0,
            StartedAt = DateTime.UtcNow,
            CurrentCard = cards.FirstOrDefault(),
            Queue = queue
        };
    }

    public async Task<CramSessionDTO> SubmitCramAsync(RequestDTO<SubmitCramRequest> request)
    {
        var req = request.Request!;

        var correct = req.Correct + (req.IsCorrect ? 1 : 0);
        var incorrect = req.Incorrect + (req.IsCorrect ? 0 : 1);

        StudyCardDTO? nextCard = null;
        var remainingQueue = req.RemainingQueue.ToList();

        if (remainingQueue.Count > 0)
        {
            var nextCardId = remainingQueue[0];
            remainingQueue.RemoveAt(0);

            var card = await _unitOfWork.Cards.GetByIdWithDetailsAsync(nextCardId);
            if (card != null)
            {
                nextCard = card.ToStudyCardDTO();
            }
        }

        return new CramSessionDTO
        {
            SessionId = req.SessionId,
            Type = string.Empty,
            TotalCards = correct + incorrect + remainingQueue.Count + (nextCard != null ? 1 : 0),
            CurrentIndex = correct + incorrect,
            Correct = correct,
            Incorrect = incorrect,
            StartedAt = req.StartedAt,
            CurrentCard = nextCard,
            Queue = remainingQueue
        };
    }
}
