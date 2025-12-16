using Domain.Entities;

namespace Application.IRepositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByPasswordResetTokenAsync(string token);
}
