using Application.Common;
using Application.DTOs.Card;
using Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/cards/{cardId}")]
[Authorize]
public class CardExampleController : BaseController
{
    private readonly ICardService _service;

    public CardExampleController(ICardService service)
    {
        _service = service;
    }

    /// <summary>
    /// Thêm example cho card
    /// </summary>
    /// <param name="cardId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("examples")]
    public async Task<ApiResponse<CardExampleDTO>> AddExampleAsync(int cardId, [FromBody] CreateExampleRequest request)
    {
        var result = await HandleException(_service.AddExampleAsync(GetCurrentUserId(), cardId, request));
        return result;
    }

    /// <summary>
    /// Cập nhật example
    /// </summary>
    /// <param name="cardId"></param>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut("examples/{id}")]
    public async Task<ApiResponse<CardExampleDTO>> UpdateExampleAsync(int cardId, int id, [FromBody] UpdateExampleRequest request)
    {
        var result = await HandleException(_service.UpdateExampleAsync(GetCurrentUserId(), cardId, id, request));
        return result;
    }

    /// <summary>
    /// Xóa example
    /// </summary>
    /// <param name="cardId"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("examples/{id}")]
    public async Task<ApiResponse<bool>> DeleteExampleAsync(int cardId, int id)
    {
        var result = await HandleException(_service.DeleteExampleAsync(GetCurrentUserId(), cardId, id));
        return result;
    }

    /// <summary>
    /// Cập nhật grammar details (chỉ cho Grammar cards)
    /// </summary>
    /// <param name="cardId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut("grammar")]
    public async Task<ApiResponse<GrammarDetailsDTO>> UpdateGrammarAsync(int cardId, [FromBody] UpdateGrammarRequest request)
    {
        var result = await HandleException(_service.UpdateGrammarAsync(GetCurrentUserId(), cardId, request));
        return result;
    }
}
