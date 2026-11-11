using DiscussionBoard.Authorization;
using DiscussionBoard.Models;
using DiscussionBoard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace DiscussionBoard.Pages;
[Authorize] public class ThreadEditModel(DiscussionThreadService threads, IApplicationAuthorizationService authorization) : PageModel
{
 [BindProperty] public DiscussionEditViewModel Input { get; set; } = new();
 public async Task<IActionResult> OnGetAsync(int id) {
  var thread = await threads.GetAsync(id); if (thread is null) return NotFound();
  if (!await authorization.CanEditAsync(User, thread)) return Forbid();
  Input = new() { Title = thread.Title, Content = thread.Content }; return Page();
 }
 public async Task<IActionResult> OnPostAsync(int id) {
  var thread = await threads.GetAsync(id); if (thread is null) return NotFound();
  if (!await authorization.CanEditAsync(User, thread)) return Forbid();
  if (!ModelState.IsValid) return Page();
  await threads.UpdateAsync(new DiscussionThread { Id = id, Title = Input.Title, Content = Input.Content });
  return RedirectToPage("/Discussions/Details", new { threadId = id });
 }
}
