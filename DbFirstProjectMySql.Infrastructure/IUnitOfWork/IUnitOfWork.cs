using DbFirstProjectMySql.Infrastructure.Entities;
using DbFirstProjectMySql.Infrastructure.Repositories;

namespace DbFirstProjectMySql.Infrastructure.IUnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<User> UserRepository { get; }
        IRepository<Role> RoleRepository { get; }
        IRepository<Product> ProductRepository { get; }
        Task SaveAsync();
    }

}
