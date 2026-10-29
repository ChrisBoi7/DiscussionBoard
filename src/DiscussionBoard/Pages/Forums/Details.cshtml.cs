using DiscussionBoard.Models;
using DiscussionBoard.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace DiscussionBoard.Pages.Forums;
public class DetailsModel(ForumService forums, DiscussionThreadService threads) : PageModel {
 public Forum Forum { get; private set; } = null!; public List<DiscussionThread> Threads { get; private set; } = [];
 public async Task<IActionResult> OnGetAsync(int id) { var forum = await forums.GetAsync(id); if (forum is null) return NotFound(); Forum = forum; Threads = await threads.GetForForumAsync(id); return Page(); }
}
