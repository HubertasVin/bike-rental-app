using BikeRentalApp.Application.DTOs;

namespace BikeRentalApp.Application.Services.Interfaces;

public interface IUserService
{
    Task<UserDTO> CreateUserAsync(UserDTO dto);
    Task<UserDTO?> UpdateUserAsync(Guid id, UpdateUserDTO updateUserDto);
    Task<AuthDTO> LoginAsync(LoginDTO dto);
}