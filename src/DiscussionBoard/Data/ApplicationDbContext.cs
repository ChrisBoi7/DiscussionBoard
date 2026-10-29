using DiscussionBoard.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace DiscussionBoard.Data;
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options) {
 public DbSet<Forum> Forums => Set<Forum>();
 public DbSet<DiscussionThread> DiscussionThreads => Set<DiscussionThread>();
 public DbSet<Post> Posts => Set<Post>();
 protected override void OnModelCreating(ModelBuilder builder) {
  base.OnModelCreating(builder);
  builder.Entity<DiscussionThread>().HasOne(x => x.Forum).WithMany(x => x.DiscussionThreads).HasForeignKey(x => x.ForumId).OnDelete(DeleteBehavior.Restrict);
  builder.Entity<DiscussionThread>().HasOne(x => x.ApplicationUser).WithMany(x => x.DiscussionThreads).HasForeignKey(x => x.ApplicationUserId).OnDelete(DeleteBehavior.Restrict);
  builder.Entity<Post>().HasOne(x => x.DiscussionThread).WithMany(x => x.Posts).HasForeignKey(x => x.DiscussionThreadId).OnDelete(DeleteBehavior.Restrict);
  builder.Entity<Post>().HasOne(x => x.ParentPost).WithMany(x => x.Replies).HasForeignKey(x => x.ParentPostId).OnDelete(DeleteBehavior.Restrict);
  builder.Entity<Post>().HasOne(x => x.ApplicationUser).WithMany(x => x.Posts).HasForeignKey(x => x.ApplicationUserId).OnDelete(DeleteBehavior.Restrict);
 }
}
