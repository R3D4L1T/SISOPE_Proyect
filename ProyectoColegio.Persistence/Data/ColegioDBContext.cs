using Microsoft.EntityFrameworkCore;
using ProyectoColegio.Domain;

namespace ProyectoColegio.Persistence.Data
{
    public class ColegioDBContext: DbContext
    {
        public ColegioDBContext(DbContextOptions<ColegioDBContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Clases>()
                .HasOne(c => c.docente)
                .WithMany(d => d.Sesiones)
                .HasForeignKey(c => c.DocenteID);

            modelBuilder.Entity<Rol>().HasData(
                new Rol { RoleID = 1, Description = "Administrador" },
                new Rol { RoleID = 2, Description = "Docente" },
                new Rol { RoleID = 3, Description = "Estudiante" }
            );

        }

        public DbSet<Cursos> Cursos { get; set; }
        public DbSet<Docente> Docente { get; set; }
        public DbSet<Clases> Clases { get; set; }
        public DbSet<Rol> Rol { get; set; }
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<ClasesEstudiantes> ClasesEstudiantes { get; set; }
    }
}
