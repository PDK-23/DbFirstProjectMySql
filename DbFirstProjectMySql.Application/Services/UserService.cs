using AutoMapper;
using DbFirstProjectMySql.Application.DTOs;
using DbFirstProjectMySql.Application.Interfaces;
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

        public async Task<UserDto?> CreateAsync(UserCreateDto dto)
        {
            // Kiểm tra username đã tồn tại chưa
            var existingUser = (await _unitOfWork.UserRepository.GetAllAsync())
                .FirstOrDefault(u => u.Username == dto.Username);

            if (existingUser != null)
                return null; // Đã tồn tại, không cho tạo mới

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

        public async Task UpdateAsync(UserDto dto)
        {
            var user = _mapper.Map<User>(dto);
            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(int id)
        {
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


    }
}
