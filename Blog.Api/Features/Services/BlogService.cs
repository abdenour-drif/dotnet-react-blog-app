using Blog.Api.Data;
using Blog.Api.Dtos.Blogs;
using Blog.Api.Helpers;
using Microsoft.EntityFrameworkCore;
using Blog.Api.Enums;
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

  private async Task<BlogContributorRole?> GetCurrentUserRoleAsync(
  string slug,
  int currentContributorId,
  CancellationToken ct)
  {
    return await dbContext.BlogContributors
      .Where(bc => bc.Blog.Slug == slug && bc.ContributorId == currentContributorId)
      .Select(bc => (BlogContributorRole?)bc.Role)
      .FirstOrDefaultAsync(ct);
  }

  public async Task<CreateBlogResultDto> CreateAsync(CreateBlogDto newBlog, int currentContributorId, CancellationToken ct = default)
  {
    var currentContributorExists = await dbContext.Contributors
  .AnyAsync(c => c.Id == currentContributorId, ct);

    if (!currentContributorExists)
    {
      return new CreateBlogResultDto(
        null,
        CreateBlogError.Forbidden
      );
    }

    var existingSlugs = await dbContext.Blogs
          .AsNoTracking()
          .Select(blog => blog.Slug)
          .ToListAsync(ct);

    var slug = SlugHelper.CreateUniqueSlug(newBlog.Title, existingSlugs);

    try
    {
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

      blog.Contributors =
      [
        new BlogContributorModel
        {
          Blog = blog,
          ContributorId = currentContributorId,
          Role = BlogContributorRole.Author
        }
      ];

      dbContext.Blogs.Add(blog);
      await dbContext.SaveChangesAsync(ct);

      await dbContext.Entry(blog)
        .Collection(b => b.Contributors)
        .Query()
        .Include(bc => bc.Contributor)
        .LoadAsync(ct);


      BlogDto blogDto = new(
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

      return new CreateBlogResultDto(
        blogDto,
        CreateBlogError.None
      );
    }
    catch
    {
      return new CreateBlogResultDto(
        null,
        CreateBlogError.SaveFailed,
        null,
        "Failed to save blog."
      );
    }
  }

  public async Task<AddBlogContributorResultDto> AddBlogContributorAsync(string slug, int currentContributorId, BlogContributorAssignmentDto contributorAssignment, CancellationToken ct = default)
  {
    if (!Enum.IsDefined(typeof(BlogContributorRole), contributorAssignment.Role))
    {
      return new AddBlogContributorResultDto(
        null,
        AddBlogContributorErrorEnum.RoleNotFound
      );
    }

    var currentUserRole = await GetCurrentUserRoleAsync(slug, currentContributorId, ct);

    if (currentUserRole is null || currentUserRole != BlogContributorRole.Author)
    {
      return new AddBlogContributorResultDto(
        null,
        AddBlogContributorErrorEnum.Forbidden
      );
    }

    var blog = await dbContext.Blogs
      .Include(b => b.Contributors)
      .ThenInclude(bc => bc.Contributor)
      .FirstOrDefaultAsync(b => b.Slug == slug, ct);

    if (blog is null)
    {
      return new AddBlogContributorResultDto(
        null,
        AddBlogContributorErrorEnum.BlogNotFound
      );
    }

    var alreadyAssigned = blog.Contributors.Any(c => c.ContributorId == contributorAssignment.ContributorId);
    if (alreadyAssigned)
    {
      return new AddBlogContributorResultDto(
        null,
        AddBlogContributorErrorEnum.ContributorAlreadyAssigned
      );
    }

    var contributorExists = await dbContext.Contributors
      .AnyAsync(c => c.Id == contributorAssignment.ContributorId, ct);

    if (!contributorExists)
    {
      return new AddBlogContributorResultDto(
        null,
        AddBlogContributorErrorEnum.ContributorNotFound
      );
    }

    try
    {
      BlogContributorAssignmentDto contributor = new(
        contributorAssignment.ContributorId,
        contributorAssignment.Role
      );

      dbContext.BlogContributors.Add(new BlogContributorModel
      {
        BlogId = dbContext.Blogs
          .Where(b => b.Slug == slug)
          .Select(b => b.Id)
          .FirstOrDefault(),
        ContributorId = contributor.ContributorId,
        Role = contributor.Role
      });
      await dbContext.SaveChangesAsync(ct);

      return new AddBlogContributorResultDto(
        contributor,
        AddBlogContributorErrorEnum.None
      );
    }
    catch
    {
      return new AddBlogContributorResultDto(
        null,
        AddBlogContributorErrorEnum.SaveFailed,
        "Failed to save blog."
      );
    }
  }

  public async Task<UpdateBlogResultDto> UpdateAsync(string slug, int currentContributorId, UpdateBlogDto updatedBlog, CancellationToken ct = default)
  {
    // Load blog with contributors -> to update blog and its contributors in one transaction
    var blog = await dbContext.Blogs
      .Include(b => b.Contributors)
      .ThenInclude(bc => bc.Contributor)
      .FirstOrDefaultAsync(b => b.Slug == slug, ct);

    if (blog is null)
    {
      return new UpdateBlogResultDto(
        null,
        UpdateBlogError.BlogNotFound
      );
    }

    var role = await GetCurrentUserRoleAsync(slug, currentContributorId, ct);

    if (role is null)
    {
      return new UpdateBlogResultDto(
        null,
        UpdateBlogError.Forbidden
      );
    }

    if (role is not (BlogContributorRole.Author or BlogContributorRole.Editor))
    {
      return new UpdateBlogResultDto(
        null,
        UpdateBlogError.Forbidden
      );
    }

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

    try
    {
      if (updatedBlog.Contributors is not null)
      {
        return new UpdateBlogResultDto(
          null,
          UpdateBlogError.ContributorsUpdateNotAllowed
        );
      }

      await dbContext.SaveChangesAsync(ct);

      await dbContext.Entry(blog)
        .Collection(b => b.Contributors)
        .Query()
        .Include(bc => bc.Contributor)
        .LoadAsync(ct);

      BlogDto blogDto = new(
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
        [.. blog.Tags.Split(",", StringSplitOptions.RemoveEmptyEntries)]
      );
      return new UpdateBlogResultDto(
        blogDto,
        UpdateBlogError.None
      );
    }
    catch
    {
      return new UpdateBlogResultDto(
        null,
        UpdateBlogError.SaveFailed
      );
    }
  }

  public async Task<UpdateBlogContributorResultDto> UpdateBlogContributorAsync(string slug, int contributorId, string role, int currentContributorId, CancellationToken ct = default)
  {
    var currentUserRole = await GetCurrentUserRoleAsync(slug, currentContributorId, ct);

    if (currentUserRole is null || currentUserRole != BlogContributorRole.Author)
    {
      return new UpdateBlogContributorResultDto(
        null,
        UpdateBlogContributorRoleErrorEnum.Forbidden
      );
    }
    var blog = await dbContext.Blogs
      .Include(b => b.Contributors)
      .ThenInclude(bc => bc.Contributor)
      .FirstOrDefaultAsync(b => b.Slug == slug, ct);

    if (blog is null)
    {
      return new UpdateBlogContributorResultDto(
        null,
        UpdateBlogContributorRoleErrorEnum.BlogNotFound
      );
    }

    var contributorExists = await dbContext.Contributors
        .AnyAsync(c => c.Id == contributorId, ct);

    if (!contributorExists)
    {
      return new UpdateBlogContributorResultDto(
          null,
          UpdateBlogContributorRoleErrorEnum.ContributorNotFound
      );
    }

    var blogContributor = blog.Contributors
        .FirstOrDefault(c => c.ContributorId == contributorId);

    if (blogContributor is null)
    {
      return new UpdateBlogContributorResultDto(
          null,
          UpdateBlogContributorRoleErrorEnum.ContributorNotAssignedToBlog
      );
    }
    if (!Enum.TryParse<BlogContributorRole>(role, true, out var parsedRole))
    {
      return new UpdateBlogContributorResultDto(
        null,
        UpdateBlogContributorRoleErrorEnum.RoleNotFound
      );
    }

    if (blogContributor.Role == BlogContributorRole.Author &&
    parsedRole != BlogContributorRole.Author)
    {
      var authorsCount = blog.Contributors.Count(c => c.Role == BlogContributorRole.Author);

      if (authorsCount <= 1)
      {
        return new UpdateBlogContributorResultDto(
            null,
            UpdateBlogContributorRoleErrorEnum.LastAuthorCannotBeChanged
        );
      }
    }

    blogContributor.Role = parsedRole;
    blog.UpdatedAt = DateTimeOffset.UtcNow;

    try
    {
      await dbContext.SaveChangesAsync(ct);
    }
    catch (DbUpdateException)
    {
      return new UpdateBlogContributorResultDto(
        null,
        UpdateBlogContributorRoleErrorEnum.SaveFailed
      );
    }


    return new UpdateBlogContributorResultDto(
      parsedRole,
      UpdateBlogContributorRoleErrorEnum.None
    );
  }
  public async Task<bool> DeleteAsync(string slug, int currentContributorId, CancellationToken ct = default)
  {
    var role = await GetCurrentUserRoleAsync(slug, currentContributorId, ct);

    if (role is null)
      return false;

    if (role != BlogContributorRole.Author)
      return false;

    var blog = await dbContext.Blogs
      .Include(b => b.Contributors)
      .FirstOrDefaultAsync(b => b.Slug == slug, ct);

    if (blog is null)
      return false;

    dbContext.Blogs.Remove(blog);
    await dbContext.SaveChangesAsync(ct);

    return true;
  }

  public async Task<bool> RemoveBlogContributorAsync(string slug, int contributorId, int currentContributorId, CancellationToken ct = default)
  {
    var currentUserRole = await GetCurrentUserRoleAsync(slug, currentContributorId, ct);

    if (currentUserRole is null || currentUserRole != BlogContributorRole.Author)
      return false;

    var blog = await dbContext.Blogs
      .Include(b => b.Contributors)
      .FirstOrDefaultAsync(b => b.Slug == slug, ct);

    if (blog is null)
      return false;

    var blogContributor = blog.Contributors
      .FirstOrDefault(c => c.ContributorId == contributorId);

    if (blogContributor is null)
      return false;

    if (blog.Contributors.Count <= 1)
      return false;

    if (blogContributor.Role == BlogContributorRole.Author)
    {
      var authorsCount = blog.Contributors.Count(c => c.Role == BlogContributorRole.Author);
      if (authorsCount <= 1)
        return false; // Blog must have at least one author
    }

    blog.Contributors.Remove(blogContributor);
    blog.UpdatedAt = DateTimeOffset.UtcNow;
    await dbContext.SaveChangesAsync(ct);

    return true;
  }
}