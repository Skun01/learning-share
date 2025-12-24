using Application.Common;
using Application.DTOs.Common;
using Application.DTOs.Deck;
using Application.DTOs.Store;
using Application.IServices;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/store/decks")]
public class StoreController : BaseController
{
    private readonly IStoreService _service;

    public StoreController(IStoreService service)
    {
        _service = service;
    }

    /// <summary>
    /// Lấy danh sách public decks từ Store
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ApiResponse<IEnumerable<PublicDeckDetailDTO>>> GetPublicDeckByFilter(
        [FromQuery] SearchDeckRequest request
    )
    {
        
        var queryModel = new QueryDTO<SearchDeckRequest>
        {
            UserId = GetCurrentUserId(),
            Query = request
        };

        var result = await HandleException(_service.GetPublicDecksByFilterAsync(queryModel));

        result.MetaData = new MetaData
        {
            Page = request.Page,
            PageSize = request.PageSize,
            Total = queryModel.Total
        };

        return result;
    }

    /// <summary>
    /// Lấy danh sách decks đang trending
    /// </summary>
    /// <param name="limit"></param>
    /// <returns></returns>
    [HttpGet("trending")]
    public async Task<ApiResponse<IEnumerable<PublicDeckDetailDTO>>> GetTrendingDecksAsync(
        [FromQuery] int limit = 10)
    {
        var result = await HandleException(_service.GetTrendingDecksAsync(limit));
        return result;
    }

    /// <summary>
    /// Lấy danh sách tags phổ biến
    /// </summary>
    /// <param name="limit"></param>
    /// <returns></returns>
    [HttpGet("~/api/store/tags")]
    public async Task<ApiResponse<IEnumerable<TagStatDTO>>> GetPopularTagsAsync(
        [FromQuery] int limit = 20)
    {
        var result = await HandleException(_service.GetPopularTagsAsync(limit));
        return result;
    }

    /// <summary>
    /// Lấy chi tiết public deck với sample cards
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<ApiResponse<PublicDeckDetailDTO>> GetPublicDeckByIdAsync(int id)
    {
        var result = await HandleException(_service.GetPublicDeckByIdAsync(id));
        return result;
    }

    /// <summary>
    /// Clone public deck về thư viện cá nhân
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("{id}/clone")]
    public async Task<ApiResponse<DeckDetailDTO>> CloneDeckAsync(
        [FromRoute] int id, 
        [FromBody] CloneDeckRequest request)
    {
        var requestModel = new RequestDTO<CloneDeckRequest>
        {
            UserId = GetCurrentUserId(),
            Request = request
        };

        var result = await HandleException(_service.CloneDeckAsync(id, requestModel));
        return result;
    }
}
