using ProyectoColegio.Domain;
namespace ProyectColegio.Test
{
    public class CursoTest
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void CursoValidation()
        {
            // Crear una instancia de Category con valores válidos
            
            Cursos nueva = new Cursos();
            nueva.CursoID = 1;
            nueva.nombre = "Ciencias Naturales";
            nueva.descripcion = "Curso para el tercer grado";
            nueva.imagen = ".../ruta/imagen";


            // Validar la instancia de Category
            bool val = Validador(nueva);

            // Verificar que la validación sea verdadera
            Assert.True(val);
        }

        public bool Validador(Cursos p)
        {
            if (p.CursoID != null
                && p.nombre != null
                && p.descripcion != null
                && p.imagen!=null)
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