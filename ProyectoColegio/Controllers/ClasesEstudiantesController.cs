using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoColegio.Persistence.Data;
using ProyectoColegio.Domain;
using System.Linq;
using Microsoft.AspNetCore.Authorization;


namespace ProyectoColegio.Controllers
{
    public class ClasesEstudiantesController : Controller
    {
        private readonly ColegioDBContext _context;

        public ClasesEstudiantesController(ColegioDBContext context)
        {
            _context = context;
        }

        // GET: ClasesEstudiantes
        [Authorize(Roles = "Administrador, Docente, Estudiante")]
        public async Task<IActionResult> Index(string campoOrden, string ordenActual, int? searchName)
        {
            if (string.IsNullOrEmpty(campoOrden))
            {
                campoOrden = "CURSO";
            }

            if (string.IsNullOrEmpty(ordenActual))
            {
                ordenActual = "asc";
            }
            else
            {
                ordenActual = ordenActual == "asc" ? "desc" : "asc";
            }

            var query = _context.ClasesEstudiantes
                .Include(ce => ce.Clases)
                    .ThenInclude(c => c.curso)
                .Include(ce => ce.Usuario)
                .AsQueryable();

            if (searchName.HasValue)
            {
                query = query.Where(ce => ce.ClasesID == searchName.Value);
            }

            List<ClasesEstudiantes> clasesEstudiantes;

            switch (campoOrden)
            {
                case "T1":
                    clasesEstudiantes = await Ordenar(query.ToList(), ce => ce.C1, ordenActual);
                    break;
                case "T2":
                    clasesEstudiantes = await Ordenar(query.ToList(), ce => ce.C2, ordenActual);
                    break;
                case "T3":
                    clasesEstudiantes = await Ordenar(query.ToList(), ce => ce.C3, ordenActual);
                    break;
                case "T4":
                    clasesEstudiantes = await Ordenar(query.ToList(), ce => ce.C4, ordenActual);
                    break;
                case "FINAL":
                    clasesEstudiantes = await Ordenar(query.ToList(), ce => ce.Final, ordenActual);
                    break;
                default:
                    clasesEstudiantes = await query.ToListAsync();
                    break;
            }

            // Almacena el campo de ordenamiento y el orden actual en ViewData
            ViewData["CampoOrden"] = campoOrden;
            ViewData["OrdenActual"] = ordenActual;

            return View(clasesEstudiantes);
        }

        private async Task<List<ClasesEstudiantes>> Ordenar<T>(List<ClasesEstudiantes> list, Func<ClasesEstudiantes, T> keySelector, string orden)
        {
            if (orden == "asc")
            {
                return await Task.Run(() => Ascendente(list, keySelector));
            }
            else
            {
                return await Task.Run(() => Descendente(list, keySelector));
            }
        }

        private List<ClasesEstudiantes> Descendente<T>(List<ClasesEstudiantes> list, Func<ClasesEstudiantes, T> keySelector)
        {
            int n = list.Count;
            bool intercambio;
            do
            {
                intercambio = false;
                for (int i = 1; i < n; i++)
                {
                    if (Comparer<T>.Default.Compare(keySelector(list[i - 1]), keySelector(list[i])) < 0)
                    {
                        ClasesEstudiantes temp = list[i - 1];
                        list[i - 1] = list[i];
                        list[i] = temp;
                        intercambio = true;
                    }
                }
                n--;
            } while (intercambio);

            return list;
        }

        private List<ClasesEstudiantes> Ascendente<T>(List<ClasesEstudiantes> list, Func<ClasesEstudiantes, T> keySelector)
        {
            int n = list.Count;
            bool intercambio;
            do
            {
                intercambio = false;
                for (int i = 1; i < n; i++)
                {
                    if (Comparer<T>.Default.Compare(keySelector(list[i - 1]), keySelector(list[i])) > 0)
                    {
                        ClasesEstudiantes temp = list[i - 1];
                        list[i - 1] = list[i];
                        list[i] = temp;
                        intercambio = true;
                    }
                }
                n--;
            } while (intercambio);

            return list;
        }


        [Authorize(Roles = "Administrador, Docente, Estudiante")]
        // GET: ClasesEstudiantes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ClasesEstudiantes == null)
            {
                return NotFound();
            }

            var clasesEstudiantes = await _context.ClasesEstudiantes
                .Include(c => c.Clases)
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(m => m.ClasesEstudiantesID == id);
            if (clasesEstudiantes == null)
            {
                return NotFound();
            }

            return View(clasesEstudiantes);
        }

        // GET: ClasesEstudiantes/Create
        [Authorize(Roles = "Administrador, Docente, Estudiante")]
        public IActionResult Create()
        {
            ViewData["ClasesID"] = new SelectList(_context.Clases, "ClaseID", "ClaseID");
            ViewData["UsuarioID"] = new SelectList(_context.Usuario, "UsuarioID", "Apellidos");
            return View();
        }

        // POST: ClasesEstudiantes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Docente, Estudiante")]
        public async Task<IActionResult> Create([Bind("ClasesEstudiantesID,ClasesID,UsuarioID,T1,T2,Final,Estado")] ClasesEstudiantes clasesEstudiantes)
        {
            if (ModelState.IsValid)
            {
                _context.Add(clasesEstudiantes);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClasesID"] = new SelectList(_context.Clases, "ClaseID", "ClaseID", clasesEstudiantes.ClasesID);
            ViewData["UsuarioID"] = new SelectList(_context.Usuario, "UsuarioID", "Apellidos", clasesEstudiantes.UsuarioID);
            return View(clasesEstudiantes);
        }

        // GET: ClasesEstudiantes/Edit/5
        [Authorize(Roles = "Administrador, Docente, Estudiante")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ClasesEstudiantes == null)
            {
                return NotFound();
            }

            var clasesEstudiantes = await _context.ClasesEstudiantes.FindAsync(id);
            if (clasesEstudiantes == null)
            {
                return NotFound();
            }
            ViewData["ClasesID"] = new SelectList(_context.Clases, "ClaseID", "ClaseID", clasesEstudiantes.ClasesID);
            ViewData["UsuarioID"] = new SelectList(_context.Usuario, "UsuarioID", "Apellidos", clasesEstudiantes.UsuarioID);
            return View(clasesEstudiantes);
        }

        // POST: ClasesEstudiantes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Docente, Estudiante")]
        public async Task<IActionResult> Edit(int id, [Bind("ClasesEstudiantesID,ClasesID,UsuarioID,C1,C2,C3,C4,Final,TipoGrado,Grado")] ClasesEstudiantes clasesEstudiantes)
        {
            if (id != clasesEstudiantes.ClasesEstudiantesID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(clasesEstudiantes);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClasesEstudiantesExists(clasesEstudiantes.ClasesEstudiantesID))
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
            ViewData["ClasesID"] = new SelectList(_context.Clases, "ClaseID", "ClaseID", clasesEstudiantes.ClasesID);
            ViewData["UsuarioID"] = new SelectList(_context.Usuario, "UsuarioID", "Apellidos", clasesEstudiantes.UsuarioID);
            return View(clasesEstudiantes);
        }


        // GET: ClasesEstudiantes/Delete/5
        [Authorize(Roles = "Administrador, Docente, Estudiante")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ClasesEstudiantes == null)
            {
                return NotFound();
            }

            var clasesEstudiantes = await _context.ClasesEstudiantes
                .Include(c => c.Clases)
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(m => m.ClasesEstudiantesID == id);
            if (clasesEstudiantes == null)
            {
                return NotFound();
            }

            return View(clasesEstudiantes);
        }

        // POST: ClasesEstudiantes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Docente, Estudiante")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ClasesEstudiantes == null)
            {
                return Problem("Entity set 'ColegioDBContext.ClasesEstudiantes'  is null.");
            }
            var clasesEstudiantes = await _context.ClasesEstudiantes.FindAsync(id);
            if (clasesEstudiantes != null)
            {
                _context.ClasesEstudiantes.Remove(clasesEstudiantes);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClasesEstudiantesExists(int id)
        {
          return (_context.ClasesEstudiantes?.Any(e => e.ClasesEstudiantesID == id)).GetValueOrDefault();
        }
        // Método para crear las ClasesEstudiantes al crear una nueva Clase
        private async Task CrearClasesEstudiantesAsync(Clases clase)
        {
            // Obtener usuarios del mismo TipoGrado y Grado que la clase
            var usuarios = await _context.Usuario
                .Where(u => u.TipoGrado == clase.TipoGrado && u.Grado == clase.Grado)
                .ToListAsync();

            // Crear ClasesEstudiantes para cada usuario
            foreach (var usuario in usuarios)
            {
                var clasesEstudiantes = new ClasesEstudiantes
                {
                    ClasesID = clase.ClaseID,
                    UsuarioID = usuario.UsuarioID,
                    TipoGrado = usuario.TipoGrado,
                    Grado = usuario.Grado
                };

                _context.ClasesEstudiantes.Add(clasesEstudiantes);
            }

            await _context.SaveChangesAsync();
        }

        // Método para obtener la lista de ClasesEstudiantes relacionadas
        private async Task<List<ClasesEstudiantes>> ObtenerClasesEstudiantesAsync(int claseId)
        {
            return await _context.ClasesEstudiantes
                .Where(ce => ce.ClasesID == claseId)
                .ToListAsync();
        }

        // Método para mostrar la vista de ClasesEstudiantes
        public async Task<IActionResult> ClasesEstudiantes(int claseId)
        {
            var clasesEstudiantes = await ObtenerClasesEstudiantesAsync(claseId);
            return View(clasesEstudiantes);
        }
        public async Task<IActionResult> ActualizarEstudiantes()
        {
            // Lógica para actualizar la base de datos según tus requisitos
            // Puedes utilizar el mismo enfoque que usamos al crear un nuevo usuario y asociarlo a clases

            // Obtener todos los estudiantes
            var estudiantes = _context.Usuario.Where(u => u.RoleID == 3).ToList();

            foreach (var estudiante in estudiantes)
            {
                // Lógica para asociar al estudiante con las clases correspondientes
                var clases = _context.Clases
                    .Where(c => c.TipoGrado == estudiante.TipoGrado && c.Grado == estudiante.Grado)
                    .ToList();

                foreach (var clase in clases)
                {
                    // Verificar si ya existe una entrada en ClasesEstudiantes para este estudiante y clase
                    var existente = _context.ClasesEstudiantes
                        .Any(ce => ce.ClasesID == clase.ClaseID && ce.UsuarioID == estudiante.UsuarioID);

                    if (!existente)
                    {
                        // Si no existe, crear una nueva entrada en ClasesEstudiantes
                        var claseEstudiante = new ClasesEstudiantes
                        {
                            ClasesID = clase.ClaseID,
                            UsuarioID = estudiante.UsuarioID,
                            TipoGrado = estudiante.TipoGrado,
                            Grado = estudiante.Grado
                        };

                        _context.Add(claseEstudiante);
                    }
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


    }
}
