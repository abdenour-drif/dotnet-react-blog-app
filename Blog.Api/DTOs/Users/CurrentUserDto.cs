namespace Blog.Api.Dtos.Users;

public record CurrentUserDto(
    int Id,
    string Email,
    int ContributorId,
    string DisplayName
);