using ProyectoColegio.Domain;
namespace ProyectColegio.Test
{
    public class User
    {
        [SetUp]
        public void Setup()
        {

        }
        [Test]
        public void UserValidation()
        {
            // Crear una instancia de Category con valores válidos
            Usuario nuevo = new Usuario();
            nuevo.UsuarioID = 1;
            nuevo.RoleID = 1;
            nuevo.Dni = "75124204";
            nuevo.Nombres = "franco";
            nuevo.Apellidos = "Cerna";
            nuevo.Correo = "franco@colegio.com";
            nuevo.Password = "75134204";
            nuevo.Grado = 1;
            // Validar la instancia de Category
            bool val = Validador(nuevo);

            // Verificar que la validación sea verdadera
            Assert.True(val);
        }
        public bool Validador(Usuario u)
        {
            if (u.UsuarioID != null
                && u.RoleID != null
                && u.Dni != null
                && u.Nombres!=null
                && u.Apellidos !=null
                && u.Correo !=null
                && u.Password !=null
                && u.Grado !=null)
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