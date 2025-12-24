using Application.Common;
using Application.DTOs.Common;
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
    public async Task<ApiResponse<IEnumerable<DeckSummaryDTO>>> GetMyDecksAsync([FromQuery] GetMyDecksRequest request)
    {
        var requestModel = new QueryDTO<GetMyDecksRequest>
        {
            UserId = GetCurrentUserId(),
            Query = request
        };

        var result = await HandleException(_service.GetMyDecksByFilterAsync(requestModel));
        result.MetaData = new MetaData
        {
            Page = request.Page,
            PageSize = request.PageSize,
            Total = requestModel.Total
        };

        return result;
    }

    /// <summary>
    /// Lấy chi tiết Deck theo ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<ApiResponse<DeckDetailDTO>> GetDeckByIdAsync(int id)
    {
        var result = await HandleException(_service.GetDeckByIdAsync(GetCurrentUserId(), id));

        return result;
    }

    /// <summary>
    /// Lấy thống kê tổng quan về decks
    /// </summary>
    /// <returns></returns>
    [HttpGet("statistics")]
    public async Task<ApiResponse<DeckStatisticsDTO>> GetDeckStatisticsAsync()
    {
        var result = await HandleException(_service.GetDeckStatisticsAsync(GetCurrentUserId()));

        return result;
    }

    /// <summary>
    /// Tạo Deck mới
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ApiResponse<DeckDetailDTO>> CreateDeckAsync([FromBody] CreateDeckRequest request)
    {
        var result = await HandleException(_service.CreateDeckAsync(GetCurrentUserId(), request));

        return result;
    }

    /// <summary>
    /// Cập nhật thông tin Deck
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<ApiResponse<DeckDetailDTO>> UpdateDeckAsync(int id, [FromBody] UpdateDeckRequest request)
    {
        var result = await HandleException(_service.UpdateDeckAsync(GetCurrentUserId(), id, request));

        return result;
    }

    /// <summary>
    /// Publish/Unpublish Deck (toggle IsPublic)
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPatch("{id}/publish")]
    public async Task<ApiResponse<DeckDetailDTO>> TogglePublishAsync(int id)
    {
        var result = await HandleException(_service.TogglePublishAsync(GetCurrentUserId(), id));

        return result;
    }

    /// <summary>
    /// Xóa Deck
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> DeleteDeckAsync(int id)
    {
        var result = await HandleException(_service.DeleteDeckAsync(GetCurrentUserId(), id));

        return result;
    }

    /// <summary>
    /// Học lại từ đầu - Reset tiến độ học
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPost("{id}/reset")]
    public async Task<ApiResponse<bool>> ResetDeckProgressAsync(int id)
    {
        var result = await HandleException(_service.ResetDeckProgressAsync(GetCurrentUserId(), id));

        return result;
    }
}
