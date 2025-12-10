using Application.IRepositories;
using Domain.Entities;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories;

public class UserSettingsRepository : Repository<UserSettings>, IUserSettingsRepository
{
    public UserSettingsRepository(AppDbContext context) : base(context) {}
}
