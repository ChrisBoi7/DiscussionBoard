using DiscussionBoard.Data;
using DiscussionBoard.Models;
using Microsoft.EntityFrameworkCore;
namespace DiscussionBoard.Services;
public class DiscussionThreadService(ApplicationDbContext db, ILogger<DiscussionThreadService> logger) {
 public DiscussionThreadService(ApplicationDbContext db) : this(db, Microsoft.Extensions.Logging.Abstractions.NullLogger<DiscussionThreadService>.Instance) { }
 public Task<DiscussionThread?> GetAsync(int id) => db.DiscussionThreads.Include(x => x.ApplicationUser).Include(x => x.Forum).FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted && !x.Forum.IsDeleted);
 public Task<List<DiscussionThread>> GetForForumAsync(int forumId) => db.DiscussionThreads.Include(x => x.ApplicationUser).Where(x => x.ForumId == forumId && !x.IsDeleted).ToListAsync();
 public async Task AddAsync(DiscussionThread thread) { try { db.DiscussionThreads.Add(thread); await db.SaveChangesAsync(); } catch (DbUpdateException ex) { logger.LogError(ex, "Could not create discussion"); throw; } }
 public async Task UpdateAsync(DiscussionThread thread) { try { var existing = await db.DiscussionThreads.FirstOrDefaultAsync(x => x.Id == thread.Id && !x.IsDeleted) ?? throw new InvalidOperationException("Discussion not found."); existing.Title = thread.Title; existing.Content = thread.Content; await db.SaveChangesAsync(); } catch (DbUpdateException ex) { logger.LogError(ex, "Could not update discussion {Id}", thread.Id); throw; } }
}

