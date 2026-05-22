namespace Blog.Api.Dtos.Users;

public record UserDto(
    int Id,
    string Email,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);