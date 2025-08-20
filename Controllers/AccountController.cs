using BCrypt.Net;
using ElGantte.Data;
using ElGantte.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ElGantte.ViewModels;

namespace ElGantte.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Login
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                ModelState.AddModelError("", "Credenciales incorrectas.");
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(claims, "MiCookieAuth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("MiCookieAuth", principal);

            return RedirectToAction("Index", "Home");
        }

        [Authorize(AuthenticationSchemes = "MiCookieAuth", Roles = "Admin,GodMode")]
        public async Task<IActionResult> Register()
        {
            var model = new RegisterViewModel
            {
                Usuarios = await _context.Usuarios.ToListAsync()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Register(string email, string password)
        {
            if (_context.Usuarios.Any(u => u.Email == email))
            {
                ModelState.AddModelError("", "El email ya está registrado.");
                return View();
            }

            var user = new Usuario
            {
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = "User"
            };

            _context.Usuarios.Add(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login");
        }

        [Authorize(AuthenticationSchemes = "MiCookieAuth", Roles = "GodMode")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUsuario(int id, string email, string role, string password)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            usuario.Email = email;
            usuario.Role = role;

            if (!string.IsNullOrEmpty(password))
            {
                usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            }

            _context.Update(usuario);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Register));
        }


        [Authorize(AuthenticationSchemes = "MiCookieAuth", Roles = "GodMode")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            // Generar contraseña segura
            var nuevaPassword = GenerarPasswordSegura(8); // Método que te muestro abajo

            // Guardar contraseña hasheada
            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(nuevaPassword);
            _context.Update(usuario);
            await _context.SaveChangesAsync();

            // Mostrar la nueva contraseña en pantalla (temporal)
            TempData["Success"] = "La nueva contraseña es: " + "nuevaPassword";

            return RedirectToAction(nameof(Register));
        }

        // Método para generar contraseña segura
        private string GenerarPasswordSegura(int length = 8)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+-.,";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }



        //Pagina de error si no tienes acceso
        [HttpGet]
        public IActionResult AccessDenied(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        //Pagina para logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MiCookieAuth");
            return RedirectToAction("Login", "Account");
        }
    }
}