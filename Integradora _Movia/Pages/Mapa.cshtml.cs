using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging; // Agregado para usar ILogger

namespace Integradora_Movia.Pages
{
    // ✅ Asegúrate de que el nombre de la clase sea MapaModel
    public class MapaModel : PageModel
    {
        private readonly ILogger<MapaModel> _logger;

        public MapaModel(ILogger<MapaModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}