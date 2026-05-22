namespace Blog.Api.Enums;

public enum AddBlogContributorErrorEnum
{
  None,
  BlogNotFound,
  ContributorNotFound,
  ContributorAlreadyAssigned,
  RoleNotFound,
  Forbidden,
  SaveFailed
}