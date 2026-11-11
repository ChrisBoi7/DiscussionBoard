using DiscussionBoard.Authorization;
using DiscussionBoard.Models;
using DiscussionBoard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace DiscussionBoard.Pages;
[Authorize] public class EditPostModel(PostService posts, IApplicationAuthorizationService authorization) : PageModel
{
 [BindProperty] public DiscussionEditViewModel Input { get; set; } = new();
 public async Task<IActionResult> OnGetAsync(int threadId, int postId) {
  var post = await posts.GetAsync(postId); if (post is null || post.DiscussionThreadId != threadId) return NotFound();
  if (!await authorization.CanEditAsync(User, post)) return Forbid();
  Input = new() { Title = post.Title, Content = post.Content }; return Page();
 }
 public async Task<IActionResult> OnPostAsync(int threadId, int postId) {
  var post = await posts.GetAsync(postId); if (post is null || post.DiscussionThreadId != threadId) return NotFound();
  if (!await authorization.CanEditAsync(User, post)) return Forbid();
  if (!ModelState.IsValid) return Page();
  await posts.UpdateAsync(new Post { Id = postId, Title = Input.Title, Content = Input.Content });
  return RedirectToPage("/Discussions/Details", new { threadId });
 }
}
