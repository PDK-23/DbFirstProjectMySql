using DbFirstProjectMySql.Application.DTOs;

public interface IRoleService
{
    Task<IEnumerable<RoleDto>> GetAllAsync();
}
