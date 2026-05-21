using Blog.Api.Enums;

namespace Blog.Api.Models;

public class BlogContributor
{
  public int Id { get; set; }
  public Blog Blog { get; set; } = null!;
  public int BlogId { get; set; }
  public Contributor Contributor { get; set; } = null!;
  public int ContributorId { get; set; }
  public BlogContributorRole Role { get; set; }
}
