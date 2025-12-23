using Application.IRepositories;
using Domain.Entities;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories;

public class CardExampleRepository : Repository<CardExample>, ICardExampleRepository
{
    public CardExampleRepository(AppDbContext context) : base(context) {}
}
