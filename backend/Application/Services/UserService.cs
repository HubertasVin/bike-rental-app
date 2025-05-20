using BikeRentalApp.Application.DTOs;
using BikeRentalApp.Application.Services.Interfaces;
using BikeRentalApp.Domain.Entities;
using BikeRentalApp.Repositories.Interfaces;

namespace BikeRentalApp.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly JwtService _jwtService;

    public UserService(IUserRepository userRepository, JwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    public async Task<UserDTO> CreateUserAsync(UserDTO dto)
    {

    var exists = await _userRepository.EmailExistsAsync(dto.Email);
    if (exists)
        throw new ArgumentException("Email already exists");

        var user = new User(
            Guid.NewGuid(),
            dto.Name,
            dto.Email,
            dto.Password
        );

        var created = await _userRepository.CreateAsync(user);
        return MapToDto(created);
    }

    public async Task<UserDTO?> UpdateUserAsync(Guid id, UpdateUserDTO updateUserDto)
    {
        var existingUser = await _userRepository.GetByIdAsync(id);
        if (existingUser == null)
        {
            return null;
        }

        var updatedUser = new User(
            existingUser.Id,
            updateUserDto.Name,
            updateUserDto.Email,
            updateUserDto.Password
        );

        var result = await _userRepository.UpdateAsync(updatedUser);
        return result != null ? MapToDto(result) : null;
    }
    
    public async Task<AuthDTO> LoginAsync(LoginDTO dto)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        var token = _jwtService.GenerateJwtToken(user);
        return new AuthDTO(token);
    }

    private UserDTO MapToDto(User user)
    {
        return new UserDTO
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Password = user.PasswordHash
        };

    }
}