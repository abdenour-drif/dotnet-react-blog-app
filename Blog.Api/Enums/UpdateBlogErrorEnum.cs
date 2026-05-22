namespace Blog.Api.Enums;

public enum UpdateBlogError
{
  None,
  BlogNotFound,
  ContributorsUpdateNotAllowed,
  Forbidden,
  SaveFailed
}