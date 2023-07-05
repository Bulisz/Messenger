using System.ComponentModel.DataAnnotations;

namespace Messenger.Backend.Models.AuthDTOs;

public record LoginRequest
{
    [Required]
    public string UserName { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
