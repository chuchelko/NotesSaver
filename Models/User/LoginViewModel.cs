using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MVCNotesSaver.Models.User;

public class LoginViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required] 
    [PasswordPropertyText] 
    [MinLength(length: 4, ErrorMessage = "At least 4 characters")]
    public string Password { get; set; }

    public bool UserDoesntExist = false;
}