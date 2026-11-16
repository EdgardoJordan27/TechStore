using Microsoft.AspNetCore.Identity;
using TechStore.Models;

namespace TechStore.Data
{
    // Crea los roles y tres usuarios de prueba (uno por rol) si todavía no existen.
    public static class IdentidadInicial
    {
        public const string CorreoAdministrador = "admin@techstore.com";
        public const string ClaveAdministrador = "Admin123";

        public const string CorreoVendedor = "vendedor@techstore.com";
        public const string ClaveVendedor = "Vendedor123";

        public const string CorreoCliente = "cliente@techstore.com";
        public const string ClaveCliente = "Cliente123";

        public static async Task SembrarAsync(IServiceProvider servicios)
        {
            var roleManager = servicios.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = servicios.GetRequiredService<UserManager<IdentityUser>>();
            var logger = servicios.GetRequiredService<ILoggerFactory>().CreateLogger("IdentidadInicial");

            foreach (var rol in RolesSistema.Todos)
            {
                if (!await roleManager.RoleExistsAsync(rol))
                {
                    var resultado = await roleManager.CreateAsync(new IdentityRole(rol));
                    if (resultado.Succeeded)
                    {
                        logger.LogInformation("Rol creado: {Rol}", rol);
                    }
                    else
                    {
                        logger.LogError("No se pudo crear el rol {Rol}", rol);
                    }
                }
            }

            await CrearUsuarioAsync(userManager, logger, CorreoAdministrador, ClaveAdministrador, RolesSistema.Administrador);
            await CrearUsuarioAsync(userManager, logger, CorreoVendedor, ClaveVendedor, RolesSistema.Vendedor);
            await CrearUsuarioAsync(userManager, logger, CorreoCliente, ClaveCliente, RolesSistema.Cliente);
        }

        private static async Task CrearUsuarioAsync(
            UserManager<IdentityUser> userManager, ILogger logger, string correo, string clave, string rol)
        {
            var usuario = await userManager.FindByEmailAsync(correo);

            if (usuario is null)
            {
                usuario = new IdentityUser
                {
                    UserName = correo,
                    Email = correo,
                    EmailConfirmed = true
                };

                var resultado = await userManager.CreateAsync(usuario, clave);
                if (!resultado.Succeeded)
                {
                    logger.LogError("No se pudo crear el usuario {Correo}: {Errores}",
                        correo, string.Join("; ", resultado.Errors.Select(e => e.Description)));
                    return;
                }

                logger.LogInformation("Usuario creado: {Correo}", correo);
            }

            if (!await userManager.IsInRoleAsync(usuario, rol))
            {
                await userManager.AddToRoleAsync(usuario, rol);
                logger.LogInformation("Rol {Rol} asignado a {Correo}", rol, correo);
            }
        }
    }
}
