using Application.Common;
using Application.DTOs.Common;
using Application.DTOs.Deck;
using Application.DTOs.Store;

namespace Application.IServices;

public interface IStoreService
{
    Task<IEnumerable<PublicDeckDetailDTO>> GetPublicDecksByFilterAsync(QueryDTO<SearchDeckRequest> request);
    Task<PublicDeckDetailDTO> GetPublicDeckByIdAsync(int deckId);
    Task<DeckDetailDTO> CloneDeckAsync(int deckId, RequestDTO<CloneDeckRequest> request);
    Task<IEnumerable<PublicDeckDetailDTO>> GetTrendingDecksAsync(int limit);
    Task<IEnumerable<TagStatDTO>> GetPopularTagsAsync(int limit);
}
