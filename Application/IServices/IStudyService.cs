using Application.DTOs.Common;
using Application.DTOs.Study;

namespace Application.IServices;

public interface IStudyService
{
    Task<StudyCountDTO> GetStudyCountAsync(QueryDTO<GetStudyCountRequest> request);
    Task<IEnumerable<StudyCardDTO>> GetAvailableReviewsAsync(QueryDTO<GetAvailableReviewsRequest> request);
    Task<IEnumerable<StudyCardDTO>> GetNewLessonsAsync(QueryDTO<GetNewLessonsRequest> request);
}
