using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoColegio.Persistence.Data;
using ProyectoColegio.Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;

namespace ProyectoColegio.Controllers
{
    public class CursosController : Controller
    {
        private readonly ColegioDBContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CursosController(ColegioDBContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Cursos
        public async Task<IActionResult> VistadeCursos()
        {
            return _context.Cursos != null ?
                        View(await _context.Cursos.ToListAsync()) :
                        Problem("Entity set 'ColegioDBContext.Cursos'  is null.");
        }
        // GET: Cursos
        [Authorize(Roles = "Administrador, Docente, Estudiante")]
        public async Task<IActionResult> Index()
        {
              return _context.Cursos != null ? 
                          View(await _context.Cursos.ToListAsync()) :
                          Problem("Entity set 'ColegioDBContext.Cursos'  is null.");
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
        // GET: Cursos/Details/5
        [Authorize(Roles = "Administrador, Docente, Estudiante")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Cursos == null)
            {
                return NotFound();
            }

            var cursos = await _context.Cursos
                .FirstOrDefaultAsync(m => m.CursoID == id);
            if (cursos == null)
            {
                return NotFound();
            }

            return View(cursos);
        }

        // GET: Cursos/Create
        [Authorize(Roles = "Administrador, Docente, Estudiante")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cursos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CursoID,nombre,descripcion")] Cursos cursos, IFormFile imagen)
        {
            if (ModelState.IsValid)
            {
                if (imagen != null && imagen.Length > 0)
                {
                    // Verificar la imagen enviada
                    string webRootPath = _webHostEnvironment.WebRootPath;
                    string upload = Path.Combine(webRootPath, "uploads");

                    if (!Directory.Exists(upload))
                    {
                        Directory.CreateDirectory(upload);
                    }

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imagen.FileName);
                    string filePath = Path.Combine(upload, fileName);

                    // Cargar en el servidor
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imagen.CopyToAsync(fileStream);
                    }

                    cursos.imagen = fileName;

                }

                _context.Add(cursos);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(cursos);
        }

        // GET: Cursos/Edit/5
        [Authorize(Roles = "Administrador, Docente, Estudiante")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Cursos == null)
            {
                return NotFound();
            }

            var cursos = await _context.Cursos.FindAsync(id);
            if (cursos == null)
            {
                return NotFound();
            }
            return View(cursos);
        }

        // POST: Cursos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Docente, Estudiante")]
        public async Task<IActionResult> Edit(int id, [Bind("CursoID,nombre,descripcion")] Cursos cursos)
        {
            if (id != cursos.CursoID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cursos);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CursosExists(cursos.CursoID))
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
            return View(cursos);
        }

        // GET: Cursos/Delete/5
        [Authorize(Roles = "Administrador, Docente, Estudiante")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Cursos == null)
            {
                return NotFound();
            }

            var cursos = await _context.Cursos
                .FirstOrDefaultAsync(m => m.CursoID == id);
            if (cursos == null)
            {
                return NotFound();
            }

            return View(cursos);
        }

        // POST: Cursos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Docente, Estudiante")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Cursos == null)
            {
                return Problem("Entity set 'ColegioDBContext.Cursos'  is null.");
            }
            var cursos = await _context.Cursos.FindAsync(id);
            if (cursos != null)
            {
                _context.Cursos.Remove(cursos);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CursosExists(int id)
        {
            return (_context.Cursos?.Any(e => e.CursoID == id)).GetValueOrDefault();
        }
    }
}
