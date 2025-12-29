using Application.DTOs.Card;
using Application.DTOs.Common;
using Application.DTOs.Study;
using Application.IRepositories;
using Application.IServices;

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

        return dueProgress.Select(p => MapToStudyCardDTO(p));
    }

    private StudyCardDTO MapToStudyCardDTO(Domain.Entities.UserCardProgress progress)
    {
        var card = progress.Card;
        return new StudyCardDTO
        {
            CardId = card.Id,
            DeckId = card.DeckId,
            DeckName = card.Deck?.Name ?? string.Empty,
            Type = card.Type.ToString(),
            Term = card.Term,
            Meaning = card.Meaning,
            Synonyms = card.Synonyms,
            ImageMediaId = card.ImageMediaId,
            ImageUrl = card.ImageMedia != null ? "/" + card.ImageMedia.FilePath : null,
            Note = card.Note,
            SRSLevel = (int)progress.SRSLevel,
            GhostLevel = progress.GhostLevel,
            Streak = progress.Streak,
            LastReviewedDate = progress.LastReviewedDate,
            GrammarDetails = card.GrammarDetails != null ? new GrammarDetailsDTO
            {
                Structure = card.GrammarDetails.Structure,
                Explanation = card.GrammarDetails.Explanation,
                Caution = card.GrammarDetails.Caution,
                Level = card.GrammarDetails.Level.ToString()
            } : null,
            Examples = card.Examples.Select(e => new CardExampleDTO
            {
                Id = e.Id,
                SentenceJapanese = e.SentenceJapanese,
                SentenceMeaning = e.SentenceMeaning,
                ClozePart = e.ClozePart,
                AlternativeAnswers = e.AlternativeAnswers,
                AudioMediaId = e.AudioMediaId,
                AudioUrl = e.AudioMedia != null ? "/" + e.AudioMedia.FilePath : null
            }).ToList()
        };
    }
}
