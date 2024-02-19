using System.ComponentModel.DataAnnotations;
using System.Data;

namespace ProyectoColegio.Domain
{
    public class Usuario
    {
        [Key]
        public int UsuarioID { get; set; }
        public int RoleID { get; set; }
        [Required(ErrorMessage = "El DNI es obligatorio")]
        [MaxLength(15, ErrorMessage = "El DNI debe tener como máximo 15 digitos"), MinLength(8)]
        public string Dni { get; set; }
        [Required(ErrorMessage = "El Nombre es obligatorio")]
        [MaxLength(100, ErrorMessage = "El Nombre debe tener como máximo 100 digitos")]
        public string Nombres { get; set; }
        [Required(ErrorMessage = "El Apellido es obligatorio")]
        [MaxLength(100, ErrorMessage = "El Apellido debe tener como máximo 100 digitos")]
        public string Apellidos { get; set; }
        [EmailAddress]
        public string Correo { get; set; }
        public string Password { get; set; }

        [Required(ErrorMessage = "El Tipo de Grado es obligatorio")]
        public string TipoGrado { get; set; } // Primaria o Secundaria

        [Required(ErrorMessage = "El Grado es obligatorio")]
        public int Grado { get; set; }

        public Rol? Rol { get; set; }

        public ICollection<ClasesEstudiantes>? ClasesEstudiantes { get; set; }
    }
}
