using System.Security.Claims;
using DiscussionBoard.Models;
using Microsoft.AspNetCore.Authorization;
namespace DiscussionBoard.Authorization;
public sealed class IsOwnerOrAdminHandler : AuthorizationHandler<IsOwnerOrAdminRequirement, DiscussionBase>
{
 protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsOwnerOrAdminRequirement requirement, DiscussionBase resource)
 {
  var id = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
  if (context.User.Identity?.IsAuthenticated == true &&
      (context.User.IsInRole(ApplicationRoles.Admin) || (!string.IsNullOrEmpty(id) && id == resource.ApplicationUserId)))
   context.Succeed(requirement);
  return Task.CompletedTask;
 }
}
