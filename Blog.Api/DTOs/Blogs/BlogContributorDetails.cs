using System.ComponentModel.DataAnnotations;
using Blog.Api.Enums;

namespace Blog.Api.Dtos.Blogs;

public record BlogContributorDetailsDto(
    int ContributorId,
    BlogContributorRole Role,
    [Required] string DisplayName,
    string? AvatarUrl
);