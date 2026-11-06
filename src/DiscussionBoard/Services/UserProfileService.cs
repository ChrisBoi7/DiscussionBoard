using DiscussionBoard.Data;
using Microsoft.EntityFrameworkCore;
namespace DiscussionBoard.Services;
public class UserProfileService(ApplicationDbContext db) : IUserProfileService
{
    public async Task<bool> IsDisplayNameAvailableAsync(string displayName, string? excludeUserId = null)
    {
        string name = (displayName ?? "").Trim();
        if (name.Length is < 2 or > 60) return false;
        // Compare in .NET so non-ASCII names are also matched case-insensitively.
        var names = await db.Users.Where(u => u.Id != excludeUserId && u.DisplayName != "")
            .Select(u => u.DisplayName).ToListAsync();
        return !names.Any(existing => string.Equals(existing.Trim(), name, StringComparison.OrdinalIgnoreCase));
    }
}
