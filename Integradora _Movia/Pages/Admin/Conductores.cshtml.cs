using Integradora_Movia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace Integradora_Movia.Pages.Admin
{
    public class ConductoresModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public ConductoresModel(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://moviaconnect-f9gkcbg8f9dzh0g2.westus2-01.azurewebsites.net/");
        }

        public List<ConductorModel> Conductores { get; set; } = new();
        public string Message { get; set; } = "";

        public async Task OnGet()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/Conductores");

                if (!response.IsSuccessStatusCode)
                {
                    Message = "No se pudieron cargar los conductores.";
                    return;
                }

                var json = await response.Content.ReadAsStringAsync();
                Conductores = JsonSerializer.Deserialize<List<ConductorModel>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }
            catch
            {
                Message = "Error de conexión con el servidor.";
            }
        }

        public async Task<IActionResult> OnPostEliminar(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/Conductores/{id}");

            if (!response.IsSuccessStatusCode)
            {
                Message = "No se pudo eliminar el conductor.";
            }

            return RedirectToPage();
        }
    }
}
