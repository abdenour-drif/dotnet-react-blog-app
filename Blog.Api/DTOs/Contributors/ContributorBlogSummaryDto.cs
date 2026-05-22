using Blog.Api.Enums;

namespace Blog.Api.Dtos.Contributors;

public record ContributorBlogSummaryDto(
    string Slug,
    string Title,
    BlogContributorRole Role
);