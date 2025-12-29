using Application.IRepositories;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class StudyLogRepository : Repository<StudyLog>, IStudyLogRepository
{
    public StudyLogRepository(AppDbContext context) : base(context) {}

    public async Task<IEnumerable<StudyLog>> GetByUserIdAsync(int userId, int? limit = null)
    {
        var query = _context.StudyLogs
            .Where(sl => sl.UserId == userId)
            .OrderByDescending(sl => sl.ReviewDate);

        if (limit.HasValue)
        {
            return await query.Take(limit.Value).ToListAsync();
        }

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<StudyLog>> GetByDateRangeAsync(int userId, DateTime startDate, DateTime endDate)
    {
        return await _context.StudyLogs
            .Where(sl => sl.UserId == userId 
                && sl.ReviewDate >= startDate 
                && sl.ReviewDate <= endDate)
            .OrderBy(sl => sl.ReviewDate)
            .ToListAsync();
    }
}
