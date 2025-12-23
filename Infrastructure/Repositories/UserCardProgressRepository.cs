using Application.IRepositories;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserCardProgressRepository : Repository<UserCardProgress>, IUserCardProgressRepository
{
    public UserCardProgressRepository(AppDbContext context) : base(context){}

    public async Task<IEnumerable<UserCardProgress>> GetByUserId(int userId)
    {
        return await _context.UserCardProgresses.Where(ucp => ucp.UserId == userId).ToListAsync();
    }

    public async Task<IEnumerable<UserCardProgress>> GetByListCardId(IEnumerable<int> cardIds, int userId)
    {
        return await _context.UserCardProgresses
            .Where(ucp => ucp.UserId == userId && cardIds.Contains(ucp.CardId))
            .ToListAsync();
    }
}
