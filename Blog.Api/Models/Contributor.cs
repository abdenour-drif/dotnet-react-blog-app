

namespace Blog.Api.Models;

public class Contributor
{
  public int Id { get; set; }
  public required string DisplayName { get; set; }
  public required string Email { get; set; }
  public required string FirstName { get; set; }
  public string MiddleNames { get; set; } = string.Empty;
  public required string LastName { get; set; }
  public string? Bio { get; set; }
  public string? AvatarUrl { get; set; }
  public DateTimeOffset CreatedAt { get; set; }
  public DateTimeOffset UpdatedAt { get; set; }
}
