namespace Blog.Api.Models;

public class User
{
  public int Id { get; set; }
  public required string Email { get; set; }
  public required string PasswordHash { get; set; }
  public DateTimeOffset CreatedAt { get; set; }
  public DateTimeOffset UpdatedAt { get; set; }

  public Contributor Contributor { get; set; } = null!;
}