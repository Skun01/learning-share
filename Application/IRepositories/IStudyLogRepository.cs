using Domain.Entities;

namespace Application.IRepositories;

public interface IStudyLogRepository : IRepository<StudyLog>
{
    Task<IEnumerable<StudyLog>> GetByUserIdAsync(int userId, int? limit = null);
    Task<IEnumerable<StudyLog>> GetByDateRangeAsync(int userId, DateTime startDate, DateTime endDate);
}
