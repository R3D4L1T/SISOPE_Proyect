using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoColegio.Persistence.Data;
using ProyectoColegio.Domain;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;

namespace ProyectoColegio.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly ColegioDBContext _context;

        public UsuariosController(ColegioDBContext context)
        {
            _context = context;
        }

        // GET: Usuarios
        [Authorize(Roles = "Administrador, Docente, Estudiante")]
        public async Task<IActionResult> Index()
        {
              return _context.Usuario != null ? 
                          View(await _context.Usuario.ToListAsync()) :
                          Problem("Entity set 'ColegioDBContext.Usuario'  is null.");
        }
        private void Descendente<T, TKey>(List<T> list, Func<T, TKey> keySelector)
        {
            int n = list.Count;
            bool intercambio;
            do
            {
                intercambio = false;
                for (int i = 1; i < n; i++)
                {
                    if (Comparer<TKey>.Default.Compare(keySelector(list[i - 1]), keySelector(list[i])) < 0)
                    {
                        T temp = list[i - 1];
                        list[i - 1] = list[i];
                        list[i] = temp;
                        intercambio = true;
                    }
                }
                n--;
            } while (intercambio);
        }

        private void Ascendente<T, TKey>(List<T> list, Func<T, TKey> keySelector)
        {
            int n = list.Count;
            bool intercambio;
            do
            {
                intercambio = false;
                for (int i = 1; i < n; i++)
                {
                    if (Comparer<TKey>.Default.Compare(keySelector(list[i - 1]), keySelector(list[i])) > 0)
                    {
                        T temp = list[i - 1];
                        list[i - 1] = list[i];
                        list[i] = temp;
                        intercambio = true;
                    }
                }
                n--;
            } while (intercambio);
        }
        // GET: Usuarios/Details/5
        [Authorize(Roles = "Administrador, Docente, Estudiante")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Usuario == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(m => m.UsuarioID == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuarios/Create
        [Authorize(Roles = "Administrador, Docente, Estudiante")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Docente, Estudiante")]
        public async Task<IActionResult> Create([Bind("UsuarioID,RoleID,Dni,Nombres,Apellidos,Correo,Password,TipoGrado,Grado")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                var roleId = 3; // Puedes ajustar esto según tu lógica específica
                var rol = await _context.Rol.FindAsync(roleId);

                // Asignar el rol al usuario
                usuario.Rol = rol;

                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(usuario.Password));

                    StringBuilder stringBuilder = new StringBuilder();
                    for (int i = 0; i < hashedBytes.Length; i++)
                    {
                        stringBuilder.Append(hashedBytes[i].ToString("x2"));
                    }
                    usuario.Password = stringBuilder.ToString();
                }

                _context.Add(usuario);
                await _context.SaveChangesAsync();

                // Recuperar las clases que coinciden con el TipoGrado y Grado del nuevo usuario
                var clases = _context.Clases
                    .Where(c => c.TipoGrado == usuario.TipoGrado && c.Grado == usuario.Grado)
                    .ToList();

                // Obtener el ID del usuario recién creado
                var usuarioId = usuario.UsuarioID;

                // Crear las entradas en la tabla ClasesEstudiantes
                foreach (var clase in clases)
                {
                    var claseEstudiante = new ClasesEstudiantes
                    {
                        ClasesID = clase.ClaseID,
                        UsuarioID = usuarioId,
                        TipoGrado = usuario.TipoGrado,
                        Grado = usuario.Grado
                    };

                    _context.Add(claseEstudiante);
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(usuario);
        }


        // GET: Usuarios/Edit/5
        [Authorize(Roles = "Administrador, Docente, Estudiante")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Usuario == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Docente, Estudiante")]
        public async Task<IActionResult> Edit(int id, [Bind("UsuarioID,RoleID,Dni,Nombres,Apellidos,Correo,Password,TipoGrado,Grado")] Usuario usuario)
        {
            if (id != usuario.UsuarioID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Obtener el usuario actual de la base de datos
                    var usuarioActual = await _context.Usuario.FindAsync(id);

                    // Asignar el valor actual de RoleID al modelo antes de la actualización
                    usuario.RoleID = usuarioActual.RoleID;

                    // Actualizar el resto de los campos
                    _context.Entry(usuarioActual).CurrentValues.SetValues(usuario);

                    // Guardar los cambios
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.UsuarioID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }



        // GET: Usuarios/Delete/5
        [Authorize(Roles = "Administrador, Docente, Estudiante")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Usuario == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(m => m.UsuarioID == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Docente, Estudiante")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Usuario == null)
            {
                return Problem("Entity set 'ColegioDBContext.Usuario'  is null.");
            }
            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuario.Remove(usuario);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Administrador, Docente, Estudiante")]
        private bool UsuarioExists(int id)
        {
          return (_context.Usuario?.Any(e => e.UsuarioID == id)).GetValueOrDefault();
        }
        [Authorize(Roles = "Administrador, Docente, Estudiante")]
        public async Task<IActionResult> DetallesCursos(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Realiza la consulta SQL y obtén los resultados
            var resultados = await _context.ClasesEstudiantes
                .Join(_context.Clases, cs => cs.ClasesID, c => c.ClaseID, (cs, c) => new { cs, c })
                .Join(_context.Cursos, j => j.c.CursoID, cur => cur.CursoID, (j, cur) => new { j.cs, cur })
                .Where(j => j.cs.UsuarioID == id)
                .Select(j => new DetalleCursoViewModel
                {
                    NombreCurso = j.cur.nombre,
                    C1 = j.cs.C1,
                    C2 = j.cs.C2,
                    C3 = j.cs.C3,
                    C4 = j.cs.C4,
                    Final = j.cs.Final
                })
                .ToListAsync();

            if (resultados == null || resultados.Count == 0)
            {
                return NotFound();
            }

            return View(resultados);
        }
        [Authorize(Roles = "Administrador, Docente, Estudiante")]
        public async Task<IActionResult> Seccion()
        {
            // Obtén los claims de Grado y TipoGrado del usuario
            var claimGrado = User.Claims.FirstOrDefault(c => c.Type == "Grado")?.Value;
            var claimTipoGrado = User.Claims.FirstOrDefault(c => c.Type == "TipoGrado")?.Value;

            // Filtra los usuarios según los claims
            var usuarios = await _context.Usuario
                .Where(u => u.Grado == int.Parse(claimGrado) && u.TipoGrado == claimTipoGrado)
                .ToListAsync();

            return View(usuarios);
        }
        [Authorize(Roles = "Administrador, Docente, Estudiante")]
        public async Task<IActionResult> Horario()
        {
            // Obtén los claims de Grado y TipoGrado del usuario
            var claimGrado = User.Claims.FirstOrDefault(c => c.Type == "Grado")?.Value;
            var claimTipoGrado = User.Claims.FirstOrDefault(c => c.Type == "TipoGrado")?.Value;

            // Filtra las clases según los claims
            var clases = await _context.Clases
                .Include(c => c.curso)
                .Where(c => c.Grado == int.Parse(claimGrado) && c.TipoGrado == claimTipoGrado)
                .ToListAsync();

            return View(clases);
        }


    }
}
