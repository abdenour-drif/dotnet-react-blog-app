using Blog.Api.Enums;

namespace Blog.Api.Dtos.Blogs;

public record AddBlogContributorResultDto(
  BlogContributorAssignmentDto? Contributors,
  AddBlogContributorErrorEnum Error,
  string? ErrorMessage = null
);