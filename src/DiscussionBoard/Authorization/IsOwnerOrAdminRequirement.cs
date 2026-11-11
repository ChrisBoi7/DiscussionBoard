using Microsoft.AspNetCore.Authorization;
namespace DiscussionBoard.Authorization;
public sealed class IsOwnerOrAdminRequirement : IAuthorizationRequirement { }
