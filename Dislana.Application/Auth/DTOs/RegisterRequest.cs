using System.ComponentModel.DataAnnotations;

namespace Dislana.Application.Auth.DTOs
{
    public record RegisterRequest(
        [property: Required(ErrorMessage = "El nombre es obligatorio.")]
        [property: MaxLength(100)]
        string Name,

        [property: Required(ErrorMessage = "El apellido es obligatorio.")]
        [property: MaxLength(100)]
        string LastName,

        [property: Required(ErrorMessage = "El correo es obligatorio.")]
        [property: EmailAddress(ErrorMessage = "Formato de correo inválido.")]
        [property: MaxLength(150)]
        string Email,

        [property: Required(ErrorMessage = "La contraseña es obligatoria.")]
        [property: MinLength(6, ErrorMessage = "La contraseña debe tener mínimo 6 caracteres.")]
        [property: MaxLength(100)]
        string Password
    );
}
