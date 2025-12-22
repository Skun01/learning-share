using Application.Common;
using Application.DTOs.Deck;
using Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/decks")]
[Authorize]
public class DeckController : BaseController
{
    private readonly IDeckService _service;
    public DeckController(IDeckService service)
    {
        _service = service;
    }

    /// <summary>
    /// Lấy danh sách Decks của người dùng hiện tại
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<ApiResponse<IEnumerable<DeckSummaryDTO>>> GetMyDecksAsync()
    {
        var result = await HandleException(_service.GetMyDecksAsync(GetCurrentUserId()));

        return result;
    }
}
