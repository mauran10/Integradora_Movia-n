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
    public class LoginModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        // Propiedad que captura la URL de destino (ej: /Reportes)
        [BindProperty(SupportsGet = true)]
        public string ReturnUrl { get; set; } = string.Empty;

        // Utilizamos el modelo definido en Models/LoginApiModel.cs
        [BindProperty]
        public LoginApiModel Input { get; set; } = new LoginApiModel();

        // Para mensajes de error
        [TempData]
        public string ErrorMessage { get; set; } = string.Empty;

        // Constructor
        public LoginModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Se ejecuta al cargar la página (GET)
        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");
            ErrorMessage = string.Empty;
        }

        // Se ejecuta al enviar el formulario (POST)
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");

            if (!ModelState.IsValid)
            {
                // Si el modelo no es válido (ej: faltan campos), regresa a la página
                return Page();
            }

            // === PASO CLAVE: Crear el payload con el nombre de propiedad esperado por el API (ContrasenaHash) ===
            var apiPayload = new
            {
                Input.CorreoElectronico,
                // Mapea Input.Contrasena (lo que escribió el usuario) a ContrasenaHash (lo que espera el API)
                ContrasenaHash = Input.Contrasena
            };

            var client = _httpClientFactory.CreateClient("MovIA_API");
            var apiEndpoint = "api/Usuarios/Login";

            // Serializamos el nuevo objeto con el nombre ContrasenaHash
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
                        new Claim(ClaimTypes.Name, Input.CorreoElectronico)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, "Cookies");

                    var authProperties = new AuthenticationProperties
                    {
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30),
                        IsPersistent = true
                    };

                    // CREA LA COOKIE Y FIRMA AL USUARIO
                    await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(claimsIdentity), authProperties);

                    // === REDIRECCIÓN ===
                    // Usamos IsLocalUrl para verificar si la URL es segura antes de redirigir
                    if (Url.IsLocalUrl(ReturnUrl))
                    {
                        return LocalRedirect(ReturnUrl);
                    }

                    // Redirección por defecto a /Index
                    return RedirectToPage("/Index");
                }
                else
                {
                    // Fallo del API
                    ErrorMessage = $"Credenciales incorrectas. Verifique usuario y contraseña.";
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