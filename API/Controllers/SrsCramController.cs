using Application.Common;
using Application.DTOs.Common;
using Application.DTOs.Study;
using Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/srs/cram")]
[ApiController]
[Authorize]
public class SrsCramController : BaseController
{
    private readonly ICramService _cramService;

    public SrsCramController(ICramService cramService)
    {
        _cramService = cramService;
    }

    /// <summary>
    /// Bắt đầu phiên Cram (practice bất kể schedule)
    /// </summary>
    [HttpPost("start")]
    public async Task<ApiResponse<CramSessionDTO>> StartCramAsync([FromBody] StartCramRequest request)
    {
        var requestModel = new RequestDTO<StartCramRequest>
        {
            UserId = GetCurrentUserId(),
            Request = request
        };
        return await HandleException(_cramService.StartCramAsync(requestModel));
    }

    /// <summary>
    /// Submit kết quả và lấy card tiếp theo (không update SRS)
    /// </summary>
    [HttpPost("submit")]
    public async Task<ApiResponse<CramSessionDTO>> SubmitCramAsync([FromBody] SubmitCramRequest request)
    {
        var requestModel = new RequestDTO<SubmitCramRequest>
        {
            UserId = GetCurrentUserId(),
            Request = request
        };
        return await HandleException(_cramService.SubmitCramAsync(requestModel));
    }
}
