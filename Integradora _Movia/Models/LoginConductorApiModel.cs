// Models/LoginConductorApiModel.cs
using System.ComponentModel.DataAnnotations;

namespace Integradora_Movia.Models
{
    public class LoginConductorApiModel
    {
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de correo electrónico inválido.")]
        [Display(Name = "Correo Electrónico")]
        public string CorreoElectronico { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Contrasena { get; set; }

        [Required(ErrorMessage = "El número de licencia es obligatorio.")]
        [Display(Name = "Número de Licencia")]
        public string NumeroLicencia { get; set; }
    }
}