using System.ComponentModel.DataAnnotations;

namespace ProyectoColegio.Domain
{
    public class Docente
    {
        [Key]
        public int DocenteID { get; set; }
        [Required(ErrorMessage = "El DNI es obligatorio")]
        [MaxLength(15, ErrorMessage = "El DNI debe tener como máximo 15 digitos"), MinLength(8)]
        public string Dni { get; set; }
        [Required(ErrorMessage = "El Nombre es obligatorio")]
        [MaxLength(100, ErrorMessage = "El Nombre debe tener como máximo 100 digitos")]
        public string Nombres { get; set; }
        [Required(ErrorMessage = "El Apellido es obligatorio")]
        [MaxLength(100, ErrorMessage = "El Apellido debe tener como máximo 100 digitos")]
        public string Apellidos { get; set; }
        public int? IdLogin { get; set; }

        public ICollection<Clases>? Sesiones { get; set; }

        // Cambios realizados para hacer la colección de solo lectura
        /*private readonly List<Clases> _sesiones = new List<Clases>();
        public IReadOnlyCollection<Clases> Sesiones => _sesiones.AsReadOnly();*/

        
    }

}
