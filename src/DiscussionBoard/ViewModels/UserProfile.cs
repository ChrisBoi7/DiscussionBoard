using System.ComponentModel.DataAnnotations;
using DiscussionBoard.Services;
namespace DiscussionBoard.ViewModels;
public class UserProfile
{
    [Display(Name = "Display name")]
    [Required, StringLength(60, MinimumLength = 2)]
    public string DisplayName { get; set; } = "";
    [Display(Name = "Avatar")]
    [Required]
    public string AvatarId { get; set; } = "";
}
