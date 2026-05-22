using Blog.Api.Enums;

namespace Blog.Api.Dtos.Blogs;

public record UpdateBlogResultDto(
  BlogDto? Blog,
  UpdateBlogError Error,
  IReadOnlyList<int>? MissingContributorIds = null,
  string? ErrorMessage = null
);