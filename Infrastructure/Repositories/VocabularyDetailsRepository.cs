using Application.IRepositories;
using Domain.Entities;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories;

public class VocabularyDetailsRepository : Repository<VocabularyDetails>, IVocabularyDetailsRepository
{
    public VocabularyDetailsRepository(AppDbContext context) : base(context) {}
}
