using Application.DTOs.Common;
using Application.DTOs.Deck;
using Application.DTOs.Store;
using Application.DTOs.User;
using Application.IRepositories;
using Application.IServices;
using Domain.Constants;

namespace Application.Services;

public class StoreService : IStoreService
{
    private readonly IUnitOfWork _unitOfWork;

    public StoreService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<DeckDetailDTO> CloneDeckAsync(int deckId, RequestDTO<CloneDeckRequest> request)
    {
        var originDeck = await _unitOfWork.Decks.GetPublicByIdAsync(deckId);
        if (originDeck == null)
            throw new ApplicationException(MessageConstant.DeckMessage.DECK_NOT_FOUND);

        var clonedDeck = await _unitOfWork.Decks.CreateCloneAsync(originDeck, request.UserId, request.Request?.CustomName);
        
        // Increment downloads của deck gốc
        originDeck.Downloads++;
        _unitOfWork.Decks.UpdateAsync(originDeck);
        
        await _unitOfWork.SaveChangesAsync();

        // Return thông tin deck đã clone
        return new DeckDetailDTO
        {
            Id = clonedDeck.Id,
            Name = clonedDeck.Name,
            Description = clonedDeck.Description,
            Type = clonedDeck.Type.ToString(),
            IsPublic = false,
            ParentDeckId = deckId,
            Tags = clonedDeck.DeckTags.Select(t => t.TagName).ToList(),
            TotalCards = 0,
            Downloads = 0,
            CreatedAt = clonedDeck.CreatedAt
        };
    }

    public async Task<PublicDeckDetailDTO> GetPublicDeckByIdAsync(int deckId)
    {
        var deck = await _unitOfWork.Decks.GetPublicByIdAsync(deckId);
        if (deck == null)
            throw new ApplicationException(MessageConstant.DeckMessage.DECK_NOT_FOUND);
        
        var sampleCards = await _unitOfWork.Cards.GetSampleCardsByDeckIdAsync(deckId, 5);
        
        return new PublicDeckDetailDTO
        {
            Id = deck.Id,
            Name = deck.Name,
            Description = deck.Description,
            Type = deck.Type.ToString(),
            Author = new AuthorDTO
            {
                Id = deck.User.Id,
                Name = deck.User.Username,
                AvatarUrl = deck.User.AvatarUrl
            },
            Tags = deck.DeckTags.Select(t => t.TagName).ToList(),
            TotalCards = deck.TotalCards,
            Downloads = deck.Downloads,
            CreatedAt = deck.CreatedAt
        };
    }

    public async Task<IEnumerable<PublicDeckDetailDTO>> GetPublicDecksByFilterAsync(QueryDTO<SearchDeckRequest> request)
    {
        var decks = await _unitOfWork.Decks.GetPublicByFilterAsync(request);

        return decks.Select(deck => new PublicDeckDetailDTO
        {
            Id = deck.Id,
            Name = deck.Name,
            Description = deck.Description,
            Type = deck.Type.ToString(),
            Author = new AuthorDTO
            {
                Id = deck.User.Id,
                Name = deck.User.Username,
                AvatarUrl = deck.User.AvatarUrl
            },
            Tags = deck.DeckTags.Select(t => t.TagName).ToList(),
            TotalCards = deck.TotalCards,
            Downloads = deck.Downloads,
            CreatedAt = deck.CreatedAt
        });
    }

    public async Task<IEnumerable<PublicDeckDetailDTO>> GetTrendingDecksAsync(int limit)
    {
        var decks = await _unitOfWork.Decks.GetTrendingDecksAsync(limit);
        
        return decks.Select(deck => new PublicDeckDetailDTO
        {
            Id = deck.Id,
            Name = deck.Name,
            Description = deck.Description,
            Type = deck.Type.ToString(),
            Author = new AuthorDTO
            {
                Id = deck.User.Id,
                Name = deck.User.Username,
                AvatarUrl = deck.User.AvatarUrl
            },
            Tags = deck.DeckTags.Select(t => t.TagName).ToList(),
            TotalCards = deck.TotalCards,
            Downloads = deck.Downloads,
            CreatedAt = deck.CreatedAt
        });
    }

    public async Task<IEnumerable<TagStatDTO>> GetPopularTagsAsync(int limit)
    {
        return await _unitOfWork.Decks.GetPopularTagsAsync(limit);
    }
}
