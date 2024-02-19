using System.ComponentModel.DataAnnotations;

namespace ProyectoColegio.Domain
{
    public class ClasesEstudiantes
    {
        [Key]
        public int ClasesEstudiantesID { get; set; }
        public int ClasesID { get; set; }
        public int UsuarioID { get; set; }
        [Range(0, 20)]
        public float C1 { get; set; }
        [Range(0, 20)]
        public float C2 { get; set; }
        [Range(0, 20)]
        public float C3 { get; set; }
        [Range(0, 20)]
        public float C4 { get; set; }
        [Range(0, 20)]
        public float Final { get; set; }
        public string TipoGrado { get; set; } // Primaria o Secundaria
        public int Grado { get; set; } 

        public Clases? Clases { get; set; }
        public Usuario? Usuario { get; set; }
    }
}
