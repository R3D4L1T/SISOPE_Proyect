using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoColegio.Persistence.Data;
using ProyectoColegio.Domain;
using Microsoft.AspNetCore.Authorization;

namespace ProyectoColegio.Controllers
{
    public class ClasesController : Controller
    {
        private readonly ColegioDBContext _context;

        public ClasesController(ColegioDBContext context)
        {
            _context = context;
        }

        // GET: Clases
        // GET: Clases
        [Authorize(Roles = "Administrador, Docente, Estudiante")]
        public async Task<IActionResult> Index(string campoOrden, string ordenActual, string searchName)
        {
            if (string.IsNullOrEmpty(campoOrden))
            {
                campoOrden = "nombre";
            }

            if (string.IsNullOrEmpty(ordenActual))
            {
                ordenActual = "asc";
            }
            else
            {
                ordenActual = ordenActual == "asc" ? "desc" : "asc";
            }

            var query = _context.Clases
                .Include(c => c.curso)
                .Include(c => c.docente)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchName))
            {
                query = query.Where(c => c.curso.nombre.Contains(searchName) || c.docente.Apellidos.Contains(searchName));
            }

            List<Clases> clases;

            switch (campoOrden)
            {
                case "nombre":
                    clases = await query.ToListAsync();
                    if (ordenActual == "asc")
                    {
                        Ascendente(clases, c => c.curso.nombre);
                    }
                    else
                    {
                        Descendente(clases, c => c.curso.nombre);
                    }
                    break;
                case "docente":
                    clases = await query.ToListAsync();
                    if (ordenActual == "asc")
                    {
                        Ascendente(clases, c => c.docente.Apellidos);
                    }
                    else
                    {
                        Descendente(clases, c => c.docente.Apellidos);
                    }
                    break;
                default:
                    clases = await query.ToListAsync();
                    break;
            }

            // Almacena el campo de ordenamiento y el orden actual en ViewData
            ViewData["CampoOrden"] = campoOrden;
            ViewData["OrdenActual"] = ordenActual;

            return View(clases);
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

        // GET: Clases/Details/5
        [Authorize(Roles = "Administrador, Docente, Estudiante")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Clases == null)
            {
                return NotFound();
            }

            var clase = await _context.Clases
                .Include(c => c.curso)
                .Include(c => c.docente)
                .FirstOrDefaultAsync(m => m.ClaseID == id);
            if (clase == null)
            {
                return NotFound();
            }

            return View(clase);
        }

        // GET: Clases/Create
        [Authorize(Roles = "Administrador, Docente, Estudiante")]
        public IActionResult Create()
        {
            ViewData["Cursos"] = new SelectList(_context.Cursos, "CursoID", "nombre");
            ViewData["Docentes"] = new SelectList(_context.Docente, "DocenteID", "Apellidos");
            return View();
        }

        // POST: Clases/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Docente, Estudiante")]
        public async Task<IActionResult> Create([Bind("DocenteID,CursoID,DiaClase,HoraInicio,HoraFin,TipoGrado,Grado")] Clases clase)
        {
            if (ModelState.IsValid)
            {
                _context.Add(clase);
                await _context.SaveChangesAsync();

                // Obtén el ID de la clase recién creada
                var claseId = clase.ClaseID;

                // Recupera los usuarios que cumplen con los criterios de tipoGrado y Grado
                var usuarios = _context.Usuario
                    .Where(u => u.TipoGrado == clase.TipoGrado && u.Grado == clase.Grado)
                    .ToList();

                // Crea las entradas en la tabla ClasesEstudiantes
                foreach (var usuario in usuarios)
                {
                    var claseEstudiante = new ClasesEstudiantes
                    {
                        ClasesID = claseId,
                        UsuarioID = usuario.UsuarioID,
                        TipoGrado = clase.TipoGrado, // Asigna el valor de TipoGrado
                        Grado = clase.Grado // Asigna el valor de Grado
                    };

                    _context.Add(claseEstudiante);
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewData["Cursos"] = new SelectList(_context.Cursos, "CursoID", "nombre", clase.CursoID);
            ViewData["Docentes"] = new SelectList(_context.Docente, "DocenteID", "Apellidos", clase.DocenteID);
            return View(clase);
        }






        // GET: Clases/Edit/5
        [Authorize(Roles = "Administrador, Docente, Estudiante")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Clases == null)
            {
                return NotFound();
            }

            var clase = await _context.Clases.FindAsync(id);
            if (clase == null)
            {
                return NotFound();
            }

            // Asegúrate de inicializar ViewBag.Docentes antes de devolver la vista
            ViewBag.Docentes = new SelectList(_context.Docente, "DocenteID", "Apellidos", clase.DocenteID);

            ViewData["Cursos"] = new SelectList(_context.Cursos, "CursoID", "nombre", clase.CursoID);
            ViewData["Docentes"] = new SelectList(_context.Docente, "DocenteID", "Apellidos", clase.DocenteID);
            return View(clase);
        }





        // POST: Clases/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Docente, Estudiante")]
        public async Task<IActionResult> Edit(int id, [Bind("ClaseID,DocenteID,CursoID,DiaClase,HoraInicio,HoraFin,TipoGrado,Grado")] Clases clase)
        {
            if (id != clase.ClaseID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(clase);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClaseExists(clase.ClaseID))
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
            ViewData["CursoID"] = new SelectList(_context.Cursos, "CursoID", "nombre", clase.CursoID);
            ViewData["DocenteID"] = new SelectList(_context.Docente, "DocenteID", "Apellidos", clase.DocenteID);
            return View(clase);
        }

        // GET: Clases/Delete/5
        [Authorize(Roles = "Administrador, Docente, Estudiante")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Clases == null)
            {
                return NotFound();
            }

            var clase = await _context.Clases
                .Include(c => c.curso)
                .Include(c => c.docente)
                .FirstOrDefaultAsync(m => m.ClaseID == id);
            if (clase == null)
            {
                return NotFound();
            }

            return View(clase);
        }

        // POST: Clases/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Docente, Estudiante")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Clases == null)
            {
                return Problem("Entity set 'ColegioDBContext.Clases'  is null.");
            }
            var clase = await _context.Clases.FindAsync(id);
            if (clase != null)
            {
                _context.Clases.Remove(clase);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClaseExists(int id)
        {
            return (_context.Clases?.Any(e => e.ClaseID == id)).GetValueOrDefault();
        }
    }
}
