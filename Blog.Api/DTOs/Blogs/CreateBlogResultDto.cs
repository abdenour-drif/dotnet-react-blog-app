using Blog.Api.Enums;

namespace Blog.Api.Dtos.Blogs;

public record CreateBlogResultDto(
  BlogDto? Blog,
  CreateBlogError Error,
  IReadOnlyList<int>? MissingContributorIds = null,
  string? ErrorMessage = null
);