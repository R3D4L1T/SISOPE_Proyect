using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using ProyectoColegio.Domain;
using System.Security.Claims;
using System.Text;
using System;
using ProyectoColegio.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace ProyectoColegio.Controllers
{
    public class AccessController : Controller
    {
        private readonly ColegioDBContext _context;

        public AccessController(ColegioDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> Login(Usuario user)
        {
            if (user.Correo != null && user.Password != null)
            {
                //Validar los credenciales
                var _user = validarCredenciales(user.Correo, user.Password);

                if (_user != null)
                {
                    var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, _user.UsuarioID.ToString()),
                    new Claim(ClaimTypes.Name, _user.Nombres+ " " + _user.Apellidos),
                    new Claim(ClaimTypes.Email, _user.Correo),
                    new Claim(ClaimTypes.SerialNumber, _user.Dni),
                    new Claim(ClaimTypes.Role, _user.Rol.Description),
                    new Claim("Grado", _user.Grado.ToString()), // Agregar Grado como Claim
                    new Claim("TipoGrado", _user.TipoGrado)
                };

                    var claimsIdentity = new ClaimsIdentity(claims,
                                                            CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                                    new ClaimsPrincipal(claimsIdentity));
                    return RedirectToAction("Index", "Home");
                }
                ViewData["Mensaje"] = "El usuario y/o la contraseña son incorrectos.";
            }
            return View();

        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Access");
        }

        [HttpGet]
        public IActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registrar(Usuario user)
        {
            if (user.Password != null)
            {
                user.Password = Encriptar(user.Password);
                user.RoleID = 1;
                if (ModelState.IsValid)
                {
                    // Validación de unicidad para usuario, correo y número de celular
                    if (_context.Usuario.Any(u =>
                        u.Correo == user.Correo ||
                        u.Dni == user.Dni))
                    {
                        if (_context.Usuario.Any(u => u.Correo == user.Correo))
                            ModelState.AddModelError("correo", "Ya existe un usuario con este correo.");

                        if (_context.Usuario.Any(u => u.Dni == user.Dni))
                            ModelState.AddModelError("documento", "Ya existe un usuario con este documento.");                       

                        return View(user);
                    }

                    _context.Usuario.Add(user);
                    _context.SaveChanges();
                    ViewData["Mensaje"] = "Usuario registrado exitosamente";
                    return RedirectToAction("Login", "Access");
                }
            }
            return View();
        }

        private Usuario validarCredenciales(string alias, string clave)
        {
            clave = Encriptar(clave);
            var tipo_usuario = _context.Usuario.Include(r => r.Rol)
                                    .Where(u => u.Correo == alias)
                                    .Where(u => u.Password == clave)
                                    .FirstOrDefault();
            return tipo_usuario;
        }

        public static string Encriptar(string texto)
        {
            StringBuilder sb = new StringBuilder();
            using (SHA256 hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(texto));
                foreach (byte b in result)
                {
                    sb.Append(b.ToString("x2"));
                }
            }
            return sb.ToString();
        }
        // UsuariosController.cs



    }
}
