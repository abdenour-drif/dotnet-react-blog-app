namespace Blog.Api.Enums;

public enum UpdateBlogContributorRoleErrorEnum
{
  None,
  BlogNotFound,
  ContributorNotFound,
  ContributorNotAssignedToBlog,
  LastAuthorCannotBeChanged,
  RoleNotFound,
  Forbidden,
  SaveFailed
}