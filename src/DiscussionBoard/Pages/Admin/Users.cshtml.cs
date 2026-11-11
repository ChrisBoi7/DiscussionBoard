using DiscussionBoard.Authorization;
using DiscussionBoard.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
namespace DiscussionBoard.Pages.Admin;
[Authorize(Roles = ApplicationRoles.Admin)]
public class UsersModel(UserManager<ApplicationUser> users, IApplicationAuthorizationService authorization) : PageModel
{
 [BindProperty(SupportsGet = true)] public string? SelectedUserId { get; set; }
 [BindProperty] public string Role { get; set; } = ApplicationRoles.Admin;
 [BindProperty] public string Action { get; set; } = "";
 public List<ApplicationUser> AvailableUsers { get; private set; } = [];
 public ApplicationUser? SelectedUser { get; private set; }
 public bool IsAdmin { get; private set; }
 [TempData] public string? StatusMessage { get; set; }
 private async Task LoadAsync() {
  AvailableUsers = await users.Users.OrderBy(u => u.Email).ToListAsync();
  SelectedUser = string.IsNullOrWhiteSpace(SelectedUserId) ? null : await users.FindByIdAsync(SelectedUserId);
  if (SelectedUser != null) IsAdmin = await users.IsInRoleAsync(SelectedUser, ApplicationRoles.Admin);
 }
 public async Task OnGetAsync() { await LoadAsync(); }
 public async Task<IActionResult> OnPostAsync() {
  await LoadAsync(); if (SelectedUser is null) ModelState.AddModelError(nameof(SelectedUserId), "Select a valid user.");
  else StatusMessage = "User selected.";
  return Page();
 }
 public async Task<IActionResult> OnPostUpdateRoleAsync() => await ChangeAsync(Action, Role);
 public async Task<IActionResult> OnPostAddAsync() => await ChangeAsync("add", ApplicationRoles.Admin);
 public async Task<IActionResult> OnPostRemoveAsync() => await ChangeAsync("remove", ApplicationRoles.Admin);
 private async Task<IActionResult> ChangeAsync(string action, string role) {
  IdentityResult result = action switch {
   "add" => await authorization.AddRoleAsync(User, SelectedUserId ?? "", role),
   "remove" => await authorization.RemoveRoleAsync(User, SelectedUserId ?? "", role),
   _ => IdentityResult.Failed(new IdentityError { Description = "Invalid action." })
  };
  StatusMessage = result.Succeeded ? "Administrator role updated." : string.Join(" ", result.Errors.Select(e => e.Description));
  return RedirectToPage(new { SelectedUserId });
 }
}
