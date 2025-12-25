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
}
