using DiscussionBoard.Data;
using DiscussionBoard.Models;
using Microsoft.EntityFrameworkCore;
namespace DiscussionBoard.Services;
public class PostService(ApplicationDbContext db, ILogger<PostService> logger) {
 public PostService(ApplicationDbContext db) : this(db, Microsoft.Extensions.Logging.Abstractions.NullLogger<PostService>.Instance) { }
 public Task<List<Post>> GetForThreadAsync(int threadId) => db.Posts.Include(x => x.ApplicationUser).Where(x => x.DiscussionThreadId == threadId && !x.IsDeleted).ToListAsync();
 public Task<Post?> GetAsync(int id) => db.Posts.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
 public async Task UpdateAsync(Post post) {
  try { var existing = await db.Posts.FirstOrDefaultAsync(x => x.Id == post.Id && !x.IsDeleted) ?? throw new InvalidOperationException("Post not found."); existing.Title = post.Title; existing.Content = post.Content; await db.SaveChangesAsync(); }
  catch (DbUpdateException ex) { logger.LogError(ex, "Could not update post {Id}", post.Id); throw; }
 }
 public async Task AddAsync(Post post) { try { db.Posts.Add(post); await db.SaveChangesAsync(); } catch (DbUpdateException ex) { logger.LogError(ex, "Could not add reply to discussion {Id}", post.DiscussionThreadId); throw; } }
}

