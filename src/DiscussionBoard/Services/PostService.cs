using DiscussionBoard.Data;
using DiscussionBoard.Models;
using Microsoft.EntityFrameworkCore;
namespace DiscussionBoard.Services;
public class PostService(ApplicationDbContext db, ILogger<PostService> logger) {
 public PostService(ApplicationDbContext db) : this(db, Microsoft.Extensions.Logging.Abstractions.NullLogger<PostService>.Instance) { }
 public Task<List<Post>> GetForThreadAsync(int threadId) => db.Posts.Include(x => x.ApplicationUser).Where(x => x.DiscussionThreadId == threadId && !x.IsDeleted).ToListAsync();
 public async Task AddAsync(Post post) { try { db.Posts.Add(post); await db.SaveChangesAsync(); } catch (DbUpdateException ex) { logger.LogError(ex, "Could not add reply to discussion {Id}", post.DiscussionThreadId); throw; } }
}

