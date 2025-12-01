using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace Integradora_Movia.Pages
{
    // Opcional, pero buena práctica: Asegura que solo los usuarios autenticados
    // puedan intentar cerrar sesión.
    [Authorize]
    public class LogoutModel : PageModel
    {
        // Usamos OnPostAsync para cumplir con los requisitos de Anti-Forgery Tokens (CSRF)
        // y porque es la forma estándar de manejar acciones que cambian el estado (como cerrar sesión).
        public async Task<IActionResult> OnPostAsync()
        {
            // Este es el comando mágico: borra la cookie de autenticación.
            await HttpContext.SignOutAsync("Cookies");

            // Redirigir al usuario a la página de inicio o de login después de cerrar sesión
            return RedirectToPage("/Index");
        }

        // Si alguien intenta acceder a /Logout sin usar el formulario POST, lo redirigimos
        public IActionResult OnGet()
        {
            return RedirectToPage("/Index");
        }
    }
}