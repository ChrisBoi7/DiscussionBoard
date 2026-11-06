using DiscussionBoard.Models;
using DiscussionBoard.Services;
using DiscussionBoard.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace DiscussionBoard.Pages;
[Authorize]
public class MyProfileModel(UserManager<ApplicationUser> users, IUserProfileService profiles) : PageModel
{
    [BindProperty] public UserProfile Input { get; set; } = new();
    [TempData] public string? StatusMessage { get; set; }
    public async Task<IActionResult> OnGetAsync()
    {
        var user = await users.GetUserAsync(User);
        if (user is null) return Challenge();
        Input = new UserProfile { DisplayName = user.DisplayName, AvatarId = user.AvatarId };
        return Page();
    }
    public async Task<IActionResult> OnPostAsync()
    {
        var user = await users.GetUserAsync(User);
        if (user is null) return Challenge();
        Input.DisplayName = (Input.DisplayName ?? "").Trim();
        ModelState.Remove("Input.DisplayName");
        if (Input.DisplayName.Length is < 2 or > 60)
            ModelState.AddModelError("Input.DisplayName", "Display name must be between 2 and 60 characters.");
        else if (!await profiles.IsDisplayNameAvailableAsync(Input.DisplayName, user.Id))
            ModelState.AddModelError("Input.DisplayName", "This display name is already in use.");
        if (!AvatarCatalog.IsValid(Input.AvatarId))
            ModelState.AddModelError("Input.AvatarId", "Please select a valid avatar.");
        if (!ModelState.IsValid) return Page();
        user.DisplayName = Input.DisplayName;
        user.AvatarId = Input.AvatarId;
        var result = await users.UpdateAsync(user);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
            return Page();
        }
        StatusMessage = "Your profile has been updated.";
        return RedirectToPage();
    }
}
