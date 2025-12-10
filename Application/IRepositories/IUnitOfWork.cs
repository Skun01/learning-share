namespace Application.IRepositories;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IUserSettingsRepository UserSettings { get; }
    Task<int> SaveChangesAsync();
}
