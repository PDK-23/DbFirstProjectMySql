using DbFirstProjectMySql.Infrastructure.Data;
using DbFirstProjectMySql.Infrastructure.Entities;
using DbFirstProjectMySql.Infrastructure.IUnitOfWork;
using DbFirstProjectMySql.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public IRepository<User> UserRepository { get; }
    public IRepository<Role> RoleRepository { get; }
    public IRepository<Product> ProductRepository { get; }

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        UserRepository = new Repository<User>(context);
        RoleRepository = new Repository<Role>(context);
        ProductRepository = new Repository<Product>(context);
    }

    public async Task SaveAsync() => await _context.SaveChangesAsync();
    public void Dispose() => _context.Dispose();
}
