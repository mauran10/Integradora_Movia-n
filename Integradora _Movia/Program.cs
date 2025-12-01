using Microsoft.AspNetCore.Rewrite;

var builder = WebApplication.CreateBuilder(args);

// ==========================================================
// 1. CONFIGURACIÓN DE SERVICIOS (builder.Services)
// ==========================================================

// Configurar HttpClient (Paso crucial para la comunicación API)
builder.Services.AddHttpClient("MovIA_API", client =>
{
    // Usamos solo la base de la URL (sin /api/Usuarios)
    client.BaseAddress = new Uri("https://moviaconnect-f9gkcbg8f9dzh0g2.westus2-01.azurewebsites.net/");
    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
});

// Configuración del Servicio de Autenticación (Cookies)
builder.Services.AddAuthentication("Cookies") // 1. Nombre del esquema: "Cookies"
    .AddCookie("Cookies", options => // 2. Definición del esquema con ese nombre
    {
        // 3. Configuración para manejar la redirección automática a Login
        options.LoginPath = "/Login";
        options.AccessDeniedPath = "/AccessDenied"; // Opcional, pero recomendado
        options.Cookie.Name = "MoviaAuthCookie"; // Nombre personalizado de la cookie
        options.Cookie.HttpOnly = true; // Solo accesible por HTTP (seguridad)
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Duración de la sesión
        options.SlidingExpiration = true;
    });
// Servicio de Autorización (Necesario para el atributo [Authorize])
builder.Services.AddAuthorization();

builder.Services.AddRazorPages();

var app = builder.Build();

// ==========================================================
// 2. CONFIGURACIÓN DE MIDDLEWARE (app)
// ==========================================================

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();

app.UseRouting();

// Middlewares de Autenticación y Autorización
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();