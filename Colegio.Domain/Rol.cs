using System.ComponentModel.DataAnnotations;

namespace ProyectoColegio.Domain
{
    public class Rol
    {
        [Key]
        public int RoleID { get; set; }
        [MaxLength(30)] //Admin, Instructor, Estudiante
        public string Description { get; set; }
        public ICollection<Usuario>? Usuarios { get; set; }
    }
}
