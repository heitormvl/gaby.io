using System.ComponentModel.DataAnnotations;

namespace Gaby.io.ViewModels;

public class ForgotPasswordViewModel
{
    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "Insira um e-mail válido.")]
    public string Email { get; set; } = string.Empty;
}
