using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;
using Integradora_Movia.Models.ApiModels;
// Quitamos el 'using' problemático para que compile
// using Microsoft.AspNetCore.Mvc.ModelBinding; 

namespace Integradora_Movia.Pages
{
    public class RegistroModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<RegistroModel> _logger;

        // Propiedad que enlaza con los campos del formulario
        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        // Propiedad para mostrar mensajes de error del API
        [BindProperty]
        public string ErrorMessage { get; set; } = string.Empty;

        // Constructor para inyección de dependencias
        public RegistroModel(IHttpClientFactory httpClientFactory, ILogger<RegistroModel> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        // Clase anidada (ViewModel) que usamos para la interfaz de usuario
        public class InputModel
        {
            [Required(ErrorMessage = "Los nombres son obligatorios.")]
            [Display(Name = "Nombres")]
            public string Nombres { get; set; } = string.Empty;

            [Required(ErrorMessage = "Los apellidos son obligatorios.")]
            [Display(Name = "Apellidos")]
            public string Apellidos { get; set; } = string.Empty;

            [Required(ErrorMessage = "La edad es obligatoria.")]
            [Range(18, 100, ErrorMessage = "Debes ser mayor de 18 años y menor de 100.")]
            public int Edad { get; set; }

            [Required(ErrorMessage = "El municipio es obligatorio.")]
            [Display(Name = "Municipio de Residencia")]
            public string Municipio { get; set; } = string.Empty;

            [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
            [Display(Name = "Nombre de Usuario")]
            public string UsuarioNombre { get; set; } = string.Empty;

            [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
            [EmailAddress(ErrorMessage = "Formato de correo inválido.")]
            public string CorreoElectronico { get; set; } = string.Empty;

            [Required(ErrorMessage = "La contraseña es obligatoria.")]
            [DataType(DataType.Password)]
            [StringLength(100, MinimumLength = 6, ErrorMessage = "La {0} debe tener al menos {2} y máximo {1} caracteres.")]
            public string Contrasena { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Display(Name = "Confirmar Contraseña")]
            [Compare("Contrasena", ErrorMessage = "La contraseña y la confirmación no coinciden.")]
            public string ConfirmarContrasena { get; set; } = string.Empty;
        }

        public void OnGet()
        {
            ErrorMessage = string.Empty;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ErrorMessage = string.Empty;

            // FIX DE VALIDACIÓN: Ignoramos el error de validación causado por ErrorMessage.
            ModelState.Remove(nameof(ErrorMessage));

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // 2. Mapeo de datos para la API
            // *** ESTA SECCIÓN HA SIDO CORREGIDA PARA COINCIDIR CON LOS NOMBRES DEL MODELO DE TU API ***
            var registerData = new RegisterApiModel
            {
                // API espera 'nombres' (minúscula)
                nombres = Input.Nombres,
                // API espera 'apellidos' (minúscula)
                apellidos = Input.Apellidos,
                // API espera 'edad' (minúscula)
                edad = Input.Edad,
                // API espera 'municipioResidencia' (TODO JUNTO)
                municipioResidencia = Input.Municipio,
                // API espera 'usuarioNombre' (minúscula inicial)
                usuarioNombre = Input.UsuarioNombre,
                // API espera 'correoElectronico' (minúscula inicial)
                correoElectronico = Input.CorreoElectronico,
                // API espera 'contrasenaHash' (Nombre que usa tu modelo)
                contrasenaHash = Input.Contrasena
            };

            var jsonContent = JsonSerializer.Serialize(registerData);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var client = _httpClientFactory.CreateClient("MovIA_API");
            try
            {
                // DEBUG LINE: Esta línea es para que puedas ver el JSON en el debugger.
                var jsonToSend = await content.ReadAsStringAsync();

                // 3. Envío al API (COLOCA EL BREAKPOINT AQUÍ)
                var response = await client.PostAsync("api/Usuarios", content);

                if (response.IsSuccessStatusCode)
                {
                    // Éxito: Redirección
                    return RedirectToPage("/Login");
                }
                else
                {
                    // Error del API: Muestra el mensaje
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Error al registrar: {StatusCode}, Respuesta: {Error}", response.StatusCode, errorResponse);

                    ErrorMessage = $"Error al registrar usuario. Estado: {response.StatusCode} ({response.StatusCode.ToString()}). ";

                    if (!string.IsNullOrEmpty(errorResponse))
                    {
                        ErrorMessage += $"Detalles del servidor: {errorResponse}";
                    }

                    return Page();
                }
            }
            catch (HttpRequestException ex)
            {
                // Error de conexión
                _logger.LogError(ex, "Error de conexión al API durante el registro.");
                ErrorMessage = "Error de conexión. No se pudo contactar al servidor de la API. Revisa la configuración del URL base.";
                return Page();
            }
        }
    }
}