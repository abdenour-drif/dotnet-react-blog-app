using Blog.Api.Data;
using Blog.Api.Dtos.Blogs;
using Blog.Api.Helpers;
using Microsoft.EntityFrameworkCore;
using BlogModel = Blog.Api.Models.Blog;
using BlogContributorModel = Blog.Api.Models.BlogContributor;

namespace Blog.Api.Features.Services;

public class BlogService(BlogStoreContext dbContext) : IBlogService
{
  public async Task<IReadOnlyList<BlogDto>> GetBlogsAsync(CancellationToken ct = default)
  {
    return await dbContext.Blogs
        .AsNoTracking()
        .Select(blog => new BlogDto(
          blog.Id,
          blog.Slug,
          blog.Title,
          blog.Subtitle,
          blog.Intro,
          blog.Content,
          blog.Summary,
          blog.CoverImageUrl,
          blog.CreatedAt,
          blog.UpdatedAt,
          blog.PublishedAt,
          blog.Contributors
          .Select(contributor => new BlogContributorDetailsDto(
              contributor.ContributorId,
              contributor.Role,
              contributor.Contributor.DisplayName,
              contributor.Contributor.AvatarUrl
          ))
          .ToList(),
          blog.Tags.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList()
        ))
        .ToListAsync(ct);
  }

  public async Task<BlogDto?> GetBlogBySlugAsync(string slug, CancellationToken ct = default)
  {
    return await dbContext.Blogs
        .AsNoTracking()
        .Where(blog => blog.Slug == slug)
        .Select(blog => new BlogDto(
          blog.Id,
          blog.Slug,
          blog.Title,
          blog.Subtitle,
          blog.Intro,
          blog.Content,
          blog.Summary,
          blog.CoverImageUrl,
          blog.CreatedAt,
          blog.UpdatedAt,
          blog.PublishedAt,
          blog.Contributors
          .Select(contributor => new BlogContributorDetailsDto(
              contributor.ContributorId,
              contributor.Role,
              contributor.Contributor.DisplayName,
              contributor.Contributor.AvatarUrl
          ))
          .ToList(),
          blog.Tags.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList()
        ))
        .FirstOrDefaultAsync(ct);
  }

  public async Task<BlogDto> CreateAsync(CreateBlogDto newBlog, CancellationToken ct = default)
  {
    var existingSlugs = await dbContext.Blogs
      .AsNoTracking()
      .Select(blog => blog.Slug)
      .ToListAsync(ct);

    var slug = SlugHelper.CreateUniqueSlug(newBlog.Title, existingSlugs);

    BlogModel blog = new()
    {
      Slug = slug,
      Title = newBlog.Title,
      Subtitle = newBlog.Subtitle,
      Intro = newBlog.Intro,
      Content = newBlog.Content,
      Summary = newBlog.Summary,
      CoverImageUrl = newBlog.CoverImageUrl,
      CreatedAt = DateTimeOffset.UtcNow,
      UpdatedAt = DateTimeOffset.UtcNow,
      PublishedAt = null,
      Contributors = [],
      Tags = string.Join(",", newBlog.Tags ?? [])
    };

    blog.Contributors = [
      .. newBlog.Contributors.Select(assignment => new BlogContributorModel
      {
          Blog = blog,
          ContributorId = assignment.ContributorId,
          Role = assignment.Role
      })
    ];

    dbContext.Blogs.Add(blog);
    await dbContext.SaveChangesAsync(ct);

    await dbContext.Entry(blog)
      .Collection(b => b.Contributors)
      .Query()
      .Include(bc => bc.Contributor)
      .LoadAsync(ct);

    return new BlogDto(
      blog.Id,
      blog.Slug,
      blog.Title,
      blog.Subtitle,
      blog.Intro,
      blog.Content,
      blog.Summary,
      blog.CoverImageUrl,
      blog.CreatedAt,
      blog.UpdatedAt,
      blog.PublishedAt,
      [.. blog.Contributors.Select(c => new BlogContributorDetailsDto(
        c.ContributorId,
        c.Role,
        c.Contributor.DisplayName,
        c.Contributor.AvatarUrl
      ))],
      newBlog.Tags ?? []
    );
  }

  public async Task<BlogDto?> UpdateAsync(string slug, UpdateBlogDto updatedBlog, CancellationToken ct = default)
  {
    var blog = await dbContext.Blogs
      .Include(b => b.Contributors)
      .ThenInclude(bc => bc.Contributor)
      .FirstOrDefaultAsync(b => b.Slug == slug, ct);

    if (blog is null)
      return null;

    blog.Title = updatedBlog.Title ?? blog.Title;
    blog.Subtitle = updatedBlog.Subtitle ?? blog.Subtitle;
    blog.Intro = updatedBlog.Intro ?? blog.Intro;
    blog.Content = updatedBlog.Content ?? blog.Content;
    blog.Summary = updatedBlog.Summary ?? blog.Summary;
    blog.CoverImageUrl = updatedBlog.CoverImageUrl ?? blog.CoverImageUrl;
    blog.UpdatedAt = DateTimeOffset.UtcNow;

    if (updatedBlog.Tags is not null)
    {
      blog.Tags = string.Join(",", updatedBlog.Tags);
    }

    if (updatedBlog.Contributors is not null)
    {
      var contributorIds = updatedBlog.Contributors.Select(c => c.ContributorId).Distinct().ToList();

      var existingContributorIds = await dbContext.Contributors
        .Where(c => contributorIds.Contains(c.Id))
        .Select(c => c.Id)
        .ToListAsync(ct);

      if (existingContributorIds.Count != contributorIds.Count)
        return null;

      dbContext.BlogContributors.RemoveRange(blog.Contributors);

      blog.Contributors = [.. updatedBlog.Contributors
        .Select(assignment => new BlogContributorModel
        {
          Blog = blog,
          ContributorId = assignment.ContributorId,
          Role = assignment.Role
        })];
    }
    await dbContext.SaveChangesAsync(ct);

    await dbContext.Entry(blog)
      .Collection(b => b.Contributors)
      .Query()
      .Include(bc => bc.Contributor)
      .LoadAsync(ct);

    return new BlogDto(
      blog.Id,
      blog.Slug,
      blog.Title,
      blog.Subtitle,
      blog.Intro,
      blog.Content,
      blog.Summary,
      blog.CoverImageUrl,
      blog.CreatedAt,
      blog.UpdatedAt,
      blog.PublishedAt,
      [.. blog.Contributors.Select(c => new BlogContributorDetailsDto(
        c.ContributorId,
        c.Role,
        c.Contributor.DisplayName,
        c.Contributor.AvatarUrl
      ))],
      blog.Tags.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList()
    );
  }

  public async Task<bool> DeleteAsync(string slug, CancellationToken ct = default)
  {
    var blog = await dbContext.Blogs
      .Include(b => b.Contributors)
      .FirstOrDefaultAsync(b => b.Slug == slug, ct);

    if (blog is null)
      return false;

    dbContext.Blogs.Remove(blog);
    await dbContext.SaveChangesAsync(ct);

    return true;
  }
}