using Domain.Entities;

namespace Application.IRepositories;

public interface IUserSettingsRepository : IRepository<UserSettings>
{
    Task<UserSettings?> GetByUserIdAsync(int userId);
}
