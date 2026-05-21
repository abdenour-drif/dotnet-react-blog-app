using Blog.Api.Dtos.Contributors;

namespace Blog.Api.Dtos.Blogs;
public record BlogListItemDto(
    int Id,
    string Slug,
    string Title,
    string Subtitle,
    string Intro,
    string? CoverImageUrl,
    IReadOnlyList<BlogContributorDto> Contributors,
    IReadOnlyList<string> Tags,
    DateTimeOffset? PublishedAt
);
