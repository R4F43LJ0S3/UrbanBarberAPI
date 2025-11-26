using Microsoft.EntityFrameworkCore;
using UrbanBarberAPI.Models;

namespace UrbanBarberAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Barbero> Barberos { get; set; }
        public DbSet<Servicio> Servicios { get; set; }
        public DbSet<Cita> Citas { get; set; }
        public DbSet<Pago> Pagos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Username).IsUnique();
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Correo).IsUnique();
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Celular).IsUnique();

            modelBuilder.Entity<Cita>()
                .HasOne(c => c.Usuario)
                .WithMany(u => u.Citas)
                .HasForeignKey(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cita>()
                .HasOne(c => c.Barbero)
                .WithMany(b => b.Citas)
                .HasForeignKey(c => c.BarberoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cita>()
                .HasOne(c => c.Servicio)
                .WithMany(s => s.Citas)
                .HasForeignKey(c => c.ServicioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pago>()
                .HasOne(p => p.Cita)
                .WithOne(c => c.Pago)
                .HasForeignKey<Pago>(p => p.CitaId)
                .OnDelete(DeleteBehavior.Cascade);

            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Barbero>().HasData(
                new Barbero { Id = 1, Nombre = "Ricardo 'El Clásico'", Especialidad = "Cortes Tradicionales", Experiencia = "10 años", Rating = 4.9m },
                new Barbero { Id = 2, Nombre = "Rafael 'El Diseñador'", Especialidad = "Diseños y Fade Modernos", Experiencia = "8 años", Rating = 4.8m },
                new Barbero { Id = 3, Nombre = "Juan 'El Lápiz'", Especialidad = "Afeitado con Navaja y Patillas", Experiencia = "12 años", Rating = 5.0m }
            );

            modelBuilder.Entity<Servicio>().HasData(
                new Servicio { Id = 1, Nombre = "Corte Sencillo", Descripcion = "Corte clásico y limpio", Duracion = 45, Precio = 25000 },
                new Servicio { Id = 2, Nombre = "Corte + Cejas", Descripcion = "Corte preciso y diseño de cejas", Duracion = 50, Precio = 25000 },
                new Servicio { Id = 3, Nombre = "Tratamiento Corte Premium (Cejas + Perfilado de Barba)", Descripcion = "Servicio completo", Duracion = 60, Precio = 40000 },
                new Servicio { Id = 4, Nombre = "Perfilado de Barba", Descripcion = "Definición exacta", Duracion = 20, Precio = 15000 },
                new Servicio { Id = 5, Nombre = "Corte + Tinturado de Cabello", Descripcion = "Corte moderno con color", Duracion = 90, Precio = 35000 },
                new Servicio { Id = 6, Nombre = "Corte + Mascarilla", Descripcion = "Corte con mascarilla facial", Duracion = 60, Precio = 35000 }
            );

            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    Id = 1,
                    Username = "admin",
                    Nombre = "Administrador",
                    Apellido = "Sistema",
                    Correo = "admin@urbanbarber.com",
                    Celular = "3001234567",
                    PasswordHash = "$2a$11$rG8pZ4YX3VkL5qKz.mXlvuJXZxvYvN8WqH5L1fZQ2xGKmEYvXqxCi",
                    Rol = "admin"
                }
            );
        }
    }
}
