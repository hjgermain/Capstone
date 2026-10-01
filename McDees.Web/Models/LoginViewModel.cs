using System.ComponentModel.DataAnnotations;

namespace McDees.Web.Models;

public sealed class LoginViewModel
{
    [Required, EmailAddress, Display(Name = "Email address")]
    public string Email { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }
}
