using Integradora_Movia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Integradora_Movia.Pages.Admin
{
    public class CrearConductorModel : PageModel
    {
        private readonly IHttpClientFactory _http;

        public CrearConductorModel(IHttpClientFactory http)
        {
            _http = http;
        }

        [BindProperty]
        public ConductorModel Conductor { get; set; } = new ConductorModel();

        public string Message { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            var client = _http.CreateClient("MovIA_API");

            var json = JsonSerializer.Serialize(Conductor);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("api/Conductores", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("Conductores");
            }

            Message = "No se pudo crear el conductor.";
            return Page();
        }
    }
}
