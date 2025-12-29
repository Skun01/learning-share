using Application.DTOs.Common;
using Application.DTOs.Study;
using Application.IRepositories;
using Application.IServices;
using Application.Mappings;

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
        
        int newCards = 0;
        if (deckId.HasValue)
        {
            newCards = await _unitOfWork.UserCardProgresses.CountNewCardsAsync(userId, deckId.Value);
        }
        else
        {
            var userDecks = await _unitOfWork.Decks.GetByUserId(userId);
            foreach (var deck in userDecks)
            {
                newCards += await _unitOfWork.UserCardProgresses.CountNewCardsAsync(userId, deck.Id);
            }
        }

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
}
