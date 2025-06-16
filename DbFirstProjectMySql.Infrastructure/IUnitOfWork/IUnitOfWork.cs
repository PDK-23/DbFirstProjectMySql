using DbFirstProjectMySql.Domain.Entities;
using DbFirstProjectMySql.Infrastructure.Entities;
using DbFirstProjectMySql.Infrastructure.Repositories;

namespace DbFirstProjectMySql.Infrastructure.IUnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<User> UserRepository { get; }
        IRepository<Role> RoleRepository { get; }
        IRepository<Product> ProductRepository { get; }
        IRepository<RefreshToken> RefreshTokenRepository { get; }
        Task SaveAsync();
    }

}
