using System.ComponentModel.DataAnnotations;

namespace DotnetStoreAPI.Models;

public class RegisteModel
{
    [Required(ErrorMessage = "Username is required")]
    [StringLength(50, ErrorMessage = "Username is too long")]
    [MinLength(3 , ErrorMessage = "Username is too short")]
    public required string Username {get;set;}

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email is not valid")]
    public required string Email {get;set;}

    [Required(ErrorMessage = "Password is required")]
    [MinLength(6,ErrorMessage = "Password is too short")]
    public required string Password {get;set;}
}