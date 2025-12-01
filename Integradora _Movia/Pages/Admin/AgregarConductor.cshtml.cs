using Integradora_Movia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;

namespace Integradora_Movia.Pages.Admin
{
    public class AgregarConductorModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        // Utilizamos el modelo de registro para esta acción
        [BindProperty]
        public RegisterConductorApiModel Conductor { get; set; } = new();

        public AgregarConductorModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public void OnGet()
        {
            // Solo muestra la página con el formulario vacío
        }

        // ==========================
        // POST (Envía el nuevo conductor a la API)
        // ==========================
        public async Task<IActionResult> OnPost()
        {
            // 1. Validar el modelo (Revisa que todos los campos requeridos estén llenos, incluyendo Contraseña)
            if (!ModelState.IsValid)
            {
                // Si la validación falla (ej. contraseñas no coinciden), recarga la página
                return Page();
            }

            // 2. Preparar el objeto para el envío (La API de POST no requiere Id, ni FechaRegistro, etc.)
            // Serializamos directamente el objeto RegisterConductorApiModel
            var json = JsonSerializer.Serialize(Conductor);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var client = _httpClientFactory.CreateClient("MovIA_API");

            // 3. Enviar la solicitud POST a /api/Conductores
            var response = await client.PostAsync("api/Conductores", content);

            // 4. Manejo de Respuesta
            if (response.IsSuccessStatusCode)
            {
                // Redirige a la lista de conductores si fue exitoso
                return RedirectToPage("Conductores");
            }

            // Si hay un error de la API (ej. correo ya existe), lo leemos y lo mostramos.
            var respuestaApi = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", $"ERROR API: No se pudo registrar el conductor. Detalle: {respuestaApi}");

            return Page(); // Vuelve a la página con el mensaje de error
        }
    }
}