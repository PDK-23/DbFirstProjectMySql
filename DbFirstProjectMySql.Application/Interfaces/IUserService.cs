using DbFirstProjectMySql.Application.DTOs;
using DbFirstProjectMySql.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbFirstProjectMySql.Application.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<UserDto?> GetByIdAsync(int id);
        Task AddAsync(UserDto dto, string password);
        Task<UserDto?> CreateAsync(UserCreateDto dto);
        Task UpdateAsync(UserDto dto);
        Task DeleteAsync(int id);
        Task<UserDto?> GetByUsernameAsync(string username);
        Task<User?> GetEntityByUsernameAsync(string username); // Trả về entity để lấy PasswordHash

    }


}
