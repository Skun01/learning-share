using Application.DTOs.Common;
using Application.DTOs.Study;
using Application.IRepositories;
using Application.IServices;
using Application.Mappings;

namespace Application.Services;

public class SessionService : ISessionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStudyService _studyService;

    public SessionService(IUnitOfWork unitOfWork, IStudyService studyService)
    {
        _unitOfWork = unitOfWork;
        _studyService = studyService;
    }

    public async Task<SessionDTO> StartSessionAsync(RequestDTO<StartSessionRequest> request)
    {
        var userId = request.UserId;
        var mode = request.Request?.Mode?.ToLower() ?? "review";
        var limit = request.Request?.Limit > 0 ? request.Request.Limit : 10;
        var deckId = request.Request?.DeckId;

        List<StudyCardDTO> cards = new();

        if (mode == "review" || mode == "mixed")
        {
            var reviewRequest = new QueryDTO<GetAvailableReviewsRequest>
            {
                UserId = userId,
                Query = new GetAvailableReviewsRequest { DeckId = deckId, Limit = limit }
            };
            var reviews = await _studyService.GetAvailableReviewsAsync(reviewRequest);
            cards.AddRange(reviews);
        }

        if ((mode == "lesson" || mode == "mixed") && cards.Count < limit && deckId.HasValue)
        {
            var lessonRequest = new QueryDTO<GetNewLessonsRequest>
            {
                UserId = userId,
                Query = new GetNewLessonsRequest { DeckId = deckId.Value, Limit = limit - cards.Count }
            };
            var lessons = await _studyService.GetNewLessonsAsync(lessonRequest);
            cards.AddRange(lessons);
        }

        var sessionId = Guid.NewGuid().ToString("N")[..12];
        var queue = cards.Skip(1).Select(c => c.CardId).ToList();

        return new SessionDTO
        {
            SessionId = sessionId,
            Mode = mode,
            TotalCards = cards.Count,
            CurrentIndex = 0,
            Correct = 0,
            Incorrect = 0,
            StartedAt = DateTime.UtcNow,
            CurrentCard = cards.FirstOrDefault(),
            Queue = queue
        };
    }

    public async Task<SessionDTO> SubmitSessionAsync(RequestDTO<SubmitSessionRequest> request)
    {
        var userId = request.UserId;
        var req = request.Request!;
        
        var submitRequest = new SubmitReviewRequest { IsCorrect = req.IsCorrect };
        await _studyService.SubmitReviewAsync(userId, req.CardId, submitRequest);

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

        return new SessionDTO
        {
            SessionId = req.SessionId,
            Mode = string.Empty,
            TotalCards = correct + incorrect + remainingQueue.Count + (nextCard != null ? 1 : 0),
            CurrentIndex = correct + incorrect,
            Correct = correct,
            Incorrect = incorrect,
            StartedAt = req.StartedAt,
            CurrentCard = nextCard,
            Queue = remainingQueue
        };
    }

    public async Task<SessionSummaryDTO> EndSessionAsync(RequestDTO<EndSessionRequest> request)
    {
        var req = request.Request!;
        var endedAt = DateTime.UtcNow;
        var total = req.Correct + req.Incorrect;
        var rate = total > 0 ? (double)req.Correct / total : 0;
        var timeSpent = (int)(endedAt - req.StartedAt).TotalSeconds;

        return await Task.FromResult(new SessionSummaryDTO
        {
            SessionId = req.SessionId,
            TotalReviewed = total,
            Correct = req.Correct,
            Incorrect = req.Incorrect,
            AccuracyRate = Math.Round(rate, 4),
            TimeSpentSeconds = timeSpent,
            StartedAt = req.StartedAt,
            EndedAt = endedAt
        });
    }
}
