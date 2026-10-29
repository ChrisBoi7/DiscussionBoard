using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using DiscussionBoard.Models;
using DiscussionBoard.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace DiscussionBoard.Pages.Discussions;
public class DetailsModel(DiscussionThreadService threads, PostService posts) : PageModel {
 public class ReplyInput { [Required, MaxLength(10000)] public string Content { get; set; } = ""; public int? ParentPostId { get; set; } }
 [BindProperty] public ReplyInput Input { get; set; } = new();
 public DiscussionThread Thread { get; private set; } = null!; public List<PostTreeBuilder.Node> PostTree { get; private set; } = [];
 private async Task<bool> LoadAsync(int id) { var thread = await threads.GetAsync(id); if (thread is null) return false; Thread = thread; PostTree = PostTreeBuilder.Build(await posts.GetForThreadAsync(id)); return true; }
 public async Task<IActionResult> OnGetAsync(int threadId) => await LoadAsync(threadId) ? Page() : NotFound();
 public async Task<IActionResult> OnPostReplyAsync(int threadId) {
  if (User.Identity?.IsAuthenticated != true) return Challenge();
  if (!await LoadAsync(threadId)) return NotFound();
  if (Input.ParentPostId.HasValue && !(await posts.GetForThreadAsync(threadId)).Any(p => p.Id == Input.ParentPostId)) ModelState.AddModelError("Input.ParentPostId", "Parent reply was not found.");
  if (!ModelState.IsValid) return Page();
  await posts.AddAsync(new Post { DiscussionThreadId = threadId, ParentPostId = Input.ParentPostId, Title = "Reply", Content = Input.Content, ApplicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)! });
  return RedirectToPage(new { threadId });
 }
}
