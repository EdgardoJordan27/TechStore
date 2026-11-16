using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TechStore.Models;
using TechStore.ViewModels;

namespace TechStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ILogger<AccountController> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel modelo, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var usuario = await _userManager.FindByEmailAsync(modelo.Correo)
                          ?? await _userManager.FindByNameAsync(modelo.Correo);

            if (usuario is not null)
            {
                var resultado = await _signInManager.PasswordSignInAsync(
                    usuario, modelo.Clave, modelo.Recordarme, lockoutOnFailure: false);

                if (resultado.Succeeded)
                {
                    _logger.LogInformation("Inicio de sesión correcto: {Correo}", usuario.Email);
                    return RedirigirLocal(returnUrl);
                }
            }

            _logger.LogWarning("Intento de inicio de sesión fallido para {Correo}", modelo.Correo);
            ModelState.AddModelError(string.Empty, "Correo o contraseña incorrectos.");
            return View(modelo);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View(new RegistroViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegistroViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var usuario = new IdentityUser
            {
                UserName = modelo.Correo,
                Email = modelo.Correo
            };

            var resultado = await _userManager.CreateAsync(usuario, modelo.Clave);

            if (!resultado.Succeeded)
            {
                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                _logger.LogWarning("No se pudo registrar el usuario {Correo}", modelo.Correo);
                return View(modelo);
            }

            // Toda cuenta nueva se crea como Cliente; el administrador puede cambiarla luego.
            await _userManager.AddToRoleAsync(usuario, RolesSistema.Cliente);
            await _signInManager.SignInAsync(usuario, isPersistent: false);

            _logger.LogInformation("Usuario registrado: {Correo} con rol {Rol}", modelo.Correo, RolesSistema.Cliente);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var nombre = User?.Identity?.Name;
            await _signInManager.SignOutAsync();

            _logger.LogInformation("Cierre de sesión: {Correo}", nombre);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccesoDenegado()
        {
            _logger.LogWarning("Acceso denegado para {Usuario} en {Ruta}",
                User?.Identity?.Name ?? "anónimo", Request.Query["ReturnUrl"].ToString());

            Response.StatusCode = StatusCodes.Status403Forbidden;
            return View();
        }

        private IActionResult RedirigirLocal(string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
