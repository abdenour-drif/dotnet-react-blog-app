using Blog.Api.Enums;
namespace Blog.Api.Dtos.Blogs;

public record BlogContributorAssignmentDto(
    int ContributorId,
    BlogContributorRole Role
);