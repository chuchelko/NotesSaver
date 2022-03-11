using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MVCNotesSaver.Models.User;

public class RegisterViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    [PasswordPropertyText]
    [MinLength(4, ErrorMessage = "At least 4 characters")]
    public string Password { get; set; }
    [Required]
    [PasswordPropertyText]
    [Compare("Password", ErrorMessage="Must be equal to Password")]
    public string ConfirmPassword { get; set; }

}