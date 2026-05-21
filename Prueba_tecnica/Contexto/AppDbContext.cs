using Prueba_tecnica.Entidades;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Prueba_tecnica.Contexto
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<Ubicacion> Ubicaciones { get; set; }
        public DbSet<Recepcion> Recepciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Seed de datos iniciales
            modelBuilder.Entity<Producto>().HasData(
                new Producto { Id = 1, SKU = "PROD-ASUS", Nombre = "Notebook ASUS" },
                new Producto { Id = 2, SKU = "PROD-LOGI", Nombre = "Teclado Logitech" }
            );

            modelBuilder.Entity<Ubicacion>().HasData(
                new Ubicacion { Id = 1, CodigoUbicacion = "RACK-A", CapacidadMaxima = 100, OcupacionActual = 90 },
                new Ubicacion { Id = 2, CodigoUbicacion = "RACK-B", CapacidadMaxima = 50, OcupacionActual = 10 }
            );
        }
    }
}
