using System.ComponentModel.DataAnnotations;

namespace ProyectoColegio.Domain
{
    public class Cursos
    {
        [Key]
        public int CursoID { get; set; }
        [Required(ErrorMessage = "El nombre es requerido")]
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public string? imagen { get; set; }
        //public ICollection<Clases>? clases { get; set; }

        // Cambios realizados para hacer la colección de solo lectura
        private readonly List<Clases> _clases = new List<Clases>();
        public IReadOnlyCollection<Clases> Clases => _clases.AsReadOnly();

        // Método para agregar elementos a la colección
        public void AgregarClase(Clases clase)
        {
            if (clase == null) throw new ArgumentNullException(nameof(clase));
            _clases.Add(clase);
        }

        // Método para remover elementos de la colección (opcional)
        public bool RemoverClase(Clases clase)
        {
            if (clase == null) throw new ArgumentNullException(nameof(clase));
            return _clases.Remove(clase);
        }
    }
}
