namespace Blog.Api.Dtos.Contributors;

public record UpdateContributorDto(
    string DisplayName,
    string Email,
    string? FirstName,
    IReadOnlyList<string>? MiddleNames,
    string? LastName,
    string? AvatarUrl,
    string? Bio
);