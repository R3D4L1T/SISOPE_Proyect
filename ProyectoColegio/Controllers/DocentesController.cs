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
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace ProyectoColegio.Controllers
{
    public class DocentesController : Controller
    {
        private readonly ColegioDBContext _context;

        public DocentesController(ColegioDBContext context)
        {
            _context = context;
        }

        // GET: Docentes
        [Authorize(Roles = "Administrador, Docente, Estudiante")]
        public async Task<IActionResult> Index()
        {
              return _context.Docente != null ? 
                          View(await _context.Docente.ToListAsync()) :
                          Problem("Entity set 'ColegioDBContext.Docente'  is null.");
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
        // GET: Docentes/Details/5
        [Authorize(Roles = "Administrador, Docente, Estudiante")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Docente == null)
            {
                return NotFound();
            }

            var docente = await _context.Docente
                .FirstOrDefaultAsync(m => m.DocenteID == id);
            if (docente == null)
            {
                return NotFound();
            }

            return View(docente);
        }

        // GET: Docentes/Create
        [Authorize(Roles = "Administrador, Docente, Estudiante")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Docente, Estudiante")]
        public async Task<IActionResult> Create([Bind("DocenteID,Dni,Nombres,Apellidos")] Docente docente)
        {
            if (ModelState.IsValid)
            {
                _context.Add(docente);
                await _context.SaveChangesAsync();

                // Después de guardar el docente, creamos el usuario asociado
                var roleId = 2; // Puedes ajustar esto según tu lógica específica
                var rol = await _context.Rol.FindAsync(roleId);


                var nuevoUsuario = new Usuario
                {
                    Dni = docente.Dni,
                    Nombres = docente.Nombres,
                    Apellidos = docente.Apellidos,
                    Correo = $"{docente.Dni}{docente.Nombres[0]}{docente.Apellidos[0]}@colegio.com",
                    Password = "12345678", // Contraseña encriptada
                    TipoGrado = "Profesor",
                    Grado = 0,
                    RoleID = 2 
                };
                nuevoUsuario.Rol = rol;
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(nuevoUsuario.Password));

                    StringBuilder stringBuilder = new StringBuilder();
                    for (int i = 0; i < hashedBytes.Length; i++)
                    {
                        stringBuilder.Append(hashedBytes[i].ToString("x2"));
                    }
                    nuevoUsuario.Password = stringBuilder.ToString();
                }

                _context.Add(nuevoUsuario);
                await _context.SaveChangesAsync();

                // Asignar el IdLogin del Docente al Id del último Usuario creado
                docente.IdLogin = nuevoUsuario.UsuarioID;
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(docente);
        }



        // GET: Docentes/Edit/5
        [Authorize(Roles = "Administrador, Docente, Estudiante")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Docente == null)
            {
                return NotFound();
            }

            var docente = await _context.Docente.FindAsync(id);
            if (docente == null)
            {
                return NotFound();
            }
            return View(docente);
        }

        // POST: Docentes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Docente, Estudiante")]
        public async Task<IActionResult> Edit(int id, [Bind("DocenteID,Dni,Nombres,Apellidos")] Docente docente)
        {
            if (id != docente.DocenteID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(docente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DocenteExists(docente.DocenteID))
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
            return View(docente);
        }

        // GET: Docentes/Delete/5
        [Authorize(Roles = "Administrador, Docente, Estudiante")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Docente == null)
            {
                return NotFound();
            }

            var docente = await _context.Docente
                .FirstOrDefaultAsync(m => m.DocenteID == id);
            if (docente == null)
            {
                return NotFound();
            }

            return View(docente);
        }

        // POST: Docentes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Docente, Estudiante")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Docente == null)
            {
                return Problem("Entity set 'ColegioDBContext.Docente'  is null.");
            }
            var docente = await _context.Docente.FindAsync(id);
            if (docente != null)
            {
                _context.Docente.Remove(docente);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DocenteExists(int id)
        {
          return (_context.Docente?.Any(e => e.DocenteID == id)).GetValueOrDefault();
        }
        public async Task<IActionResult> HorarioDocente(int idLogin)
        {
            // Realiza la consulta SQL y obtén los resultados
            var resultados = await _context.Docente
                .Where(d => d.IdLogin == idLogin)
                .Join(_context.Clases, d => d.DocenteID, c => c.DocenteID, (d, c) => new { d, c })
                .Join(_context.Cursos, j => j.c.CursoID, curs => curs.CursoID, (j, curs) => new { j.c.HoraInicio, j.c.HoraFin, curs.nombre })
                .ToListAsync();

            if (resultados == null || resultados.Count == 0)
            {
                return NotFound();
            }

            return View(resultados);
        }
        [Authorize(Roles = "Administrador, Docente, Estudiante")]
        public async Task<IActionResult> Notas()
        {
            // Obtén el IdLogin del docente que se ha autenticado
            var claimIdLogin = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (claimIdLogin != null)
            {
                int idLogin = int.Parse(claimIdLogin);

                // Realiza la consulta SQL y obtén los resultados
                var notas = await _context.ClasesEstudiantes
                    .Include(ce => ce.Clases)
                        .ThenInclude(c => c.curso)
                    .Include(ce => ce.Usuario)  // Incluye la propiedad Usuario
                    .Include(ce => ce.Clases.docente)  // Incluye la propiedad docente dentro de Clases
                    .Where(ce => ce.Clases.docente.IdLogin == idLogin)
                    .ToListAsync();

                return View(notas);
            }

            // Manejar el caso en que no se encuentre el IdLogin en los claims
            return NotFound("No se encontró el IdLogin en los claims del usuario.");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Docente, Estudiante")]
        public async Task<IActionResult> EditarNotas(List<ClasesEstudiantes> notas)
        {
            if (ModelState.IsValid)
            {
                // Itera sobre las notas y actualiza la base de datos
                foreach (var nota in notas)
                {
                    _context.Entry(nota).State = EntityState.Modified;
                }

                await _context.SaveChangesAsync();

                // Puedes redirigir a la acción Notas después de guardar cambios si es necesario
                return RedirectToAction(nameof(Notas));
            }

            // Manejar el caso en que el modelo no sea válido
            return View("Notas", notas);
        }






    }
}
