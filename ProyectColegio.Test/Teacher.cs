using ProyectoColegio.Domain;
namespace ProyectColegio.Test
{
    public class Teacher
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void teacherValidation()
        {
            // Crear una instancia de Category con valores válidos

            Docente nuevo = new Docente();
            nuevo.DocenteID = 1;
            nuevo.Dni = "75134204";
            nuevo.Nombres = "Antoni";
            nuevo.Apellidos = "Rodriges";
            nuevo.IdLogin = 2;


            // Validar la instancia de Category
            bool val = Validador(nuevo);

            // Verificar que la validación sea verdadera
            Assert.True(val);
        }

        // Método de validación para la clase Category
        public bool Validador(Docente p)
        {
            if (p.DocenteID != null
                && p.Dni != null
                && p.Nombres != null
                && p.Apellidos != null
                && p.IdLogin != null)
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