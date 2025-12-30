using Application.IRepositories;
using Domain.Entities;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{   
    public IUserRepository Users { get; private set; }
    public IUserSettingsRepository UserSettings { get; private set; }
    public IDeckRepository Decks { get; private set; }
    public IRepository<DeckTag> DeckTags { get; private set; }
    public IUserCardProgressRepository UserCardProgresses { get; private set; }
    public ICardRepository Cards { get; private set;}
    public ICardExampleRepository CardExamples { get; private set; }
    public IGrammarDetailsRepository GrammarDetails { get; private set; }
    public IRepository<MediaFile> MediaFiles { get; private set; }
    public IStudyLogRepository StudyLogs { get; private set; }
    public IRefreshTokenRepository RefreshTokens { get; private set; }

    private readonly AppDbContext _context;
    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Users = new UserRepository(_context);
        UserSettings = new UserSettingsRepository(_context);
        Decks = new DeckRepository(_context);
        DeckTags = new Repository<DeckTag>(_context);
        UserCardProgresses = new UserCardProgressRepository(_context);
        Cards = new CardRepository(_context);
        CardExamples = new CardExampleRepository(_context);
        GrammarDetails = new GrammarDetailsRepository(_context);
        MediaFiles = new Repository<MediaFile>(_context);
        StudyLogs = new StudyLogRepository(_context);
        RefreshTokens = new RefreshTokenRepository(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
