using Domain.Entities;

namespace Application.IRepositories;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IUserSettingsRepository UserSettings { get; }
    IDeckRepository Decks { get; }
    IRepository<DeckTag> DeckTags { get; }
    IUserCardProgressRepository UserCardProgresses { get; }
    ICardRepository Cards { get; }
    ICardExampleRepository CardExamples { get; }
    IGrammarDetailsRepository GrammarDetails { get; }
    IRepository<MediaFile> MediaFiles { get; }
    IStudyLogRepository StudyLogs { get; }
    IRefreshTokenRepository RefreshTokens { get; }
    Task<int> SaveChangesAsync();
}
