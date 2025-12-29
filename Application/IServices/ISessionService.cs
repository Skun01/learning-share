using Application.DTOs.Common;
using Application.DTOs.Study;

namespace Application.IServices;

public interface ISessionService
{
    Task<SessionDTO> StartSessionAsync(RequestDTO<StartSessionRequest> request);
    Task<SessionDTO> SubmitSessionAsync(RequestDTO<SubmitSessionRequest> request);
    Task<SessionSummaryDTO> EndSessionAsync(RequestDTO<EndSessionRequest> request);
}
