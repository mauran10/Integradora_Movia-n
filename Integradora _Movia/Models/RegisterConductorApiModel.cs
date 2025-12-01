// Models/RegisterConductorApiModel.cs
using System.ComponentModel.DataAnnotations;
using System;

namespace Integradora_Movia.Models
{
    public class RegisterConductorApiModel
    {
        // === PROPIEDADES EN PASCALCASE PARA COINCIDIR CON EL MODELO DEL API ===


        [Required(ErrorMessage = "Los nombres son obligatorios.")]
        [Display(Name = "Nombres")]
        public string Nombres { get; set; }

        [Required(ErrorMessage = "Los apellidos son obligatorios.")]
        [Display(Name = "Apellidos")]
        public string Apellidos { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de correo electrónico inválido.")]
        [Display(Name = "Correo Electrónico")]
        public string CorreoElectronico { get; set; }

        [Required(ErrorMessage = "El número de licencia es obligatorio.")]
        [Display(Name = "Número de Licencia")]
        public string NumeroLicencia { get; set; } // PascalCase

        [Required(ErrorMessage = "La CURP es obligatoria.")]
        [StringLength(18, MinimumLength = 18, ErrorMessage = "La CURP debe tener 18 caracteres.")]
        [Display(Name = "CURP")]
        public string Curp { get; set; }

        [Required(ErrorMessage = "El número de teléfono es obligatorio.")]
        [Display(Name = "Número de Teléfono")]
        public string NumeroTelefono { get; set; } // PascalCase

        [Required(ErrorMessage = "El tipo de licencia es obligatorio.")]
        [Display(Name = "Tipo de Licencia")]
        public string Tipolicencia { get; set; } // PascalCase

        [Required(ErrorMessage = "La fecha de vencimiento es obligatoria.")]
        [DataType(DataType.Date)]
        [Display(Name = "Vencimiento")]
        public string Vencimiento { get; set; } // PascalCase

        // =====================================================================

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La {0} debe tener al menos {2} y un máximo de {1} caracteres.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Contrasena { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Contraseña")]
        [Compare("Contrasena", ErrorMessage = "La contraseña y la confirmación no coinciden.")]
        public string ConfirmarContrasena { get; set; }
    }
}