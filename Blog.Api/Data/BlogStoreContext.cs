using UserModel = Blog.Api.Models.User;
using BlogModel = Blog.Api.Models.Blog;
using ContributorModel = Blog.Api.Models.Contributor;
using BlogContributorModel = Blog.Api.Models.BlogContributor;
using Microsoft.EntityFrameworkCore;

namespace Blog.Api.Data;

public class BlogStoreContext(DbContextOptions<BlogStoreContext> options) : DbContext(options)
{
  public DbSet<BlogModel> Blogs => Set<BlogModel>();
  public DbSet<ContributorModel> Contributors => Set<ContributorModel>();
  public DbSet<BlogContributorModel> BlogContributors => Set<BlogContributorModel>();
  public DbSet<UserModel> Users => Set<UserModel>();
}