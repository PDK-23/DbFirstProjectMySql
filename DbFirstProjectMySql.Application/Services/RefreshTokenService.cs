using AutoMapper;
using DbFirstProjectMySql.Application.Interfaces;
using DbFirstProjectMySql.Domain.Entities;
using DbFirstProjectMySql.Infrastructure.IUnitOfWork;

namespace DbFirstProjectMySql.Application.Services
{
    public class RefreshTokenService : IRefreshToken
    {
        private readonly IUnitOfWork _unitOfWork;

        public RefreshTokenService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task RevokeRefreshTokenAsync(RefreshToken token)
        {
            token.IsRevoked = true;
            _unitOfWork.RefreshTokenRepository.Update(token);
            await _unitOfWork.SaveAsync();
        }

        public async Task AddRefreshToken(int userId, string refreshToken, DateTime expiryTime)
        {
            var hashedToken = BCrypt.Net.BCrypt.HashPassword(refreshToken); // Hash token

            var token = new RefreshToken
            {
                UserId = userId,
                Token = hashedToken,
                ExpiryTime = expiryTime,
                IsRevoked = false
            };
            await _unitOfWork.RefreshTokenRepository.AddAsync(token);
            await _unitOfWork.SaveAsync();
        }
        public async Task SetUserRefreshToken(int userId, string refreshToken)
        {
            var expiryTime = DateTime.UtcNow.AddDays(7);
            var hashedToken = BCrypt.Net.BCrypt.HashPassword(refreshToken); // Hash token

            var token = new RefreshToken
            {
                UserId = userId,
                Token = hashedToken,
                ExpiryTime = expiryTime,
                IsRevoked = false
            };
            await _unitOfWork.RefreshTokenRepository.AddAsync(token);
            await _unitOfWork.SaveAsync();
        }
        public async Task<RefreshToken?> GetValidRefreshTokenAsync(string token)
        {
            var tokens = await _unitOfWork.RefreshTokenRepository.GetAllAsync();

            var validTokens = tokens
                .Where(rt => !rt.IsRevoked && rt.ExpiryTime > DateTime.UtcNow);

            foreach (var dbToken in validTokens)
            {
                if (BCrypt.Net.BCrypt.Verify(token, dbToken.Token))
                {
                    return dbToken;
                }
            }

            return null;
        }
    }
}
