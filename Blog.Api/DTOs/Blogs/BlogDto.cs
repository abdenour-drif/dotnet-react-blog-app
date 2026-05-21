namespace Blog.Api.Dtos.Blogs;

public record BlogDto(
  int Id,
  string Slug,
  string Title,
  string Subtitle,
  string Intro,
  string Content,
  string Summary,
  string? CoverImageUrl,
  DateTimeOffset CreatedAt,
  DateTimeOffset UpdatedAt,
  DateTimeOffset? PublishedAt,
  IReadOnlyList<BlogContributorDetailsDto> Contributors,
  IReadOnlyList<string> Tags
);


