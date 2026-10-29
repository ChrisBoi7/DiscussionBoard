using System.Security.Claims;
using DiscussionBoard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace DiscussionBoard.Pages.Discussions;
[Authorize] public class EditModel(DiscussionThreadService threads) : PageModel {
 [BindProperty] public ThreadInput Input { get; set; } = new();
 public async Task<IActionResult> OnGetAsync(int threadId) { var thread = await threads.GetAsync(threadId); if (thread is null) return NotFound(); if (thread.ApplicationUserId != User.FindFirstValue(ClaimTypes.NameIdentifier)) return Forbid(); Input = new() { Title = thread.Title, Content = thread.Content }; return Page(); }
 public async Task<IActionResult> OnPostAsync(int threadId) { var thread = await threads.GetAsync(threadId); if (thread is null) return NotFound(); if (thread.ApplicationUserId != User.FindFirstValue(ClaimTypes.NameIdentifier)) return Forbid(); if (!ModelState.IsValid) return Page(); thread.Title = Input.Title; thread.Content = Input.Content; await threads.UpdateAsync(thread); return RedirectToPage("/Discussions/Details", new { threadId }); }
}
