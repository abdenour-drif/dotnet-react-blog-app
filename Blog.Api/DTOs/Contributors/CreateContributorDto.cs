namespace Blog.Api.Dtos.Contributors;

public record CreateContributorDto(
    string DisplayName,
    string Email,
    string? FirstName,
    IReadOnlyList<string> MiddleNames,
    string? LastName,
    string? AvatarUrl,
    string? Bio
);