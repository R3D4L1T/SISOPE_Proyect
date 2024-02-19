using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoColegio.Repositories.Interfaces
{
    public interface IRepositoryBase<T> where T : class
    {
        T Get(int id); //Obtener un elemento de la entidad T
        IEnumerable<T> GetAll(); //Recuperar todos los elementos
        T GetFirstOrDefault(int id); //Para obtener el primer elemento
        void add (T entity); //Agregar un registro
        void remove (int i); //Eliminar un registro
        //void remove (T entity); //Eliminar un registro
        
    }
}
