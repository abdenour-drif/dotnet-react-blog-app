using Blog.Api.Enums;

namespace Blog.Api.Dtos.Blogs;

public record UpdateBlogContributorResultDto(
  BlogContributorRole? Role,
  UpdateBlogContributorRoleErrorEnum Error,
  string? ErrorMessage = null
);