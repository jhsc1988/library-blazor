using System.ComponentModel.DataAnnotations;

namespace lib_blazor.Models;

public class Registration 
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MinLength(6)]
    public string Password { get; set; }
}