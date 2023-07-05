using System.ComponentModel.DataAnnotations;

namespace Messenger.Backend.Models.AuthDTOs;

public record CreateUserDTO
{
    [Required]
    [RegularExpression("^[a-zA-Z0-9]{2,30}$", ErrorMessage = "2 és 30 ékezet nélküli betűből és számból állhat")]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [RegularExpression("^(?=.*\\d)(?!.*\\s).{6,250}$", ErrorMessage = "Legalább 6, legfeljebb 250 karakter hosszúnak kell lennie és tartalmazzon egy számot")]
    public string Password { get; set; } = string.Empty;

    [Required]
    [RegularExpression("^(?=.*\\d)(?!.*\\s).{6,250}$", ErrorMessage = "Legalább 6, legfeljebb 250 karakter hosszúnak kell lennie és tartalmazzon egy számot")]
    [Compare("Password", ErrorMessage = "Azonos jelszót kell megadni.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required]
    [RegularExpression("^[a-z0-9.]{2,}[@][a-z0-9]{2,}[.][a-z]{2,}$", ErrorMessage = "Valós email-címet adjon meg")]
    public string Email { get; set; } = string.Empty;
}
