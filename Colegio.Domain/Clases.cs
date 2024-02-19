using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProyectoColegio.Domain
{
    public class Clases
    {
        [Key]
        public int ClaseID { get; set; }

        [Required(ErrorMessage = "El instructor es requerido")]
        public int DocenteID { get; set; }

        [Required(ErrorMessage = "El curso es requerido")]
        public int CursoID { get; set; }

        // Cambios realizados
        [Required(ErrorMessage = "El día de la clase es requerido")]
        public string DiaClase { get; set; }

        [Required(ErrorMessage = "La hora de inicio es requerida")]
        public TimeSpan HoraInicio { get; set; }

        [Required(ErrorMessage = "La hora de fin es requerida")]
        public TimeSpan HoraFin { get; set; }

        [Required(ErrorMessage = "El Tipo de Grado es obligatorio")]
        public string TipoGrado { get; set; } // Primaria o Secundaria

        [Required(ErrorMessage = "El Grado es obligatorio")]
        public int Grado { get; set; }
        public Docente? docente { get; set; }
        public Cursos? curso { get; set; }

        public ICollection<ClasesEstudiantes>? ClasesEstudiantes { get; set; }
    }
}
