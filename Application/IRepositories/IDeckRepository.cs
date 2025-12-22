using Domain.Entities;

namespace Application.IRepositories;

public interface IDeckRepository : IRepository<Deck>
{
    Task<IEnumerable<Deck>> GetByUserId(int userId);
}
