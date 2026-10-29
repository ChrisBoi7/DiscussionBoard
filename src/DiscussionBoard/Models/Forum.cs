using System.ComponentModel.DataAnnotations;
namespace DiscussionBoard.Models;
public class Forum : EntityBase {
 [Required, MaxLength(200)] public string Name { get; set; } = "";
 [MaxLength(2000)] public string Description { get; set; } = "";
 public ICollection<DiscussionThread> DiscussionThreads { get; set; } = new List<DiscussionThread>();
}
