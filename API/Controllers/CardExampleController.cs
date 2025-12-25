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
    [HttpPost("examples")]
    public async Task<ApiResponse<CardExampleDTO>> AddExampleAsync(int cardId, [FromBody] CreateExampleRequest request)
    {
        var result = await HandleException(_service.AddExampleAsync(GetCurrentUserId(), cardId, request));
        return result;
    }

    /// <summary>
    /// Cập nhật example
    /// </summary>
    [HttpPut("examples/{id}")]
    public async Task<ApiResponse<CardExampleDTO>> UpdateExampleAsync(int cardId, int id, [FromBody] UpdateExampleRequest request)
    {
        var result = await HandleException(_service.UpdateExampleAsync(GetCurrentUserId(), cardId, id, request));
        return result;
    }

    /// <summary>
    /// Xóa example
    /// </summary>
    [HttpDelete("examples/{id}")]
    public async Task<ApiResponse<bool>> DeleteExampleAsync(int cardId, int id)
    {
        var result = await HandleException(_service.DeleteExampleAsync(GetCurrentUserId(), cardId, id));
        return result;
    }

    /// <summary>
    /// Cập nhật grammar details (chỉ cho Grammar cards)
    /// </summary>
    [HttpPut("grammar")]
    public async Task<ApiResponse<GrammarDetailsDTO>> UpdateGrammarAsync(int cardId, [FromBody] UpdateGrammarRequest request)
    {
        var result = await HandleException(_service.UpdateGrammarAsync(GetCurrentUserId(), cardId, request));
        return result;
    }
}
