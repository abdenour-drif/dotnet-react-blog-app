namespace Blog.Api.Dtos.Blogs;

public record UpdateBlogDto(
    string? Title,
    string? Subtitle,
    string? Intro,
    string? Content,
    string? Summary,
    string? CoverImageUrl,
    IReadOnlyList<BlogContributorAssignmentDto>? Contributors,
    IReadOnlyList<string>? Tags
);