using Blog.Api.Dtos.Users;

namespace Blog.Api.Features.Services;

public interface IAuthService
{
  Task<CurrentUserDto> RegisterAsync(RegisterUserDto dto, CancellationToken ct = default);
  Task<CurrentUserDto?> LoginAsync(LoginUserDto dto, CancellationToken ct = default);
}