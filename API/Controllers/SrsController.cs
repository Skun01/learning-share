using Application.Common;
using Application.DTOs.Common;
using Application.DTOs.Study;
using Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/srs")]
[ApiController]
[Authorize]
public class SrsController : BaseController
{
    private readonly IStudyService _studyService;

    public SrsController(IStudyService studyService)
    {
        _studyService = studyService;
    }

    /// <summary>
    /// Lấy số lượng thẻ cần ôn, thẻ mới và thẻ ghost
    /// </summary>
    [HttpGet("count")]
    public async Task<ApiResponse<StudyCountDTO>> GetStudyCountAsync([FromQuery] GetStudyCountRequest request)
    {
        var requestModel = new QueryDTO<GetStudyCountRequest>
        {
            UserId = GetCurrentUserId(),
            Query = request
        };

        return await HandleException(_studyService.GetStudyCountAsync(requestModel));
    }

    /// <summary>
    /// Lấy danh sách thẻ cần ôn (ưu tiên thẻ ghost trước)
    /// </summary>
    [HttpGet("reviews/available")]
    public async Task<ApiResponse<IEnumerable<StudyCardDTO>>> GetAvailableReviewsAsync([FromQuery] GetAvailableReviewsRequest request)
    {
        var requestModel = new QueryDTO<GetAvailableReviewsRequest>
        {
            UserId = GetCurrentUserId(),
            Query = request
        };

        return await HandleException(_studyService.GetAvailableReviewsAsync(requestModel));
    }
}
