using Application.DTOs.Common;
using Application.DTOs.Study;
using Application.IRepositories;
using Application.IServices;
using Application.Mappings;
using Domain.Constants;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services;

public class StudyService : IStudyService
{
    private readonly IUnitOfWork _unitOfWork;

    public StudyService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<StudyCountDTO> GetStudyCountAsync(QueryDTO<GetStudyCountRequest> request)
    {
        var userId = request.UserId;
        var deckId = request.Query?.DeckId;

        var reviews = await _unitOfWork.UserCardProgresses.CountDueReviewsAsync(userId, deckId);
        var ghosts = await _unitOfWork.UserCardProgresses.CountGhostsAsync(userId, deckId);

        int newCards = deckId.HasValue
            ? await _unitOfWork.UserCardProgresses.CountNewCardsAsync(userId, deckId.Value)
            : await _unitOfWork.UserCardProgresses.CountAllNewCardsAsync(userId);

        return new StudyCountDTO
        {
            Reviews = reviews,
            New = newCards,
            Ghosts = ghosts
        };
    }

    public async Task<IEnumerable<StudyCardDTO>> GetAvailableReviewsAsync(QueryDTO<GetAvailableReviewsRequest> request)
    {
        var userId = request.UserId;
        var deckId = request.Query?.DeckId;
        var limit = request.Query?.Limit ?? 20;

        var dueProgress = await _unitOfWork.UserCardProgresses.GetDueReviewsAsync(userId, deckId, limit);

        return dueProgress.Select(p => p.ToStudyCardDTO());
    }

    public async Task<IEnumerable<StudyCardDTO>> GetNewLessonsAsync(QueryDTO<GetNewLessonsRequest> request)
    {
        var newCards = await _unitOfWork.Cards.GetNewCardsAsync(request);
        return newCards.Select(c => c.ToStudyCardDTO());
    }

    public async Task<SubmitReviewResponse> SubmitReviewAsync(int userId, int cardId, SubmitReviewRequest request)
    {
        var isCorrect = request.IsCorrect;

        var card = await _unitOfWork.Cards.GetByIdAsync(cardId);
        if (card == null)
            throw new ApplicationException(MessageConstant.SrsMessage.CARD_NOT_FOUND);

        var deck = await _unitOfWork.Decks.GetByIdAsync(card.DeckId);
        if (deck == null || deck.UserId != userId)
            throw new ApplicationException(MessageConstant.SrsMessage.DECK_PERMISSION_DENIED);

        var progress = await _unitOfWork.UserCardProgresses.GetByUserAndCardAsync(userId, cardId);
        bool isNewCard = progress == null;

        if (isNewCard)
        {
            progress = new UserCardProgress
            {
                UserId = userId,
                CardId = cardId,
                SRSLevel = SRSLevel.New,
                GhostLevel = 0,
                Streak = 0
            };
            await _unitOfWork.UserCardProgresses.AddAsync(progress);
        }

        var oldLevel = (int)progress!.SRSLevel;
        SRSLevel newLevel;
        DateTime nextReviewDate;
        string message;

        if (isCorrect)
        {
            newLevel = progress.SRSLevel < SRSLevel.Burned 
                ? (SRSLevel)((int)progress.SRSLevel + 1) 
                : SRSLevel.Burned;
            
            nextReviewDate = SRSIntervals.GetNextReviewDate(newLevel);
            
            if (progress.GhostLevel > 0)
                progress.GhostLevel--;
            
            progress.Streak++;
            
            message = newLevel == SRSLevel.Burned 
                ? MessageConstant.SrsMessage.REVIEW_BURNED 
                : MessageConstant.SrsMessage.REVIEW_CORRECT;
        }
        else
        {
            int calculatedLevel = Math.Max((int)progress.SRSLevel / 2, 1);
            newLevel = (SRSLevel)calculatedLevel;
            
            nextReviewDate = DateTime.UtcNow.AddHours(1);
            
            var userSettings = await _unitOfWork.UserSettings.GetByUserIdAsync(userId);
            if (userSettings?.EnableGhostMode == true)
                progress.GhostLevel++;
            
            progress.Streak = 0;
            
            message = MessageConstant.SrsMessage.REVIEW_INCORRECT;
        }

        progress.SRSLevel = newLevel;
        progress.NextReviewDate = nextReviewDate;
        progress.LastReviewedDate = DateTime.UtcNow;

        if (!isNewCard)
            _unitOfWork.UserCardProgresses.UpdateAsync(progress);

        var studyLog = new StudyLog
        {
            UserId = userId,
            CardId = cardId,
            ReviewDate = DateTime.UtcNow,
            IsCorrect = isCorrect
        };
        await _unitOfWork.StudyLogs.AddAsync(studyLog);

        await _unitOfWork.SaveChangesAsync();

        return new SubmitReviewResponse
        {
            CardId = cardId,
            OldLevel = oldLevel,
            NewLevel = (int)newLevel,
            NextReviewDate = nextReviewDate,
            GhostLevel = progress.GhostLevel,
            Streak = progress.Streak,
            IsCorrect = isCorrect,
            Message = message
        };
    }

    public async Task<IEnumerable<HeatmapDataDTO>> GetHeatmapAsync(QueryDTO<GetHeatmapRequest> request)
    {
        var userId = request.UserId;
        var year = request.Query?.Year ?? DateTime.UtcNow.Year;

        var startDate = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var endDate = new DateTime(year, 12, 31, 23, 59, 59, DateTimeKind.Utc);

        var logs = await _unitOfWork.StudyLogs.GetByDateRangeAsync(userId, startDate, endDate);

        return logs
            .GroupBy(l => l.ReviewDate.Date)
            .Select(g => new HeatmapDataDTO
            {
                Date = g.Key.ToString("yyyy-MM-dd"),
                Count = g.Count()
            })
            .OrderBy(h => h.Date)
            .ToList();
    }

    public async Task<IEnumerable<ForecastDTO>> GetForecastAsync(QueryDTO<GetForecastRequest> request)
    {
        var userId = request.UserId;
        var days = request.Query?.Days ?? 7;

        var forecast = await _unitOfWork.UserCardProgresses.GetForecastAsync(userId, days);

        return forecast
            .OrderBy(kvp => kvp.Key)
            .Select(kvp => new ForecastDTO
            {
                Date = kvp.Key.ToString("yyyy-MM-dd"),
                Count = kvp.Value
            })
            .ToList();
    }

    public async Task<AccuracyDTO> GetAccuracyAsync(QueryDTO<GetAccuracyRequest> request)
    {
        var userId = request.UserId;
        var period = request.Query?.Period?.ToLower() ?? "week";

        DateTime startDate = period switch
        {
            "day" => DateTime.UtcNow.AddDays(-1),
            "week" => DateTime.UtcNow.AddDays(-7),
            "month" => DateTime.UtcNow.AddMonths(-1),
            "year" => DateTime.UtcNow.AddYears(-1),
            _ => DateTime.UtcNow.AddDays(-7)
        };

        var logs = await _unitOfWork.StudyLogs.GetByDateRangeAsync(userId, startDate, DateTime.UtcNow);
        var logList = logs.ToList();

        var correct = logList.Count(l => l.IsCorrect);
        var incorrect = logList.Count(l => !l.IsCorrect);
        var total = logList.Count;
        var rate = total > 0 ? (double)correct / total : 0;

        return new AccuracyDTO
        {
            Correct = correct,
            Incorrect = incorrect,
            Total = total,
            Rate = Math.Round(rate, 4)
        };
    }

    public async Task<LevelDistributionDTO> GetDistributionAsync(QueryDTO<GetDistributionRequest> request)
    {
        var userId = request.UserId;
        var deckId = request.Query?.DeckId;

        var distribution = await _unitOfWork.UserCardProgresses.GetLevelDistributionAsync(userId, deckId);

        // Đảm bảo tất cả levels từ 0-12 đều có
        for (int i = 0; i <= 12; i++)
        {
            if (!distribution.ContainsKey(i))
                distribution[i] = 0;
        }

        var total = distribution.Values.Sum();
        var burnedCount = distribution.GetValueOrDefault(12, 0);

        return new LevelDistributionDTO
        {
            Distribution = distribution.OrderBy(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
            TotalCards = total,
            LearnedCards = total - distribution.GetValueOrDefault(0, 0),
            BurnedCards = burnedCount
        };
    }
}
