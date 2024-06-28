
using ProyectoColegio.Domain;
namespace ProyectColegio.Test
{
    public class SeccionTest
    {
        [SetUp]
        public void Setup()
        {
        }
        [Test]
        public void seccionValidation()
        {
            // Crear una instancia de Category con valores válidos

            Seccion nueva = new Seccion();
            nueva.Nombres = "Section A";
            nueva.Correo = "jhoel@colegio.com";
            nueva.Grado = 3;
            nueva.TipoGrado = "secundaria";
            // Validar la instancia de Category
            bool val = Validador(nueva);

            // Verificar que la validación sea verdadera
            Assert.True(val);
        }

        public bool Validador(Seccion p)
        {
            if (p.Nombres != null
                && p.Correo != null
                && p.Grado!=null
                && p.TipoGrado!=null)
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