using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TechStore.Data;

namespace TechStore.Tests.E2E
{
    // Levanta la aplicación completa (rutas, Identity, autorización, vistas Razor y EF Core)
    // en memoria. Solo se reemplaza SQL Server por una base de datos en memoria.
    public class TechStoreFactory : WebApplicationFactory<Program>
    {
        private readonly string _nombreBaseDeDatos = Guid.NewGuid().ToString();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(servicios =>
            {
                var registros = servicios
                    .Where(d => d.ServiceType.FullName != null
                                && d.ServiceType.FullName.Contains(nameof(TechStoreDbContext)))
                    .ToList();

                foreach (var registro in registros)
                {
                    servicios.Remove(registro);
                }

                servicios.AddDbContext<TechStoreDbContext>(opciones =>
                    opciones.UseInMemoryDatabase(_nombreBaseDeDatos));
            });
        }

        // Cliente sin redirecciones automáticas para poder comprobar los 302 (login, acceso denegado).
        public HttpClient CrearCliente()
        {
            return CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        public async Task<HttpClient> CrearClienteAutenticadoAsync(string correo, string clave)
        {
            var cliente = CrearCliente();
            var respuesta = await cliente.IniciarSesionAsync(correo, clave);

            if (respuesta.StatusCode != System.Net.HttpStatusCode.Redirect)
            {
                throw new InvalidOperationException($"No se pudo iniciar sesión con {correo}.");
            }

            return cliente;
        }
    }
}
