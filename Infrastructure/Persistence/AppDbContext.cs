using System.Reflection;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<UserSettings> UserSettings { get; set; }
    public DbSet<Deck> Decks { get; set; }
    public DbSet<DeckTag> DeckTags { get; set; }
    public DbSet<Card> Cards { get; set; }
    public DbSet<GrammarDetails> GrammarDetails { get; set; } 
    public DbSet<CardExample> CardExamples { get; set; }
    public DbSet<UserCardProgress> UserCardProgresses { get; set; }
    public DbSet<StudyLog> StudyLogs { get; set; }
    public DbSet<MediaFile> MediaFiles { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
