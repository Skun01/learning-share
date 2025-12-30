using Application.DTOs.Common;
using Application.DTOs.Study;

namespace Application.IServices;

public interface ICramService
{
    Task<CramSessionDTO> StartCramAsync(RequestDTO<StartCramRequest> request);
    Task<CramSessionDTO> SubmitCramAsync(RequestDTO<SubmitCramRequest> request);
}
