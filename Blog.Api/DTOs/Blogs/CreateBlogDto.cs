using System.ComponentModel.DataAnnotations;

namespace Blog.Api.Dtos.Blogs;

public record CreateBlogDto(
    [Required][StringLength(50)] string Title,
    [Required][StringLength(80)] string Subtitle,
    [StringLength(2000)] string Intro,
    [Required] string Content,
    [StringLength(1000)] string Summary,
    [Url] string? CoverImageUrl,
    IReadOnlyList<BlogContributorAssignmentDto> Contributors,
    IReadOnlyList<string> Tags
);