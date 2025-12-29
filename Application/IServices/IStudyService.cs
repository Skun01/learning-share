using Application.DTOs.Common;
using Application.DTOs.Study;

namespace Application.IServices;

public interface IStudyService
{
    Task<StudyCountDTO> GetStudyCountAsync(QueryDTO<GetStudyCountRequest> request);
    Task<IEnumerable<StudyCardDTO>> GetAvailableReviewsAsync(QueryDTO<GetAvailableReviewsRequest> request);
    Task<IEnumerable<StudyCardDTO>> GetNewLessonsAsync(QueryDTO<GetNewLessonsRequest> request);
    Task<SubmitReviewResponse> SubmitReviewAsync(int userId, int cardId, SubmitReviewRequest request);
    
    Task<IEnumerable<HeatmapDataDTO>> GetHeatmapAsync(QueryDTO<GetHeatmapRequest> request);
    Task<IEnumerable<ForecastDTO>> GetForecastAsync(QueryDTO<GetForecastRequest> request);
    Task<AccuracyDTO> GetAccuracyAsync(QueryDTO<GetAccuracyRequest> request);
    Task<LevelDistributionDTO> GetDistributionAsync(QueryDTO<GetDistributionRequest> request);
}
