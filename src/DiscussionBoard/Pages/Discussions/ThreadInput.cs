using System.ComponentModel.DataAnnotations;
namespace DiscussionBoard.Pages.Discussions;
public class ThreadInput { [Required, MaxLength(200)] public string Title { get; set; } = ""; [Required, MaxLength(10000)] public string Content { get; set; } = ""; }
