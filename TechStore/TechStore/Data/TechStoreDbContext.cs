using Microsoft.EntityFrameworkCore;
using TechStore.Models;

namespace TechStore.Data
{
    public class TechStoreDbContext : DbContext
    {
        public TechStoreDbContext(DbContextOptions<TechStoreDbContext> opciones)
            : base(opciones)
        {
        }

        public DbSet<Producto> Productos { get; set; } = null!;
        public DbSet<Categoria> Categorias { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Categoria>(entidad =>
            {
                entidad.ToTable("Categorias");
                entidad.HasKey(c => c.Id);
                entidad.Property(c => c.Nombre).IsRequired().HasMaxLength(80);
                entidad.Property(c => c.Descripcion).IsRequired().HasMaxLength(300);
                entidad.Property(c => c.Icono).IsRequired().HasMaxLength(40);
            });

            modelBuilder.Entity<Producto>(entidad =>
            {
                entidad.ToTable("Productos");
                entidad.HasKey(p => p.Id);
                entidad.Property(p => p.Nombre).IsRequired().HasMaxLength(120);
                entidad.Property(p => p.Descripcion).IsRequired().HasMaxLength(400);
                entidad.Property(p => p.Precio).HasPrecision(18, 2);
                entidad.Property(p => p.Estado).IsRequired().HasMaxLength(20);
                entidad.Property(p => p.Imagen).HasMaxLength(200);

                // Una categoría tiene muchos productos; un producto pertenece a una categoría.
                // Restrict evita borrar una categoría que todavía tiene productos asociados.
                entidad.HasOne(p => p.Categoria)
                       .WithMany(c => c.Productos)
                       .HasForeignKey(p => p.CategoriaId)
                       .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Categoria>().HasData(DatosIniciales.ObtenerCategorias());
            modelBuilder.Entity<Producto>().HasData(DatosIniciales.ObtenerProductos());
        }
    }
}
