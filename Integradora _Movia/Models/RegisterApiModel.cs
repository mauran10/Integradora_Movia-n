using System.ComponentModel.DataAnnotations;

namespace Integradora_Movia.Models.ApiModels
{
    public class RegisterApiModel
    {
        // Propiedades separadas para nombres y apellidos (requeridas por la API)
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string nombres { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        public string apellidos { get; set; } = string.Empty;

        // Nuevos campos requeridos por tu API (según la estructura que mostraste)
        [Required(ErrorMessage = "La edad es obligatoria.")]
        [Range(18, 100, ErrorMessage = "Debes ser mayor de 18 años.")]
        public int edad { get; set; } = 0; // Se inicializa a 0

        [Required(ErrorMessage = "El municipio es obligatorio.")]
        [Display(Name = "Municipio de Residencia")]
        public string municipioResidencia { get; set; } = string.Empty; // Mapeado desde Input.MunicipioResidencia

        // Nombre de Usuario o alias
        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        [Display(Name = "Nombre de Usuario")]
        public string usuarioNombre { get; set; } = string.Empty;
        
        // Correo Electrónico
        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido.")]
        public string correoElectronico { get; set; } = string.Empty;

        // Contraseña (ContrasenaHash en la API)
        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La {0} debe tener al menos {2} y máximo {1} caracteres.")]
        public string contrasenaHash { get; set; } = string.Empty;
        
        // ConfirmarContrasena NO va en el ApiModel, solo es para validación en el cliente/servidor.
    }
}