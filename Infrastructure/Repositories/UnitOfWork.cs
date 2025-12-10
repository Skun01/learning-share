using Application.IRepositories;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{   
    public IUserRepository Users { get; private set; }
    public IUserSettingsRepository UserSettings { get; private set; }
    private readonly AppDbContext _context;
    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Users = new UserRepository(_context);
        UserSettings = new UserSettingsRepository(_context);
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
