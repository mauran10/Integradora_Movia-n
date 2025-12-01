using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

// ¡Este atributo es la clave de la seguridad!
[Authorize]
public class ReportesModel : PageModel
{
    private readonly ILogger<ReportesModel> _logger;

    public ReportesModel(ILogger<ReportesModel> logger)
    {
        _logger = logger;
    }

    // Esta función se ejecuta cuando alguien accede a la página
    public void OnGet()
    {
        // Puedes agregar lógica para cargar los reportes aquí
    }
}