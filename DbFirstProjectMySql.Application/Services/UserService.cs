using AutoMapper;
using DbFirstProjectMySql.Application.DTOs;
using DbFirstProjectMySql.Application.Interfaces;
using DbFirstProjectMySql.Domain.Entities;
using DbFirstProjectMySql.Domain.Enums;
using DbFirstProjectMySql.Infrastructure.Entities;
using DbFirstProjectMySql.Infrastructure.IUnitOfWork;
using static BCrypt.Net.BCrypt;

namespace DbFirstProjectMySql.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = await _unitOfWork.UserRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<UserDto?> GetByIdAsync(int id)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto?> CreateAsync(UserCreateDto dto, int currentUserRole)
        {
            if (currentUserRole != (int)RoleEnum.Admin)
                throw new UnauthorizedAccessException("Only admin can create users.");

            var existingUser = (await _unitOfWork.UserRepository.GetAllAsync())
                .FirstOrDefault(u => u.Username == dto.Username);

            if (existingUser != null)
                return null;

            var user = _mapper.Map<User>(dto);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            await _unitOfWork.UserRepository.AddAsync(user);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<UserDto>(user);
        }
        public async Task<UserDto?> RegisterAsync(UserCreateDto dto)
        {
            var existingUser = (await _unitOfWork.UserRepository.GetAllAsync())
                .FirstOrDefault(u => u.Username == dto.Username);

            if (existingUser != null)
                return null;

            var user = _mapper.Map<User>(dto);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            await _unitOfWork.UserRepository.AddAsync(user);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<UserDto>(user);
        }

        public async Task AddAsync(UserDto dto, string password)
        {
            var user = _mapper.Map<User>(dto);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            await _unitOfWork.UserRepository.AddAsync(user);
            await _unitOfWork.SaveAsync();
        }
        public async Task UpdateAsync(UserEditDto dto, int id, int currentUserRole)
        {
            if (currentUserRole != (int)RoleEnum.Admin)
                throw new UnauthorizedAccessException("Only admin can update users.");

            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
            if (user == null)
                throw new Exception("User not found!");

            var allUsers = await _unitOfWork.UserRepository.GetAllAsync();
            var existed = allUsers.Any(u => u.Username == dto.Username && u.Id != id);
            if (existed)
                throw new Exception("Username already exists!");

            user.Username = dto.Username;

            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveAsync();
        }
        public async Task DeleteAsync(int id, int currentUserRole)
        {
            if (currentUserRole != (int)RoleEnum.Admin)
                throw new UnauthorizedAccessException("Only admin can delete users.");

            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
            if (user != null)
            {
                _unitOfWork.UserRepository.Delete(user);
                await _unitOfWork.SaveAsync();
            }
        }

        public async Task<UserDto?> GetByUsernameAsync(string username)
        {
            var users = await _unitOfWork.UserRepository.GetAllAsync();
            var user = users.FirstOrDefault(u => u.Username == username);
            return _mapper.Map<UserDto>(user);
        }

        public async Task<User?> GetEntityByUsernameAsync(string username)
        {
            var users = await _unitOfWork.UserRepository.GetAllAsync();
            return users.FirstOrDefault(u => u.Username == username);
        }

        public class ChangePasswordResult
        {
            public bool Success { get; set; }
            public string? Error { get; set; }
        }

        public async Task<ChangePasswordResult> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user == null)
                return new ChangePasswordResult { Success = false, Error = "User not found" };

            // Kiểm tra password cũ
            if (!BCrypt.Net.BCrypt.Verify(oldPassword, user.PasswordHash))
                return new ChangePasswordResult { Success = false, Error = "Old password is incorrect" };

            // Hash password mới và update
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);

            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveAsync();
            return new ChangePasswordResult { Success = true };
        }
        
    }
}
