namespace Messenger.Backend.Models.AuthDTOs;

public class UserAndRolesDTO
{
    public ApplicationUser? User { get; set; }
    public IList<string> Roles { get; set; } = new List<string>();
}
