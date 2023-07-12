using Microsoft.AspNetCore.Identity;

namespace Messenger.Backend.Models;

public class ApplicationUser : IdentityUser
{
    public ICollection<Group> Groups { get; set; } = null!;
    public ICollection<Message> Messages { get; set; } = null!;
}
