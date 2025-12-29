using Application.DTOs.Card;
using Application.DTOs.Study;
using Domain.Entities;

namespace Application.Mappings;

public static class StudyMapping
{
    public static StudyCardDTO ToStudyCardDTO(this UserCardProgress progress)
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

    public static StudyCardDTO ToStudyCardDTO(this Card card)
    {
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
            SRSLevel = 0,
            GhostLevel = 0,
            Streak = 0,
            LastReviewedDate = null,
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
