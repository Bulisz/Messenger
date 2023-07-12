using System.ComponentModel.DataAnnotations.Schema;

namespace Messenger.Backend.Models;

[Table("Messages")]
public class Message
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public ApplicationUser User { get; set; } = null!;
    public Group Group { get; set; } = null!;
}
