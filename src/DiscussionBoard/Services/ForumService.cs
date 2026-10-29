using DiscussionBoard.Data;
using DiscussionBoard.Models;
using Microsoft.EntityFrameworkCore;
namespace DiscussionBoard.Services;
public class ForumService(ApplicationDbContext db) {
 public Task<List<Forum>> GetAllAsync() => db.Forums.Where(x => !x.IsDeleted).ToListAsync();
 public Task<Forum?> GetAsync(int id) => db.Forums.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
}
