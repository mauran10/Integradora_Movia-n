using System.ComponentModel.DataAnnotations;

namespace Integradora_Movia.Models
{
    // Modelo utilizado para enlazar los datos de entrada del formulario de login.
    public class LoginApiModel
    {
        // Propiedad para el Correo Electrónico.
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de correo electrónico no válido.")]
        public string CorreoElectronico { get; set; }

        // Propiedad para la Contraseña. Se llama "Contrasena" para el front-end 
        // y la validación, y se mapeará a "ContrasenaHash" para el API.
        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        public string Contrasena { get; set; }
    }
}