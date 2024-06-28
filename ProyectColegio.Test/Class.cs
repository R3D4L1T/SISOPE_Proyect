using ProyectoColegio.Domain;
namespace ProyectColegio.Test
{
    public class Class
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void ClaseValidation()
        {
            // Crear una instancia de Category con valores válidos
            
            Clases nueva = new Clases();
            nueva.ClaseID = 1;
            nueva.DocenteID = 1;
            nueva.CursoID = 1;
            nueva.DiaClase = "Lunes";
            nueva.HoraInicio = TimeSpan.Parse("08:00:00");
            nueva.HoraFin = TimeSpan.Parse("10:00:00");
            nueva.TipoGrado = "Primaria";
            nueva.Grado = 1;


            // Validar la instancia de Category
            bool val = Validador(nueva);

            // Verificar que la validación sea verdadera
            Assert.True(val);
        }

        public bool Validador(Clases c)
        {
            if (c.ClaseID != null
                && c.DocenteID != null
                && c.CursoID != null
                && c.DiaClase!=null
                && c.HoraInicio!=null
                && c.HoraFin!=null
                && c.TipoGrado!=null
                && c.Grado!=null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}