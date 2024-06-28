using ProyectoColegio.Domain;
namespace ProyectColegio.Test
{
    public class RolTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void RolValidation()
        {
            // Crear una instancia de Category con valores válidos
            
            Rol nueva = new Rol();
            nueva.RoleID = 1;
            nueva.Description = "Nombre Producto";
            // Validar la instancia de Category
            bool val = Validador(nueva);

            // Verificar que la validación sea verdadera
            Assert.True(val);
        }
        public bool Validador(Rol p)
        {
            if (p.RoleID != null
                && p.Description != null)
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