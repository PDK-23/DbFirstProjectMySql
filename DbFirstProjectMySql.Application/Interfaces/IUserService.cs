using DbFirstProjectMySql.Application.DTOs;
using DbFirstProjectMySql.Domain.Entities;
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
        Task SetUserRefreshToken(int userId, string refreshToken);
        Task<RefreshToken?> GetValidRefreshTokenAsync(string token);
        Task RevokeRefreshTokenAsync(RefreshToken token);
        Task AddRefreshToken(int userId, string refreshToken, DateTime expiryTime);
        Task<ChangePasswordResult> ChangePasswordAsync(int userId, string oldPassword, string newPassword);
    }


}
