using Integradora_Movia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;

namespace Integradora_Movia.Pages.Admin
{
    public class EditarConductorModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public EditarConductorModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public ConductorModel Conductor { get; set; } = new();

        // ==========================
        // GET (Recupera datos para llenar el formulario)
        // ==========================
        public async Task<IActionResult> OnGet(int id)
        {
            var client = _httpClientFactory.CreateClient("MovIA_API");

            var response = await client.GetAsync($"api/Conductores/{id}");

            if (!response.IsSuccessStatusCode)
                return RedirectToPage("Conductores");

            var json = await response.Content.ReadAsStringAsync();

            Conductor = JsonSerializer.Deserialize<ConductorModel>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

            // 🛑 CAMBIO CLAVE: CONVERSIÓN DE FORMATO DE FECHA PARA EL INPUT HTML
            if (DateTime.TryParse(Conductor.Vencimiento, out DateTime vencimientoDT))
            {
                // Convierte la fecha al formato yyyy-MM-dd para que el control <input type="date"> la muestre.
                Conductor.Vencimiento = vencimientoDT.ToString("yyyy-MM-dd");
            }

            return Page();
        }

        // ==========================
        // POST (Guarda los cambios con PUT)
        // ==========================
        public async Task<IActionResult> OnPost()
        {
            // 1. VALIDACIÓN DEL MODELO
            if (!ModelState.IsValid)
            {
                // Si la validación falla (ej. campos requeridos vacíos), volvemos a la página.
                return Page();
            }

            var client = _httpClientFactory.CreateClient("MovIA_API");

            // 2. RECUPERAR DATOS ORIGINALES (Contraseña y FechaRegistro)
            var getResponse = await client.GetAsync($"api/Conductores/{Conductor.IdConductor}");

            if (!getResponse.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "No se pudo recuperar el conductor original.");
                return Page();
            }

            var originalJson = await getResponse.Content.ReadAsStringAsync();
            var conductorOriginal = JsonSerializer.Deserialize<ConductorModel>(originalJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (conductorOriginal == null)
            {
                ModelState.AddModelError("", "Error al deserializar datos originales.");
                return Page();
            }

            // 3. ENRIQUECER EL MODELO: Asignar datos no editables al objeto que se va a enviar

            // Mantiene la contraseña original si el formulario no envió una nueva.
            if (string.IsNullOrEmpty(Conductor.Contrasena))
            {
                Conductor.Contrasena = conductorOriginal.Contrasena;
            }

            // Mantiene la fecha de registro original.
            Conductor.FechaRegistro = conductorOriginal.FechaRegistro;

            // 4. PREPARAR y ENVIAR la solicitud PUT 
            // La fecha que viene del formulario ya estará en un formato fácil de parsear por la API
            // (generalmente yyyy-MM-dd) si se usó el control <input type="date">.
            var json = JsonSerializer.Serialize(Conductor);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"api/Conductores/{Conductor.IdConductor}", content);

            // 5. MANEJO DE RESPUESTA
            if (response.IsSuccessStatusCode)
                return RedirectToPage("Conductores");

            // Si falla, lee y muestra el error de la API
            var respuestaApi = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", $"ERROR API: La API respondió con un error al intentar actualizar. Detalle: {respuestaApi}");
            return Page();
        }
    }
}