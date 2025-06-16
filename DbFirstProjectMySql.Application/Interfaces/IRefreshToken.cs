using DbFirstProjectMySql.Domain.Entities;

namespace DbFirstProjectMySql.Application.Interfaces
{
    public interface IRefreshToken
    {
        Task RevokeRefreshTokenAsync(RefreshToken token);
        Task AddRefreshToken(int userId, string refreshToken, DateTime expiryTime);
        Task SetUserRefreshToken(int userId, string refreshToken);
        Task<RefreshToken?> GetValidRefreshTokenAsync(string token);
    }
}
