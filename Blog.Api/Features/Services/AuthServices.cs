using Blog.Api.Data;
using Blog.Api.Dtos.Users;
using Blog.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Blog.Api.Features.Services;

public class AuthService(BlogStoreContext dbContext) : IAuthService
{
  public async Task<CurrentUserDto> RegisterAsync(RegisterUserDto dto, CancellationToken ct = default)
  {
    var emailExists = await dbContext.Users.AnyAsync(u => u.Email == dto.Email, ct);
    if (emailExists)
      throw new InvalidOperationException("Email already exists.");

    var user = new User
    {
      Email = dto.Email,
      PasswordHash = string.Empty,
      CreatedAt = DateTimeOffset.UtcNow,
      UpdatedAt = DateTimeOffset.UtcNow
    };

    var hasher = new PasswordHasher<User>();
    user.PasswordHash = hasher.HashPassword(user, dto.Password);

    var contributor = new Contributor
    {
      User = user,
      DisplayName = dto.DisplayName,
      Email = dto.Email,
      FirstName = dto.FirstName,
      MiddleNames = string.Join(",", dto.MiddleNames ?? []),
      LastName = dto.LastName,
      AvatarUrl = dto.AvatarUrl,
      Bio = dto.Bio,
      CreatedAt = DateTimeOffset.UtcNow,
      UpdatedAt = DateTimeOffset.UtcNow
    };

    dbContext.Users.Add(user);
    dbContext.Contributors.Add(contributor);
    await dbContext.SaveChangesAsync(ct);

    return new CurrentUserDto(user.Id, user.Email, contributor.Id, contributor.DisplayName);
  }

  public async Task<CurrentUserDto?> LoginAsync(LoginUserDto dto, CancellationToken ct = default)
  {
    var user = await dbContext.Users
      .AsNoTracking()
      .Include(u => u.Contributor)
      .FirstOrDefaultAsync(u => u.Email == dto.Email, ct);

    if (user is null)
      return null;

    var hasher = new PasswordHasher<User>();
    var result = hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);

    if (result == PasswordVerificationResult.Failed)
      return null;

    return new CurrentUserDto(user.Id, user.Email, user.Contributor.Id, user.Contributor.DisplayName);
  }
}