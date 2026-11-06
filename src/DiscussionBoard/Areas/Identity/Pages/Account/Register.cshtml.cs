using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using DiscussionBoard.Models;
using DiscussionBoard.Services;
using DiscussionBoard.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
namespace DiscussionBoard.Areas.Identity.Pages.Account;
public class RegisterModel(UserManager<ApplicationUser> users, SignInManager<ApplicationUser> signIn,
    IUserProfileService profiles, IUserStore<ApplicationUser> store, IEmailSender emailSender,
    ILogger<RegisterModel> logger) : PageModel
{
    [BindProperty] public InputModel Input { get; set; } = new();
    public string? ReturnUrl { get; set; }
    public class InputModel
    {
        [Required, EmailAddress] public string Email { get; set; } = "";
        [Required, StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 6), DataType(DataType.Password)]
        public string Password { get; set; } = "";
        [DataType(DataType.Password), Display(Name = "Confirm password"), Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = "";
        public UserProfile Profile { get; set; } = new();
    }
    public void OnGet(string? returnUrl = null) { ReturnUrl = returnUrl; Input.Profile.AvatarId = AvatarCatalog.DefaultId; }
    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");
        ReturnUrl = returnUrl;
        Input.Profile.DisplayName = (Input.Profile.DisplayName ?? "").Trim();
        ModelState.Remove("Input.Profile.DisplayName");
        if (Input.Profile.DisplayName.Length is < 2 or > 60)
            ModelState.AddModelError("Input.Profile.DisplayName", "Display name must be between 2 and 60 characters.");
        else if (!await profiles.IsDisplayNameAvailableAsync(Input.Profile.DisplayName))
            ModelState.AddModelError("Input.Profile.DisplayName", "This display name is already in use.");
        if (!AvatarCatalog.IsValid(Input.Profile.AvatarId))
            ModelState.AddModelError("Input.Profile.AvatarId", "Please select a valid avatar.");
        if (!ModelState.IsValid) return Page();
        var user = new ApplicationUser { DisplayName = Input.Profile.DisplayName, AvatarId = Input.Profile.AvatarId };
        await store.SetUserNameAsync(user, Input.Email, CancellationToken.None);
        var emailStore = store as IUserEmailStore<ApplicationUser> ?? throw new NotSupportedException("Email store is required.");
        await emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
        var result = await users.CreateAsync(user, Input.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
            return Page();
        }
        logger.LogInformation("User created a new account with password.");
        var code = await users.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var callbackUrl = Url.Page("/Account/ConfirmEmail", pageHandler: null,
            values: new { area = "Identity", userId = await users.GetUserIdAsync(user), code, returnUrl }, protocol: Request.Scheme);
        await emailSender.SendEmailAsync(Input.Email, "Confirm your email",
            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl!)}'>clicking here</a>.");
        if (users.Options.SignIn.RequireConfirmedAccount)
            return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl });
        await signIn.SignInAsync(user, isPersistent: false);
        return LocalRedirect(returnUrl);
    }
}
