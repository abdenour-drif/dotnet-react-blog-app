

namespace Blog.Api.Models;

public class Blog
{
  public int Id { get; set; }
  public required string Slug { get; set; }
  public required string Title { get; set; }
  public required string Subtitle { get; set; }
  public required string Intro { get; set; }
  public required string Content { get; set; }
  public required string Summary { get; set; }
  public string? CoverImageUrl { get; set; }
  public DateTimeOffset CreatedAt { get; set; }
  public DateTimeOffset UpdatedAt { get; set; }
  public DateTimeOffset? PublishedAt { get; set; }
  public List<BlogContributor> Contributors { get; set; } = [];
  public string Tags { get; set; } = string.Empty;
}
