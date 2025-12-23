using Application.IRepositories;
using Domain.Entities;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories;

public class GrammarDetailsRepository : Repository<GrammarDetails>, IGrammarDetailsRepository
{
    public GrammarDetailsRepository(AppDbContext context) : base(context) {}
}
