using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechStore.Models;
using TechStore.ViewModels;

namespace TechStore.Controllers
{
    [Authorize(Roles = RolesSistema.Administrador)]
    public class UsuariosController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<UsuariosController> _logger;

        public UsuariosController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<UsuariosController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var actual = _userManager.GetUserId(User);
            var usuarios = await _userManager.Users.OrderBy(u => u.Email).ToListAsync();
            var lista = new List<UsuarioRolViewModel>();

            foreach (var usuario in usuarios)
            {
                var roles = await _userManager.GetRolesAsync(usuario);
                lista.Add(new UsuarioRolViewModel
                {
                    Id = usuario.Id,
                    Correo = usuario.Email ?? usuario.UserName ?? string.Empty,
                    Rol = roles.FirstOrDefault() ?? "Sin rol",
                    EsUsuarioActual = usuario.Id == actual
                });
            }

            _logger.LogInformation("Consulta de usuarios por {Usuario}: {Cantidad} registros",
                User?.Identity?.Name, lista.Count);

            return View(lista);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarRol(string id, string rol)
        {
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario is null)
            {
                return NotFound();
            }

            if (!await _roleManager.RoleExistsAsync(rol))
            {
                TempData["Error"] = "El rol seleccionado no existe.";
                return RedirectToAction(nameof(Index));
            }

            if (usuario.Id == _userManager.GetUserId(User))
            {
                TempData["Error"] = "No puede cambiar su propio rol.";
                return RedirectToAction(nameof(Index));
            }

            var rolesActuales = await _userManager.GetRolesAsync(usuario);
            await _userManager.RemoveFromRolesAsync(usuario, rolesActuales);
            await _userManager.AddToRoleAsync(usuario, rol);

            _logger.LogInformation("Rol de {Correo} cambiado a {Rol} por {Usuario}",
                usuario.Email, rol, User?.Identity?.Name);

            TempData["Mensaje"] = $"El rol de {usuario.Email} ahora es {rol}.";
            return RedirectToAction(nameof(Index));
        }
    }
}
