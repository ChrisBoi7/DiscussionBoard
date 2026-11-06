using Microsoft.AspNetCore.Identity;
using DiscussionBoard.Services;
namespace DiscussionBoard.Models;
public class ApplicationUser : IdentityUser {

    public string DisplayName { get; set; } = string.Empty;

    public string AvatarId { get; set; } = AvatarCatalog.DefaultId;
 public ICollection<DiscussionThread> DiscussionThreads { get; set; } = new List<DiscussionThread>();
 public ICollection<Post> Posts { get; set; } = new List<Post>();
}
