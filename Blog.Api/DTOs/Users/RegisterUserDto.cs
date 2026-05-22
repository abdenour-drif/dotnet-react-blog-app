namespace Blog.Api.Dtos.Users;

public record RegisterUserDto(
    string Email,
    string Password,
    string DisplayName,
    string FirstName,
    IReadOnlyList<string>? MiddleNames,
    string LastName,
    string? AvatarUrl,
    string? Bio
);