namespace Blog.Api.Dtos.Users;

public record LoginUserDto(
    string Email,
    string Password
);