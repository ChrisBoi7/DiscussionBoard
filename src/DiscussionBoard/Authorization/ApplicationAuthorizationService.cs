using System.Security.Claims;
using DiscussionBoard.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
namespace DiscussionBoard.Authorization;
public class ApplicationAuthorizationService(IAuthorizationService authorization, UserManager<ApplicationUser> users) : IApplicationAuthorizationService
{
 public async Task<bool> CanEditAsync(ClaimsPrincipal principal, DiscussionBase resource) =>
  (await authorization.AuthorizeAsync(principal, resource, ApplicationAuthorizationPolicies.IsOwnerOrAdmin)).Succeeded;
 private static IdentityResult Fail(string text) => IdentityResult.Failed(new IdentityError { Description = text });
 private async Task<bool> IsCurrentAdminAsync(ClaimsPrincipal principal)
 {
  if (principal.Identity?.IsAuthenticated != true || !principal.IsInRole(ApplicationRoles.Admin)) return false;
  var id = principal.FindFirstValue(ClaimTypes.NameIdentifier);
  var actor = id is null ? null : await users.FindByIdAsync(id);
  return actor != null && await users.IsInRoleAsync(actor, ApplicationRoles.Admin);
 }
 public async Task<IdentityResult> AddRoleAsync(ClaimsPrincipal actingUser, string targetUserId, string roleName)
 {
  if (!await IsCurrentAdminAsync(actingUser)) return Fail("Administrator access is required.");
  if (roleName != ApplicationRoles.Admin || string.IsNullOrWhiteSpace(targetUserId)) return Fail("Invalid user or role.");
  var target = await users.FindByIdAsync(targetUserId);
  if (target is null) return Fail("User was not found.");
  return await users.AddToRoleAsync(target, ApplicationRoles.Admin);
 }
 public async Task<IdentityResult> RemoveRoleAsync(ClaimsPrincipal actingUser, string targetUserId, string roleName)
 {
  if (!await IsCurrentAdminAsync(actingUser)) return Fail("Administrator access is required.");
  if (roleName != ApplicationRoles.Admin || string.IsNullOrWhiteSpace(targetUserId)) return Fail("Invalid user or role.");
  if (actingUser.FindFirstValue(ClaimTypes.NameIdentifier) == targetUserId) return Fail("You cannot remove your own administrator role.");
  var target = await users.FindByIdAsync(targetUserId);
  if (target is null) return Fail("User was not found.");
  if (!await users.IsInRoleAsync(target, ApplicationRoles.Admin)) return Fail("This user is not an administrator.");
  if ((await users.GetUsersInRoleAsync(ApplicationRoles.Admin)).Count <= 1) return Fail("The last administrator cannot be removed.");
  return await users.RemoveFromRoleAsync(target, ApplicationRoles.Admin);
 }
}
