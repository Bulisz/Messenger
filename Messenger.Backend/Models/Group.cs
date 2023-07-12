using System.ComponentModel.DataAnnotations.Schema;

namespace Messenger.Backend.Models;

[Table("Groups")]
public class Group
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<ApplicationUser> Users { get; set; } = null!;
    public ICollection<Message> Messages { get; set; } = null!;
}
