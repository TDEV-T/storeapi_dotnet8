using System.ComponentModel.DataAnnotations;

namespace DotnetStoreAPI.Models;

public class LoginModel
{
    [Required(ErrorMessage = "Username is required")]
    [StringLength(50, ErrorMessage = "Username is too long")]
    [MinLength(3 , ErrorMessage = "Username is too short")]
    public required string Username {get;set;}

    [Required(ErrorMessage = "Password is required")]
    public required string Password {get;set;}
}