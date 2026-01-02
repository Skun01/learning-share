using Application.DTOs.Card;
using Application.DTOs.Common;
using Application.IRepositories;
using Application.IServices;
using Domain.Constants;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services;

public class CardService : ICardService
{
    private readonly IUnitOfWork _unitOfWork;

    public CardService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<CardSummaryDTO>> GetCardsByDeckIdAsync(int userId, int deckId)
    {
        // Validate deck ownership
        var deck = await _unitOfWork.Decks.GetByIdAsync(deckId);
        if (deck == null)
            throw new ApplicationException(MessageConstant.DeckMessage.DECK_NOT_FOUND);
        
        if (deck.UserId != userId && !deck.IsPublic)
            throw new ApplicationException(MessageConstant.DeckMessage.DECK_PERMISSION_DENIED);

        var cards = await _unitOfWork.Cards.GetByDeckIdAsync(deckId);
        
        return cards.Select(c => new CardSummaryDTO
        {
            Id = c.Id,
            Type = c.Type.ToString(),
            Term = c.Term,
            Meaning = c.Meaning,
            ImageMediaId = c.ImageMediaId,
            ImageUrl = c.ImageMedia != null ? "/" + c.ImageMedia.FilePath : null,
            HasExamples = c.Examples.Any(),
            HasGrammarDetails = c.GrammarDetails != null,
            HasVocabularyDetails = c.VocabularyDetails != null
        });
    }

    public async Task<IEnumerable<CardSummaryDTO>> GetCardsWithFilterAsync(int deckId, QueryDTO<GetCardsRequest> request)
    {
        var deck = await _unitOfWork.Decks.GetByIdAsync(deckId);
        if (deck == null)
            throw new ApplicationException(MessageConstant.DeckMessage.DECK_NOT_FOUND);
        
        if (deck.UserId != request.UserId && !deck.IsPublic)
            throw new ApplicationException(MessageConstant.DeckMessage.DECK_PERMISSION_DENIED);

        var allCards = await _unitOfWork.Cards.GetByDeckIdAsync(deckId);
        var query = allCards.AsQueryable();

        var filterRequest = request.Query;

        // Filter by type
        if (filterRequest != null && !string.IsNullOrEmpty(filterRequest.Type) && Enum.TryParse<CardType>(filterRequest.Type, true, out var cardType))
        {
            query = query.Where(c => c.Type == cardType);
        }

        // Search by keyword (term or meaning)
        if (filterRequest != null && !string.IsNullOrEmpty(filterRequest.Keyword))
        {
            var keyword = filterRequest.Keyword.ToLower();
            query = query.Where(c => c.Term.ToLower().Contains(keyword) || 
                                     c.Meaning.ToLower().Contains(keyword));
        }

        // Set total for metadata
        request.Total = query.Count();

        // Pagination
        var page = filterRequest?.Page > 0 ? filterRequest.Page : 1;
        var pageSize = filterRequest?.PageSize > 0 ? filterRequest.PageSize : 20;
        var cards = query.Skip((page - 1) * pageSize).Take(pageSize);

        return cards.Select(c => new CardSummaryDTO
        {
            Id = c.Id,
            Type = c.Type.ToString(),
            Term = c.Term,
            Meaning = c.Meaning,
            ImageMediaId = c.ImageMediaId,
            ImageUrl = c.ImageMedia != null ? "/" + c.ImageMedia.FilePath : null,
            HasExamples = c.Examples.Any(),
            HasGrammarDetails = c.GrammarDetails != null,
            HasVocabularyDetails = c.VocabularyDetails != null
        });
    }

    public async Task<CardDetailDTO> GetCardByIdAsync(int userId, int deckId, int cardId)
    {
        var deck = await _unitOfWork.Decks.GetByIdAsync(deckId);
        if (deck == null)
            throw new ApplicationException(MessageConstant.DeckMessage.DECK_NOT_FOUND);
        
        if (deck.UserId != userId && !deck.IsPublic)
            throw new ApplicationException(MessageConstant.DeckMessage.DECK_PERMISSION_DENIED);

        var card = await _unitOfWork.Cards.GetByIdWithDetailsAsync(cardId);
        if (card == null || card.DeckId != deckId)
            throw new ApplicationException(MessageConstant.CardMessage.CARD_NOT_FOUND);
        
        return MapToDetailDTO(card);
    }

    public async Task<CardDetailDTO> CreateCardAsync(int userId, int deckId, CreateCardRequest request)
    {
        var deck = await _unitOfWork.Decks.GetByIdAsync(deckId);
        if (deck == null)
            throw new ApplicationException(MessageConstant.DeckMessage.DECK_NOT_FOUND);
        
        if (deck.UserId != userId)
            throw new ApplicationException(MessageConstant.DeckMessage.DECK_PERMISSION_DENIED);

        // Parse CardType
        if (!Enum.TryParse<CardType>(request.Type, true, out var cardType))
            throw new ApplicationException(MessageConstant.CardMessage.CARD_INVALID_TYPE);

        // Validate card type matches deck type
        var deckType = deck.Type.ToString();
        if (cardType.ToString() != deckType)
            throw new ApplicationException(MessageConstant.CardMessage.CARD_TYPE_MISMATCH);

        // Create card entity
        var card = new Card
        {
            DeckId = deckId,
            Type = cardType,
            Term = request.Term,
            Meaning = request.Meaning,
            Synonyms = request.Synonyms,
            ImageMediaId = request.ImageMediaId,
            Note = request.Note,
            // === Thuộc tính mở rộng ===
            Difficulty = request.Difficulty,
            Priority = request.Priority,
            Tags = request.Tags,
            IsHidden = request.IsHidden,
            AudioMediaId = request.AudioMediaId,
            Hint = request.Hint
        };

        await _unitOfWork.Cards.AddAsync(card);
        await _unitOfWork.SaveChangesAsync();

        // Add grammar details if provided
        if (request.GrammarDetails != null && cardType == CardType.Grammar)
        {
            if (!Enum.TryParse<Level>(request.GrammarDetails.Level, true, out var level))
                level = Level.N5;

            var grammarDetails = new GrammarDetails
            {
                CardId = card.Id,
                Structure = request.GrammarDetails.Structure,
                Explanation = request.GrammarDetails.Explanation,
                Caution = request.GrammarDetails.Caution,
                Level = level,
                FormationRules = request.GrammarDetails.FormationRules,
                Nuance = request.GrammarDetails.Nuance,
                UsageNotes = request.GrammarDetails.UsageNotes,
                Register = request.GrammarDetails.Register
            };
            await _unitOfWork.GrammarDetails.AddAsync(grammarDetails);
        }

        // Add vocabulary details if provided
        if (request.VocabularyDetails != null && cardType == CardType.Vocabulary)
        {
            Level? jlptLevel = null;
            if (!string.IsNullOrEmpty(request.VocabularyDetails.JLPTLevel) &&
                Enum.TryParse<Level>(request.VocabularyDetails.JLPTLevel, true, out var parsedLevel))
            {
                jlptLevel = parsedLevel;
            }

            var vocabularyDetails = new VocabularyDetails
            {
                CardId = card.Id,
                Reading = request.VocabularyDetails.Reading,
                PartOfSpeech = request.VocabularyDetails.PartOfSpeech,
                Pitch = request.VocabularyDetails.Pitch,
                JLPTLevel = jlptLevel,
                Frequency = request.VocabularyDetails.Frequency,
                WaniKaniLevel = request.VocabularyDetails.WaniKaniLevel,
                Transitivity = request.VocabularyDetails.Transitivity,
                VerbGroup = request.VocabularyDetails.VerbGroup,
                AdjectiveType = request.VocabularyDetails.AdjectiveType,
                CommonCollocations = request.VocabularyDetails.CommonCollocations,
                Antonyms = request.VocabularyDetails.Antonyms,
                KanjiComponents = request.VocabularyDetails.KanjiComponents
            };
            await _unitOfWork.VocabularyDetails.AddAsync(vocabularyDetails);
        }

        // Add examples if provided
        if (request.Examples != null && request.Examples.Any())
        {
            foreach (var ex in request.Examples)
            {
                var example = new CardExample
                {
                    CardId = card.Id,
                    SentenceJapanese = ex.SentenceJapanese,
                    SentenceMeaning = ex.SentenceMeaning,
                    ClozePart = ex.ClozePart,
                    AlternativeAnswers = ex.AlternativeAnswers,
                    AudioMediaId = ex.AudioMediaId
                };
                await _unitOfWork.CardExamples.AddAsync(example);
            }
        }

        // Update deck total cards
        deck.TotalCards++;
        _unitOfWork.Decks.UpdateAsync(deck);
        
        await _unitOfWork.SaveChangesAsync();

        // Return created card with details
        var createdCard = await _unitOfWork.Cards.GetByIdWithDetailsAsync(card.Id);
        return MapToDetailDTO(createdCard!);
    }

    public async Task<CardDetailDTO> UpdateCardAsync(int userId, int deckId, int cardId, UpdateCardRequest request)
    {
        var deck = await _unitOfWork.Decks.GetByIdAsync(deckId);
        if (deck == null)
            throw new ApplicationException(MessageConstant.DeckMessage.DECK_NOT_FOUND);
        
        if (deck.UserId != userId)
            throw new ApplicationException(MessageConstant.DeckMessage.DECK_PERMISSION_DENIED);

        var card = await _unitOfWork.Cards.GetByIdWithDetailsAsync(cardId);
        if (card == null || card.DeckId != deckId)
            throw new ApplicationException(MessageConstant.CardMessage.CARD_NOT_FOUND);

        // Update only provided fields
        if (request.Type != null && Enum.TryParse<CardType>(request.Type, true, out var cardType))
            card.Type = cardType;
        if (request.Term != null)
            card.Term = request.Term;
        if (request.Meaning != null)
            card.Meaning = request.Meaning;
        if (request.Synonyms != null)
            card.Synonyms = request.Synonyms;
        if (request.ImageMediaId.HasValue)
            card.ImageMediaId = request.ImageMediaId;
        if (request.Note != null)
            card.Note = request.Note;
        // === Thuộc tính mở rộng ===
        if (request.Difficulty.HasValue)
            card.Difficulty = request.Difficulty.Value;
        if (request.Priority.HasValue)
            card.Priority = request.Priority.Value;
        if (request.Tags != null)
            card.Tags = request.Tags;
        if (request.IsHidden.HasValue)
            card.IsHidden = request.IsHidden.Value;
        if (request.AudioMediaId.HasValue)
            card.AudioMediaId = request.AudioMediaId;
        if (request.Hint != null)
            card.Hint = request.Hint;

        // Handle GrammarDetails update
        if (request.GrammarDetails != null && card.Type == CardType.Grammar)
        {
            if (card.GrammarDetails == null)
            {
                var newGrammar = new GrammarDetails
                {
                    CardId = cardId,
                    Structure = request.GrammarDetails.Structure,
                    Explanation = request.GrammarDetails.Explanation,
                    Caution = request.GrammarDetails.Caution,
                    Level = Enum.TryParse<Level>(request.GrammarDetails.Level, true, out var level) ? level : Level.N5,
                    FormationRules = request.GrammarDetails.FormationRules,
                    Nuance = request.GrammarDetails.Nuance,
                    UsageNotes = request.GrammarDetails.UsageNotes,
                    Register = request.GrammarDetails.Register
                };
                await _unitOfWork.GrammarDetails.AddAsync(newGrammar);
            }
            else
            {
                if (request.GrammarDetails.Structure != null)
                    card.GrammarDetails.Structure = request.GrammarDetails.Structure;
                if (request.GrammarDetails.Explanation != null)
                    card.GrammarDetails.Explanation = request.GrammarDetails.Explanation;
                if (request.GrammarDetails.Caution != null)
                    card.GrammarDetails.Caution = request.GrammarDetails.Caution;
                if (request.GrammarDetails.Level != null && Enum.TryParse<Level>(request.GrammarDetails.Level, true, out var level))
                    card.GrammarDetails.Level = level;
                if (request.GrammarDetails.FormationRules != null)
                    card.GrammarDetails.FormationRules = request.GrammarDetails.FormationRules;
                if (request.GrammarDetails.Nuance != null)
                    card.GrammarDetails.Nuance = request.GrammarDetails.Nuance;
                if (request.GrammarDetails.UsageNotes != null)
                    card.GrammarDetails.UsageNotes = request.GrammarDetails.UsageNotes;
                if (request.GrammarDetails.Register != null)
                    card.GrammarDetails.Register = request.GrammarDetails.Register;
                _unitOfWork.GrammarDetails.UpdateAsync(card.GrammarDetails);
            }
        }

        // Handle VocabularyDetails update
        if (request.VocabularyDetails != null && card.Type == CardType.Vocabulary)
        {
            if (card.VocabularyDetails == null)
            {
                Level? jlptLevel = null;
                if (!string.IsNullOrEmpty(request.VocabularyDetails.JLPTLevel) &&
                    Enum.TryParse<Level>(request.VocabularyDetails.JLPTLevel, true, out var parsedLevel))
                {
                    jlptLevel = parsedLevel;
                }

                var newVocabDetails = new VocabularyDetails
                {
                    CardId = cardId,
                    Reading = request.VocabularyDetails.Reading,
                    PartOfSpeech = request.VocabularyDetails.PartOfSpeech,
                    Pitch = request.VocabularyDetails.Pitch,
                    JLPTLevel = jlptLevel,
                    Frequency = request.VocabularyDetails.Frequency,
                    WaniKaniLevel = request.VocabularyDetails.WaniKaniLevel,
                    Transitivity = request.VocabularyDetails.Transitivity,
                    VerbGroup = request.VocabularyDetails.VerbGroup,
                    AdjectiveType = request.VocabularyDetails.AdjectiveType,
                    CommonCollocations = request.VocabularyDetails.CommonCollocations,
                    Antonyms = request.VocabularyDetails.Antonyms,
                    KanjiComponents = request.VocabularyDetails.KanjiComponents
                };
                await _unitOfWork.VocabularyDetails.AddAsync(newVocabDetails);
            }
            else
            {
                if (request.VocabularyDetails.Reading != null)
                    card.VocabularyDetails.Reading = request.VocabularyDetails.Reading;
                if (request.VocabularyDetails.PartOfSpeech != null)
                    card.VocabularyDetails.PartOfSpeech = request.VocabularyDetails.PartOfSpeech;
                if (request.VocabularyDetails.Pitch != null)
                    card.VocabularyDetails.Pitch = request.VocabularyDetails.Pitch;
                if (request.VocabularyDetails.JLPTLevel != null &&
                    Enum.TryParse<Level>(request.VocabularyDetails.JLPTLevel, true, out var level))
                    card.VocabularyDetails.JLPTLevel = level;
                if (request.VocabularyDetails.Frequency.HasValue)
                    card.VocabularyDetails.Frequency = request.VocabularyDetails.Frequency;
                if (request.VocabularyDetails.WaniKaniLevel.HasValue)
                    card.VocabularyDetails.WaniKaniLevel = request.VocabularyDetails.WaniKaniLevel;
                if (request.VocabularyDetails.Transitivity != null)
                    card.VocabularyDetails.Transitivity = request.VocabularyDetails.Transitivity;
                if (request.VocabularyDetails.VerbGroup != null)
                    card.VocabularyDetails.VerbGroup = request.VocabularyDetails.VerbGroup;
                if (request.VocabularyDetails.AdjectiveType != null)
                    card.VocabularyDetails.AdjectiveType = request.VocabularyDetails.AdjectiveType;
                if (request.VocabularyDetails.CommonCollocations != null)
                    card.VocabularyDetails.CommonCollocations = request.VocabularyDetails.CommonCollocations;
                if (request.VocabularyDetails.Antonyms != null)
                    card.VocabularyDetails.Antonyms = request.VocabularyDetails.Antonyms;
                if (request.VocabularyDetails.KanjiComponents != null)
                    card.VocabularyDetails.KanjiComponents = request.VocabularyDetails.KanjiComponents;
                _unitOfWork.VocabularyDetails.UpdateAsync(card.VocabularyDetails);
            }
        }

        _unitOfWork.Cards.UpdateAsync(card);
        await _unitOfWork.SaveChangesAsync();

        // Reload card with details
        var updatedCard = await _unitOfWork.Cards.GetByIdWithDetailsAsync(cardId);
        return MapToDetailDTO(updatedCard!);
    }

    public async Task<bool> DeleteCardAsync(int userId, int deckId, int cardId)
    {
        var deck = await _unitOfWork.Decks.GetByIdAsync(deckId);
        if (deck == null)
            throw new ApplicationException(MessageConstant.DeckMessage.DECK_NOT_FOUND);
        
        if (deck.UserId != userId)
            throw new ApplicationException(MessageConstant.DeckMessage.DECK_PERMISSION_DENIED);

        var card = await _unitOfWork.Cards.GetByIdWithDetailsAsync(cardId);
        if (card == null || card.DeckId != deckId)
            throw new ApplicationException(MessageConstant.CardMessage.CARD_NOT_FOUND);

        // Delete related data first
        if (card.Examples.Any())
        {
            foreach (var example in card.Examples.ToList())
            {
                _unitOfWork.CardExamples.DeleteAsync(example);
            }
        }

        if (card.GrammarDetails != null)
        {
            _unitOfWork.GrammarDetails.DeleteAsync(card.GrammarDetails);
        }

        if (card.VocabularyDetails != null)
        {
            _unitOfWork.VocabularyDetails.DeleteAsync(card.VocabularyDetails);
        }

        // Delete card
        _unitOfWork.Cards.DeleteAsync(card);

        // Update deck total cards
        deck.TotalCards--;
        _unitOfWork.Decks.UpdateAsync(deck);

        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    private CardDetailDTO MapToDetailDTO(Card card)
    {
        return new CardDetailDTO
        {
            Id = card.Id,
            DeckId = card.DeckId,
            Type = card.Type.ToString(),
            Term = card.Term,
            Meaning = card.Meaning,
            Synonyms = card.Synonyms,
            ImageMediaId = card.ImageMediaId,
            ImageUrl = card.ImageMedia != null ? "/" + card.ImageMedia.FilePath : null,
            Note = card.Note,
            // === Thuộc tính mở rộng ===
            Difficulty = card.Difficulty,
            Priority = card.Priority,
            Tags = card.Tags,
            IsHidden = card.IsHidden,
            AudioMediaId = card.AudioMediaId,
            AudioUrl = card.AudioMedia != null ? "/" + card.AudioMedia.FilePath : null,
            Hint = card.Hint,
            GrammarDetails = card.GrammarDetails != null ? new GrammarDetailsDTO
            {
                Structure = card.GrammarDetails.Structure,
                Explanation = card.GrammarDetails.Explanation,
                Caution = card.GrammarDetails.Caution,
                Level = card.GrammarDetails.Level.ToString(),
                FormationRules = card.GrammarDetails.FormationRules,
                Nuance = card.GrammarDetails.Nuance,
                UsageNotes = card.GrammarDetails.UsageNotes,
                Register = card.GrammarDetails.Register
            } : null,
            VocabularyDetails = card.VocabularyDetails != null ? new VocabularyDetailsDTO
            {
                Reading = card.VocabularyDetails.Reading,
                PartOfSpeech = card.VocabularyDetails.PartOfSpeech,
                Pitch = card.VocabularyDetails.Pitch,
                JLPTLevel = card.VocabularyDetails.JLPTLevel?.ToString(),
                Frequency = card.VocabularyDetails.Frequency,
                WaniKaniLevel = card.VocabularyDetails.WaniKaniLevel,
                Transitivity = card.VocabularyDetails.Transitivity,
                VerbGroup = card.VocabularyDetails.VerbGroup,
                AdjectiveType = card.VocabularyDetails.AdjectiveType,
                CommonCollocations = card.VocabularyDetails.CommonCollocations,
                Antonyms = card.VocabularyDetails.Antonyms,
                KanjiComponents = card.VocabularyDetails.KanjiComponents
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

    public async Task<BulkCreateCardsResponse> BulkCreateCardsAsync(int userId, int deckId, BulkCreateCardsRequest request)
    {
        var deck = await _unitOfWork.Decks.GetByIdAsync(deckId);
        if (deck == null)
            throw new ApplicationException(MessageConstant.DeckMessage.DECK_NOT_FOUND);
        
        if (deck.UserId != userId)
            throw new ApplicationException(MessageConstant.DeckMessage.DECK_PERMISSION_DENIED);

        var response = new BulkCreateCardsResponse
        {
            TotalRequested = request.Cards.Count,
            CreatedCards = new List<CardDetailDTO>(),
            Errors = new List<BulkOperationError>()
        };

        var deckType = deck.Type.ToString();
        int createdCount = 0;

        for (int i = 0; i < request.Cards.Count; i++)
        {
            var cardRequest = request.Cards[i];

            // Validate card type
            if (!Enum.TryParse<CardType>(cardRequest.Type, true, out var cardType))
            {
                response.Errors.Add(new BulkOperationError { Index = i, Message = "Invalid card type" });
                continue;
            }

            // Validate card type matches deck type
            if (cardType.ToString() != deckType)
            {
                response.Errors.Add(new BulkOperationError { Index = i, Message = "Card type must match deck type" });
                continue;
            }

            // Create card
            var card = new Card
            {
                DeckId = deckId,
                Type = cardType,
                Term = cardRequest.Term,
                Meaning = cardRequest.Meaning,
                Synonyms = cardRequest.Synonyms,
                ImageMediaId = cardRequest.ImageMediaId,
                Note = cardRequest.Note,
                // === Thuộc tính mở rộng ===
                Difficulty = cardRequest.Difficulty,
                Priority = cardRequest.Priority,
                Tags = cardRequest.Tags,
                IsHidden = cardRequest.IsHidden,
                AudioMediaId = cardRequest.AudioMediaId,
                Hint = cardRequest.Hint
            };

            await _unitOfWork.Cards.AddAsync(card);
            await _unitOfWork.SaveChangesAsync();

            // Add grammar details if provided
            if (cardRequest.GrammarDetails != null && cardType == CardType.Grammar)
            {
                if (!Enum.TryParse<Level>(cardRequest.GrammarDetails.Level, true, out var level))
                    level = Level.N5;

                var grammarDetails = new GrammarDetails
                {
                    CardId = card.Id,
                    Structure = cardRequest.GrammarDetails.Structure,
                    Explanation = cardRequest.GrammarDetails.Explanation,
                    Caution = cardRequest.GrammarDetails.Caution,
                    Level = level,
                    FormationRules = cardRequest.GrammarDetails.FormationRules,
                    Nuance = cardRequest.GrammarDetails.Nuance,
                    UsageNotes = cardRequest.GrammarDetails.UsageNotes,
                    Register = cardRequest.GrammarDetails.Register
                };
                await _unitOfWork.GrammarDetails.AddAsync(grammarDetails);
            }

            // Add vocabulary details if provided
            if (cardRequest.VocabularyDetails != null && cardType == CardType.Vocabulary)
            {
                Level? jlptLevel = null;
                if (!string.IsNullOrEmpty(cardRequest.VocabularyDetails.JLPTLevel) &&
                    Enum.TryParse<Level>(cardRequest.VocabularyDetails.JLPTLevel, true, out var parsedLevel))
                {
                    jlptLevel = parsedLevel;
                }

                var vocabularyDetails = new VocabularyDetails
                {
                    CardId = card.Id,
                    Reading = cardRequest.VocabularyDetails.Reading,
                    PartOfSpeech = cardRequest.VocabularyDetails.PartOfSpeech,
                    Pitch = cardRequest.VocabularyDetails.Pitch,
                    JLPTLevel = jlptLevel,
                    Frequency = cardRequest.VocabularyDetails.Frequency,
                    WaniKaniLevel = cardRequest.VocabularyDetails.WaniKaniLevel,
                    Transitivity = cardRequest.VocabularyDetails.Transitivity,
                    VerbGroup = cardRequest.VocabularyDetails.VerbGroup,
                    AdjectiveType = cardRequest.VocabularyDetails.AdjectiveType,
                    CommonCollocations = cardRequest.VocabularyDetails.CommonCollocations,
                    Antonyms = cardRequest.VocabularyDetails.Antonyms,
                    KanjiComponents = cardRequest.VocabularyDetails.KanjiComponents
                };
                await _unitOfWork.VocabularyDetails.AddAsync(vocabularyDetails);
            }

            // Add examples if provided
            if (cardRequest.Examples != null && cardRequest.Examples.Any())
            {
                foreach (var ex in cardRequest.Examples)
                {
                    var example = new CardExample
                    {
                        CardId = card.Id,
                        SentenceJapanese = ex.SentenceJapanese,
                        SentenceMeaning = ex.SentenceMeaning,
                        ClozePart = ex.ClozePart,
                        AlternativeAnswers = ex.AlternativeAnswers,
                        AudioMediaId = ex.AudioMediaId
                    };
                    await _unitOfWork.CardExamples.AddAsync(example);
                }
            }

            createdCount++;
            var createdCard = await _unitOfWork.Cards.GetByIdWithDetailsAsync(card.Id);
            response.CreatedCards.Add(MapToDetailDTO(createdCard!));
        }

        // Update deck total cards
        deck.TotalCards += createdCount;
        _unitOfWork.Decks.UpdateAsync(deck);
        await _unitOfWork.SaveChangesAsync();

        response.TotalCreated = createdCount;
        if (!response.Errors.Any()) response.Errors = null;

        return response;
    }

    public async Task<BulkDeleteCardsResponse> BulkDeleteCardsAsync(int userId, int deckId, BulkDeleteCardsRequest request)
    {
        var deck = await _unitOfWork.Decks.GetByIdAsync(deckId);
        if (deck == null)
            throw new ApplicationException(MessageConstant.DeckMessage.DECK_NOT_FOUND);
        
        if (deck.UserId != userId)
            throw new ApplicationException(MessageConstant.DeckMessage.DECK_PERMISSION_DENIED);

        var response = new BulkDeleteCardsResponse
        {
            TotalRequested = request.CardIds.Count,
            DeletedIds = new List<int>(),
            FailedIds = new List<int>()
        };

        foreach (var cardId in request.CardIds)
        {
            var card = await _unitOfWork.Cards.GetByIdWithDetailsAsync(cardId);
            if (card == null || card.DeckId != deckId)
            {
                response.FailedIds.Add(cardId);
                continue;
            }

            // Delete related data
            if (card.Examples.Any())
            {
                foreach (var example in card.Examples.ToList())
                {
                    _unitOfWork.CardExamples.DeleteAsync(example);
                }
            }

            if (card.GrammarDetails != null)
            {
                _unitOfWork.GrammarDetails.DeleteAsync(card.GrammarDetails);
            }

            if (card.VocabularyDetails != null)
            {
                _unitOfWork.VocabularyDetails.DeleteAsync(card.VocabularyDetails);
            }

            _unitOfWork.Cards.DeleteAsync(card);
            response.DeletedIds.Add(cardId);
        }

        // Update deck total cards
        deck.TotalCards -= response.DeletedIds.Count;
        _unitOfWork.Decks.UpdateAsync(deck);
        await _unitOfWork.SaveChangesAsync();

        response.TotalDeleted = response.DeletedIds.Count;
        if (!response.FailedIds.Any()) response.FailedIds = null;

        return response;
    }

    public async Task<BulkUpdateCardsResponse> BulkUpdateCardsAsync(int userId, int deckId, BulkUpdateCardsRequest request)
    {
        var deck = await _unitOfWork.Decks.GetByIdAsync(deckId);
        if (deck == null)
            throw new ApplicationException(MessageConstant.DeckMessage.DECK_NOT_FOUND);
        
        if (deck.UserId != userId)
            throw new ApplicationException(MessageConstant.DeckMessage.DECK_PERMISSION_DENIED);

        var response = new BulkUpdateCardsResponse
        {
            TotalRequested = request.Cards.Count,
            UpdatedCards = new List<CardDetailDTO>(),
            Errors = new List<BulkOperationError>()
        };

        for (int i = 0; i < request.Cards.Count; i++)
        {
            var item = request.Cards[i];
            var card = await _unitOfWork.Cards.GetByIdWithDetailsAsync(item.Id);

            if (card == null || card.DeckId != deckId)
            {
                response.Errors.Add(new BulkOperationError { Index = i, Message = $"Card {item.Id} not found" });
                continue;
            }

            // Update only provided fields
            if (item.Term != null) card.Term = item.Term;
            if (item.Meaning != null) card.Meaning = item.Meaning;
            if (item.Synonyms != null) card.Synonyms = item.Synonyms;
            if (item.ImageMediaId.HasValue) card.ImageMediaId = item.ImageMediaId;
            if (item.Note != null) card.Note = item.Note;

            _unitOfWork.Cards.UpdateAsync(card);
            response.UpdatedCards.Add(MapToDetailDTO(card));
        }

        await _unitOfWork.SaveChangesAsync();
        response.TotalUpdated = response.UpdatedCards.Count;
        if (!response.Errors.Any()) response.Errors = null;

        return response;
    }

    public async Task<CardExampleDTO> AddExampleAsync(int userId, int cardId, CreateExampleRequest request)
    {
        var card = await _unitOfWork.Cards.GetByIdWithDetailsAsync(cardId);
        if (card == null)
            throw new ApplicationException(MessageConstant.CardMessage.CARD_NOT_FOUND);

        var deck = await _unitOfWork.Decks.GetByIdAsync(card.DeckId);
        if (deck == null || deck.UserId != userId)
            throw new ApplicationException(MessageConstant.DeckMessage.DECK_PERMISSION_DENIED);

        var example = new CardExample
        {
            CardId = cardId,
            SentenceJapanese = request.SentenceJapanese,
            SentenceMeaning = request.SentenceMeaning,
            ClozePart = request.ClozePart,
            AlternativeAnswers = request.AlternativeAnswers,
            AudioMediaId = request.AudioMediaId
        };

        await _unitOfWork.CardExamples.AddAsync(example);
        await _unitOfWork.SaveChangesAsync();

        // Load AudioMedia if exists
        MediaFile? audioMedia = null;
        if (example.AudioMediaId.HasValue)
        {
            audioMedia = await _unitOfWork.MediaFiles.GetByIdAsync(example.AudioMediaId.Value);
        }

        return new CardExampleDTO
        {
            Id = example.Id,
            SentenceJapanese = example.SentenceJapanese,
            SentenceMeaning = example.SentenceMeaning,
            ClozePart = example.ClozePart,
            AlternativeAnswers = example.AlternativeAnswers,
            AudioMediaId = example.AudioMediaId,
            AudioUrl = audioMedia != null ? "/" + audioMedia.FilePath : null
        };
    }

    public async Task<CardExampleDTO> UpdateExampleAsync(int userId, int cardId, int exampleId, UpdateExampleRequest request)
    {
        var card = await _unitOfWork.Cards.GetByIdWithDetailsAsync(cardId);
        if (card == null)
            throw new ApplicationException(MessageConstant.CardMessage.CARD_NOT_FOUND);

        var deck = await _unitOfWork.Decks.GetByIdAsync(card.DeckId);
        if (deck == null || deck.UserId != userId)
            throw new ApplicationException(MessageConstant.DeckMessage.DECK_PERMISSION_DENIED);

        var example = card.Examples.FirstOrDefault(e => e.Id == exampleId);
        if (example == null)
            throw new ApplicationException(MessageConstant.CardMessage.EXAMPLE_NOT_FOUND);

        // Update only provided fields
        if (request.SentenceJapanese != null) example.SentenceJapanese = request.SentenceJapanese;
        if (request.SentenceMeaning != null) example.SentenceMeaning = request.SentenceMeaning;
        if (request.ClozePart != null) example.ClozePart = request.ClozePart;
        if (request.AlternativeAnswers != null) example.AlternativeAnswers = request.AlternativeAnswers;
        if (request.AudioMediaId.HasValue) example.AudioMediaId = request.AudioMediaId;

        _unitOfWork.CardExamples.UpdateAsync(example);
        await _unitOfWork.SaveChangesAsync();

        // Load AudioMedia if exists
        MediaFile? audioMedia = null;
        if (example.AudioMediaId.HasValue)
        {
            audioMedia = await _unitOfWork.MediaFiles.GetByIdAsync(example.AudioMediaId.Value);
        }

        return new CardExampleDTO
        {
            Id = example.Id,
            SentenceJapanese = example.SentenceJapanese,
            SentenceMeaning = example.SentenceMeaning,
            ClozePart = example.ClozePart,
            AlternativeAnswers = example.AlternativeAnswers,
            AudioMediaId = example.AudioMediaId,
            AudioUrl = audioMedia != null ? "/" + audioMedia.FilePath : null
        };
    }

    public async Task<bool> DeleteExampleAsync(int userId, int cardId, int exampleId)
    {
        var card = await _unitOfWork.Cards.GetByIdWithDetailsAsync(cardId);
        if (card == null)
            throw new ApplicationException(MessageConstant.CardMessage.CARD_NOT_FOUND);

        var deck = await _unitOfWork.Decks.GetByIdAsync(card.DeckId);
        if (deck == null || deck.UserId != userId)
            throw new ApplicationException(MessageConstant.DeckMessage.DECK_PERMISSION_DENIED);

        var example = card.Examples.FirstOrDefault(e => e.Id == exampleId);
        if (example == null)
            throw new ApplicationException(MessageConstant.CardMessage.EXAMPLE_NOT_FOUND);

        _unitOfWork.CardExamples.DeleteAsync(example);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<GrammarDetailsDTO> UpdateGrammarAsync(int userId, int cardId, UpdateGrammarRequest request)
    {
        var card = await _unitOfWork.Cards.GetByIdWithDetailsAsync(cardId);
        if (card == null)
            throw new ApplicationException(MessageConstant.CardMessage.CARD_NOT_FOUND);

        var deck = await _unitOfWork.Decks.GetByIdAsync(card.DeckId);
        if (deck == null || deck.UserId != userId)
            throw new ApplicationException(MessageConstant.DeckMessage.DECK_PERMISSION_DENIED);

        if (card.Type != CardType.Grammar)
            throw new ApplicationException(MessageConstant.CardMessage.CARD_TYPE_MISMATCH);

        // Create or update grammar details
        if (card.GrammarDetails == null)
        {
            var newGrammar = new GrammarDetails
            {
                CardId = cardId,
                Structure = request.Structure,
                Explanation = request.Explanation,
                Caution = request.Caution,
                Level = Enum.TryParse<Level>(request.Level, true, out var level) ? level : Level.N5,
                FormationRules = request.FormationRules,
                Nuance = request.Nuance,
                UsageNotes = request.UsageNotes,
                Register = request.Register
            };
            await _unitOfWork.GrammarDetails.AddAsync(newGrammar);
            await _unitOfWork.SaveChangesAsync();

            return new GrammarDetailsDTO
            {
                Structure = newGrammar.Structure,
                Explanation = newGrammar.Explanation,
                Caution = newGrammar.Caution,
                Level = newGrammar.Level.ToString(),
                FormationRules = newGrammar.FormationRules,
                Nuance = newGrammar.Nuance,
                UsageNotes = newGrammar.UsageNotes,
                Register = newGrammar.Register
            };
        }
        else
        {
            // Update existing
            if (request.Structure != null) card.GrammarDetails.Structure = request.Structure;
            if (request.Explanation != null) card.GrammarDetails.Explanation = request.Explanation;
            if (request.Caution != null) card.GrammarDetails.Caution = request.Caution;
            if (request.Level != null && Enum.TryParse<Level>(request.Level, true, out var level))
                card.GrammarDetails.Level = level;
            if (request.FormationRules != null) card.GrammarDetails.FormationRules = request.FormationRules;
            if (request.Nuance != null) card.GrammarDetails.Nuance = request.Nuance;
            if (request.UsageNotes != null) card.GrammarDetails.UsageNotes = request.UsageNotes;
            if (request.Register != null) card.GrammarDetails.Register = request.Register;

            _unitOfWork.GrammarDetails.UpdateAsync(card.GrammarDetails);
            await _unitOfWork.SaveChangesAsync();

            return new GrammarDetailsDTO
            {
                Structure = card.GrammarDetails.Structure,
                Explanation = card.GrammarDetails.Explanation,
                Caution = card.GrammarDetails.Caution,
                Level = card.GrammarDetails.Level.ToString(),
                FormationRules = card.GrammarDetails.FormationRules,
                Nuance = card.GrammarDetails.Nuance,
                UsageNotes = card.GrammarDetails.UsageNotes,
                Register = card.GrammarDetails.Register
            };
        }
    }
}
