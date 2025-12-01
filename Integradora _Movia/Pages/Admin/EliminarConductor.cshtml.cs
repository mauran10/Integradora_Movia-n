using Integradora_Movia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

namespace Integradora_Movia.Pages.Admin
{
    public class EliminarConductorModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public EliminarConductorModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public ConductorModel Conductor { get; set; }

        public async Task<IActionResult> OnGet(int id)
        {
            var client = _httpClientFactory.CreateClient("MovIA_API");

            var response = await client.GetAsync($"api/Conductores/{id}");

            if (!response.IsSuccessStatusCode)
                return RedirectToPage("Conductores");

            Conductor = await response.Content.ReadFromJsonAsync<ConductorModel>();
            return Page();
        }

        public async Task<IActionResult> OnPostDelete()
        {
            var client = _httpClientFactory.CreateClient("MovIA_API");

            var response = await client.DeleteAsync($"api/Conductores/{Conductor.IdConductor}");

            if (response.IsSuccessStatusCode)
                return RedirectToPage("Conductores");

            ModelState.AddModelError("", "No se pudo eliminar el conductor.");
            return Page();
        }
    }
}
