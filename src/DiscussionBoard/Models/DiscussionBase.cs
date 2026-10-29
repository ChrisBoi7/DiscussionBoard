using System.ComponentModel.DataAnnotations;
namespace DiscussionBoard.Models;
public abstract class DiscussionBase : EntityBase {
 [Required, MaxLength(200)] public string Title { get; set; } = "";
 [Required, MaxLength(10000)] public string Content { get; set; } = "";
 public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
 [Required] public string ApplicationUserId { get; set; } = "";
 public ApplicationUser ApplicationUser { get; set; } = null!;
}
