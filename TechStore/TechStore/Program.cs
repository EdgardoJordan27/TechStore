using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;
using TechStore.Data;
using TechStore.Infrastructure;
using TechStore.Services;
using TechStore.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Logging: consola (por defecto) y archivo en la carpeta Logs del proyecto.
builder.Logging.AddProvider(new ProveedorLogArchivo(Path.Combine(builder.Environment.ContentRootPath, "Logs")));

builder.Services.AddControllersWithViews();

// Capa de datos: Entity Framework Core con SQL Server.
builder.Services.AddDbContext<TechStoreDbContext>(opciones =>
    opciones.UseSqlServer(builder.Configuration.GetConnectionString("TechStoreConnection"))
            .AddInterceptors(new NumericRoundAbortInterceptor()));

// Autenticación y roles con ASP.NET Core Identity.
builder.Services.AddIdentity<IdentityUser, IdentityRole>(opciones =>
{
    opciones.Password.RequiredLength = 6;
    opciones.Password.RequireDigit = true;
    opciones.Password.RequireUppercase = true;
    opciones.Password.RequireLowercase = true;
    opciones.Password.RequireNonAlphanumeric = false;
    opciones.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<TechStoreDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(opciones =>
{
    opciones.LoginPath = "/Account/Login";
    opciones.AccessDeniedPath = "/Account/AccesoDenegado";
});

// Inversión de control: los controladores piden interfaces y el contenedor
// decide qué implementación entregar.
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IReporteService, ReporteService>();

// Licencia de QuestPDF (uso comunitario).
QuestPDF.Settings.License = LicenseType.Community;

var app = builder.Build();

// Aplica las migraciones pendientes al iniciar y crea los roles y usuarios iniciales.
// Con un proveedor que no es relacional (pruebas con base en memoria) solo se crea el esquema.
using (var alcance = app.Services.CreateScope())
{
    var contexto = alcance.ServiceProvider.GetRequiredService<TechStoreDbContext>();

    if (contexto.Database.IsRelational())
    {
        contexto.Database.Migrate();
    }
    else
    {
        contexto.Database.EnsureCreated();
    }

    await IdentidadInicial.SembrarAsync(alcance.ServiceProvider);
}

// Manejo global de errores: se usa en todos los entornos para mostrar la página personalizada.
app.UseExceptionHandler("/Home/Error");
app.UseStatusCodePagesWithReExecute("/Home/Error", "?codigo={0}");

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();

// Cultura fija para que los precios con punto decimal se interpreten igual en cualquier equipo.
var culturaFija = new CultureInfo("en-US");
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(culturaFija),
    SupportedCultures = new List<CultureInfo> { culturaFija },
    SupportedUICultures = new List<CultureInfo> { culturaFija }
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();

// Permite que el proyecto de pruebas use WebApplicationFactory<Program>.
public partial class Program
{
}
