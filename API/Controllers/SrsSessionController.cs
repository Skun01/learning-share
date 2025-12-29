using Application.Common;
using Application.DTOs.Common;
using Application.DTOs.Study;
using Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/srs/sessions")]
[ApiController]
[Authorize]
public class SrsSessionController : BaseController
{
    private readonly ISessionService _sessionService;

    public SrsSessionController(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    /// <summary>
    /// Bắt đầu phiên học tập
    /// </summary>
    [HttpPost("start")]
    public async Task<ApiResponse<SessionDTO>> StartSessionAsync([FromBody] StartSessionRequest request)
    {
        var requestModel = new RequestDTO<StartSessionRequest>
        {
            UserId = GetCurrentUserId(),
            Request = request
        };
        return await HandleException(_sessionService.StartSessionAsync(requestModel));
    }

    /// <summary>
    /// Submit kết quả và lấy card tiếp theo
    /// </summary>
    [HttpPost("submit")]
    public async Task<ApiResponse<SessionDTO>> SubmitSessionAsync([FromBody] SubmitSessionRequest request)
    {
        var requestModel = new RequestDTO<SubmitSessionRequest>
        {
            UserId = GetCurrentUserId(),
            Request = request
        };
        return await HandleException(_sessionService.SubmitSessionAsync(requestModel));
    }

    /// <summary>
    /// Kết thúc phiên học và lấy summary
    /// </summary>
    [HttpPost("end")]
    public async Task<ApiResponse<SessionSummaryDTO>> EndSessionAsync([FromBody] EndSessionRequest request)
    {
        var requestModel = new RequestDTO<EndSessionRequest>
        {
            UserId = GetCurrentUserId(),
            Request = request
        };
        return await HandleException(_sessionService.EndSessionAsync(requestModel));
    }
}
