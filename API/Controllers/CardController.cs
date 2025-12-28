using Application.Common;
using Application.DTOs.Card;
using Application.DTOs.Common;
using Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/decks/{deckId}/cards")]
[Authorize]
public class CardController : BaseController
{
    private readonly ICardService _service;

    public CardController(ICardService service)
    {
        _service = service;
    }

    /// <summary>
    /// Lấy danh sách cards trong deck (với pagination và filter)
    /// </summary>
    /// <param name="deckId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ApiResponse<IEnumerable<CardSummaryDTO>>> GetCardsAsync(int deckId, [FromQuery] GetCardsRequest request)
    {
        var requestModel = new QueryDTO<GetCardsRequest>
        {
            UserId = GetCurrentUserId(),
            Query = request
        };

        var result = await HandleException(_service.GetCardsWithFilterAsync(deckId, requestModel));
        result.MetaData = new MetaData
        {
            Page = request.Page,
            PageSize = request.PageSize,
            Total = requestModel.Total
        };

        return result;
    }

    /// <summary>
    /// Lấy chi tiết card
    /// </summary>
    /// <param name="deckId"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<ApiResponse<CardDetailDTO>> GetCardByIdAsync(int deckId, int id)
    {
        var result = await HandleException(_service.GetCardByIdAsync(GetCurrentUserId(), deckId, id));
        return result;
    }

    /// <summary>
    /// Tạo card mới
    /// </summary>
    /// <param name="deckId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ApiResponse<CardDetailDTO>> CreateCardAsync(int deckId, [FromBody] CreateCardRequest request)
    {
        var result = await HandleException(_service.CreateCardAsync(GetCurrentUserId(), deckId, request));
        return result;
    }

    /// <summary>
    /// Cập nhật card
    /// </summary>
    /// <param name="deckId"></param>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<ApiResponse<CardDetailDTO>> UpdateCardAsync(int deckId, int id, [FromBody] UpdateCardRequest request)
    {
        var result = await HandleException(_service.UpdateCardAsync(GetCurrentUserId(), deckId, id, request));
        return result;
    }

    /// <summary>
    /// Xóa card
    /// </summary>
    /// <param name="deckId"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> DeleteCardAsync(int deckId, int id)
    {
        var result = await HandleException(_service.DeleteCardAsync(GetCurrentUserId(), deckId, id));
        return result;
    }

    /// <summary>
    /// Tạo nhiều cards cùng lúc
    /// </summary>
    /// <param name="deckId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("bulk")]
    public async Task<ApiResponse<BulkCreateCardsResponse>> BulkCreateCardsAsync(int deckId, [FromBody] BulkCreateCardsRequest request)
    {
        var result = await HandleException(_service.BulkCreateCardsAsync(GetCurrentUserId(), deckId, request));
        return result;
    }

    /// <summary>
    /// Xóa nhiều cards cùng lúc
    /// </summary>
    /// <param name="deckId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpDelete("bulk")]
    public async Task<ApiResponse<BulkDeleteCardsResponse>> BulkDeleteCardsAsync(int deckId, [FromBody] BulkDeleteCardsRequest request)
    {
        var result = await HandleException(_service.BulkDeleteCardsAsync(GetCurrentUserId(), deckId, request));
        return result;
    }

    /// <summary>
    /// Cập nhật nhiều cards cùng lúc
    /// </summary>
    /// <param name="deckId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut("bulk")]
    public async Task<ApiResponse<BulkUpdateCardsResponse>> BulkUpdateCardsAsync(int deckId, [FromBody] BulkUpdateCardsRequest request)
    {
        var result = await HandleException(_service.BulkUpdateCardsAsync(GetCurrentUserId(), deckId, request));
        return result;
    }
}
