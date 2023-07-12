namespace Messenger.Backend.Models.AuthDTOs;

public record UserAndRolesDTO
{
    public ApplicationUser? User { get; set; }
    public IList<string> Roles { get; set; } = new List<string>();
}
