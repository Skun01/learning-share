namespace Application.IRepositories;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IUserSettingsRepository UserSettings { get; }
    IDeckRepository Decks { get; }
    IUserCardProgressRepository UserCardProgresses { get; }
    ICardRepository Cards { get; }
    Task<int> SaveChangesAsync();
}
