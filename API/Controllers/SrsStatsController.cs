using Application.Common;
using Application.DTOs.Common;
using Application.DTOs.Study;
using Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/srs/stats")]
[ApiController]
[Authorize]
public class SrsStatsController : BaseController
{
    private readonly IStudyService _studyService;

    public SrsStatsController(IStudyService studyService)
    {
        _studyService = studyService;
    }

    /// <summary>
    /// Lấy dữ liệu biểu đồ nhiệt theo năm
    /// </summary>
    [HttpGet("heatmap")]
    public async Task<ApiResponse<IEnumerable<HeatmapDataDTO>>> GetHeatmapAsync([FromQuery] GetHeatmapRequest request)
    {
        var requestModel = new QueryDTO<GetHeatmapRequest>
        {
            UserId = GetCurrentUserId(),
            Query = request
        };

        return await HandleException(_studyService.GetHeatmapAsync(requestModel));
    }

    /// <summary>
    /// Lấy dự báo khối lượng bài tập trong N ngày tới
    /// </summary>
    [HttpGet("forecast")]
    public async Task<ApiResponse<IEnumerable<ForecastDTO>>> GetForecastAsync([FromQuery] GetForecastRequest request)
    {
        var requestModel = new QueryDTO<GetForecastRequest>
        {
            UserId = GetCurrentUserId(),
            Query = request
        };

        return await HandleException(_studyService.GetForecastAsync(requestModel));
    }

    /// <summary>
    /// Lấy tỷ lệ đúng/sai theo khoảng thời gian
    /// </summary>
    [HttpGet("accuracy")]
    public async Task<ApiResponse<AccuracyDTO>> GetAccuracyAsync([FromQuery] GetAccuracyRequest request)
    {
        var requestModel = new QueryDTO<GetAccuracyRequest>
        {
            UserId = GetCurrentUserId(),
            Query = request
        };

        return await HandleException(_studyService.GetAccuracyAsync(requestModel));
    }
}
