using Application.DTOs.Card;
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
            ImageUrl = c.ImageUrl,
            HasExamples = c.Examples.Any(),
            HasGrammarDetails = c.GrammarDetails != null
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
            ImageUrl = request.ImageUrl,
            Note = request.Note
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
                Level = level
            };
            await _unitOfWork.GrammarDetails.AddAsync(grammarDetails);
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
                    AudioUrl = ex.AudioUrl
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
        if (request.ImageUrl != null)
            card.ImageUrl = request.ImageUrl;
        if (request.Note != null)
            card.Note = request.Note;

        _unitOfWork.Cards.UpdateAsync(card);
        await _unitOfWork.SaveChangesAsync();

        return MapToDetailDTO(card);
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
            ImageUrl = card.ImageUrl,
            Note = card.Note,
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
                AudioUrl = e.AudioUrl
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
                ImageUrl = cardRequest.ImageUrl,
                Note = cardRequest.Note
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
                    Level = level
                };
                await _unitOfWork.GrammarDetails.AddAsync(grammarDetails);
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
                        AudioUrl = ex.AudioUrl
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
            AudioUrl = request.AudioUrl
        };

        await _unitOfWork.CardExamples.AddAsync(example);
        await _unitOfWork.SaveChangesAsync();

        return new CardExampleDTO
        {
            Id = example.Id,
            SentenceJapanese = example.SentenceJapanese,
            SentenceMeaning = example.SentenceMeaning,
            ClozePart = example.ClozePart,
            AlternativeAnswers = example.AlternativeAnswers,
            AudioUrl = example.AudioUrl
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
        if (request.AudioUrl != null) example.AudioUrl = request.AudioUrl;

        _unitOfWork.CardExamples.UpdateAsync(example);
        await _unitOfWork.SaveChangesAsync();

        return new CardExampleDTO
        {
            Id = example.Id,
            SentenceJapanese = example.SentenceJapanese,
            SentenceMeaning = example.SentenceMeaning,
            ClozePart = example.ClozePart,
            AlternativeAnswers = example.AlternativeAnswers,
            AudioUrl = example.AudioUrl
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
                Level = Enum.TryParse<Level>(request.Level, true, out var level) ? level : Level.N5
            };
            await _unitOfWork.GrammarDetails.AddAsync(newGrammar);
            await _unitOfWork.SaveChangesAsync();

            return new GrammarDetailsDTO
            {
                Structure = newGrammar.Structure,
                Explanation = newGrammar.Explanation,
                Caution = newGrammar.Caution,
                Level = newGrammar.Level.ToString()
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

            _unitOfWork.GrammarDetails.UpdateAsync(card.GrammarDetails);
            await _unitOfWork.SaveChangesAsync();

            return new GrammarDetailsDTO
            {
                Structure = card.GrammarDetails.Structure,
                Explanation = card.GrammarDetails.Explanation,
                Caution = card.GrammarDetails.Caution,
                Level = card.GrammarDetails.Level.ToString()
            };
        }
    }
}
