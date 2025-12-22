using Domain.Entities;

namespace Application.IRepositories;

public interface IUserCardProgressRepository : IRepository<UserCardProgress>
{
    Task<IEnumerable<UserCardProgress>> GetByUserId(int userId);
    Task<IEnumerable<UserCardProgress>> GetByListCardId(IEnumerable<int> cardIds);
    
}
