using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using TechStore.Data;
using TechStore.Services;
using TechStore.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Capa de datos: Entity Framework Core con SQL Server.
builder.Services.AddDbContext<TechStoreDbContext>(opciones =>
    opciones.UseSqlServer(builder.Configuration.GetConnectionString("TechStoreConnection")));

// Inversión de control: los controladores piden interfaces y el contenedor
// decide qué implementación entregar.
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();

var app = builder.Build();

// Aplica las migraciones pendientes al iniciar, de modo que la base de datos
// y los datos iniciales existan aunque el proyecto se ejecute en otro equipo.
using (var alcance = app.Services.CreateScope())
{
    var contexto = alcance.ServiceProvider.GetRequiredService<TechStoreDbContext>();
    contexto.Database.Migrate();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
