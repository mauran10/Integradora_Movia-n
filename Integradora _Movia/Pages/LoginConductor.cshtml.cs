// Pages/LoginConductor.cshtml.cs
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using System.Security.Claims;
using Integradora_Movia.Models;

namespace Integradora_Movia.Pages
{
    public class LoginConductorModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        [BindProperty(SupportsGet = true)]
        public string ReturnUrl { get; set; } = string.Empty;

        [BindProperty]
        public LoginConductorApiModel Input { get; set; } = new LoginConductorApiModel();

        [TempData]
        public string ErrorMessage { get; set; } = string.Empty;

        public LoginConductorModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");
            ErrorMessage = string.Empty;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");

            if (!ModelState.IsValid)
            {
                // Si el modelo no es válido (ej: faltan campos), regresa a la página
                return Page();
            }

            // === PASO CLAVE: Crear el payload que el API de Conductores espera (3 campos) ===
            var apiPayload = new
            {
                // Incluimos los tres campos requeridos por el Swagger
                numerolicencia = Input.NumeroLicencia,
                correoElectronico = Input.CorreoElectronico,
                contrasena = Input.Contrasena
                // Nota: Usamos nombres en camelCase (minúscula inicial) para coincidir con JSON/API
            };

            var client = _httpClientFactory.CreateClient("MovIA_API");

            // Usamos el endpoint correcto que ya está implementado en el API
            var apiEndpoint = "api/Conductores/login";

            var jsonContent = JsonSerializer.Serialize(apiPayload);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await client.PostAsync(apiEndpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    // === AUTENTICACIÓN LOCAL (CREACIÓN DE LA COOKIE) ===
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, Input.CorreoElectronico),
                        new Claim(ClaimTypes.Name, Input.CorreoElectronico),
                        new Claim(ClaimTypes.Role, "Conductor") // <== ¡ASIGNAMOS EL ROL!
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, "Cookies");

                    var authProperties = new AuthenticationProperties
                    {
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60),
                        IsPersistent = true
                    };

                    // CREA LA COOKIE Y FIRMA AL USUARIO
                    await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(claimsIdentity), authProperties);

                    // === REDIRECCIÓN ===
                    if (Url.IsLocalUrl(ReturnUrl))
                    {
                        return LocalRedirect(ReturnUrl);
                    }

                    // Redirección por defecto a la página principal de conductores
                    return RedirectToPage("/Mapa");
                }
                else
                {
                    // Fallo del API (ej: credenciales incorrectas)
                    ErrorMessage = $"Credenciales de Conductor incorrectas. Verifique correo, licencia y contraseña.";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                // Error de conexión o cualquier otra excepción
                Console.WriteLine($"[EXCEPCIÓN CONEXIÓN] Mensaje: {ex.Message}");
                ErrorMessage = $"Error de conexión con el API.";
                return Page();
            }
        }
    }
}