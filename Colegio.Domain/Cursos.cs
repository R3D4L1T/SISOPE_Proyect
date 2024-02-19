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
        public ICollection<Clases>? clases { get; set; }
    }
}
