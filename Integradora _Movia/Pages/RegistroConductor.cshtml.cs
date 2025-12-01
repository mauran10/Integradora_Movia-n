// Pages/RegistroConductor.cshtml.cs
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization; 
// Asegúrate de que este using apunta a tu archivo de modelo correcto
using Integradora_Movia.Models;

namespace Integradora_Movia.Pages
{
    public class RegistroConductorModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        [BindProperty]
        public RegisterConductorApiModel Input { get; set; } = new RegisterConductorApiModel();

        [TempData]
        public string Message { get; set; } = string.Empty;

        // Constructor: Asume que tienes configurado el IHttpClientFactory en Program.cs
        public RegistroConductorModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public void OnGet()
        {
            Message = string.Empty;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // === CÓDIGO FINAL CORREGIDO: Usar PascalCase (MAYÚSCULA INICIAL) ===
            // Los nombres deben coincidir EXACTAMENTE con el modelo 'Conductor' del API.
            var apiPayload = new
            {
                idConductor = 0,  // Puedes enviar 0 o null
                nombres = Input.Nombres,
                apellidos = Input.Apellidos,
                curp = Input.Curp,
                numeroTelefono = Input.NumeroTelefono,
                tipoLicencia = Input.Tipolicencia,
                numeroLicencia = Input.NumeroLicencia,
                vencimiento = DateTime.Parse(Input.Vencimiento).ToString("yyyy-MM-ddTHH:mm:ss.fff"),
                correoElectronico = Input.CorreoElectronico,
                contrasena = Input.Contrasena,
                fechaRegistro = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff")
            };

            var client = _httpClientFactory.CreateClient("MovIA_API");
            var apiEndpoint = "api/Conductores";


            var jsonContent = JsonSerializer.Serialize(apiPayload);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await client.PostAsync(apiEndpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    Message = " ¡Registro exitoso! Ya puedes iniciar sesión.";
                    // Redirige al login de conductor
                    return RedirectToPage("/LoginConductor");
                }
                else
                {
                    // Manejo de errores 400 (Bad Request) o 500 (Internal Server Error)
                    string responseContent = await response.Content.ReadAsStringAsync();

                    if (response.StatusCode == System.Net.HttpStatusCode.Conflict) // Código 409
                    {
                        Message = " Error: El correo o número de licencia ya se encuentra registrado.";
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest) // Código 400
                    {
                        Message = " Error al registrar: Verifique todos los campos. Hay datos inválidos o incompletos.";
                    }
                    else // Código 500 (Internal Server Error) o cualquier otro no manejado
                    {
                        Message = $" Error al registrar: Internal Server Error. Verifique todos los campos e intente más tarde. (HTTP {response.StatusCode})";
                    }

                    return Page();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EXCEPCIÓN CONEXIÓN] Mensaje: {ex.Message}");
                Message = " Error de conexión con el API. Verifica que esté en línea.";
                return Page();
            }
        }
    }
}