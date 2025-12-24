using Application.DTOs.Common;
using Application.DTOs.Deck;
using Application.DTOs.Store;
using Domain.Entities;
using Domain.Enums;

namespace Application.IRepositories;

public interface IDeckRepository : IRepository<Deck>
{
    Task<IEnumerable<Deck>> GetByUserId(int userId);
    Task<IEnumerable<Deck>> GetPublicByFilterAsync(QueryDTO<SearchDeckRequest> request);
    Task<IEnumerable<Deck>> GetMyDecksByFilterAsync(QueryDTO<GetMyDecksRequest> request);
    Task<Deck?> GetPublicByIdAsync(int deckId);
    Task<Deck> CreateCloneAsync(Deck originDeck, int userId, string? customName);
}
