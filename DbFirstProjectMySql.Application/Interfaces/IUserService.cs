using DbFirstProjectMySql.Application.DTOs;
using DbFirstProjectMySql.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DbFirstProjectMySql.Application.Services.UserService;

namespace DbFirstProjectMySql.Application.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<UserDto?> GetByIdAsync(int id);
        Task AddAsync(UserDto dto, string password);
        Task<UserDto?> CreateAsync(UserCreateDto dto);
        Task UpdateAsync(UserEditDto dto, int id);
        Task DeleteAsync(int id);
        Task<UserDto?> GetByUsernameAsync(string username);
        Task<User?> GetEntityByUsernameAsync(string username); // Trả về entity để lấy PasswordHash
        Task<ChangePasswordResult> ChangePasswordAsync(int userId, string oldPassword, string newPassword);
    }


}
