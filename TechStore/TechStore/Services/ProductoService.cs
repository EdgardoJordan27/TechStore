using Microsoft.EntityFrameworkCore;
using TechStore.Data;
using TechStore.Models;
using TechStore.Services.Interfaces;

namespace TechStore.Services
{
    // Única clase que toca el DbContext para los productos. Los controladores
    // trabajan contra IProductoService y no conocen Entity Framework.
    public class ProductoService : IProductoService
    {
        private readonly TechStoreDbContext _contexto;

        public ProductoService(TechStoreDbContext contexto)
        {
            _contexto = contexto;
        }

        public async Task<List<Producto>> ObtenerTodosAsync()
        {
            return await _contexto.Productos
                .AsNoTracking()
                .Include(p => p.Categoria)
                .OrderByDescending(p => p.Id)
                .ToListAsync();
        }

        public async Task<List<Producto>> ObtenerDestacadosAsync(int cantidad)
        {
            return await _contexto.Productos
                .AsNoTracking()
                .Include(p => p.Categoria)
                .Where(p => p.Estado == EstadosProducto.Activo)
                .OrderByDescending(p => p.Stock)
                .Take(cantidad)
                .ToListAsync();
        }

        public async Task<Producto?> ObtenerPorIdAsync(int id)
        {
            return await _contexto.Productos
                .AsNoTracking()
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AgregarAsync(Producto producto)
        {
            _contexto.Productos.Add(producto);
            await _contexto.SaveChangesAsync();
        }

        public async Task<bool> EditarAsync(Producto producto)
        {
            var existente = await _contexto.Productos.FindAsync(producto.Id);
            if (existente is null)
            {
                return false;
            }

            existente.Nombre = producto.Nombre;
            existente.Descripcion = producto.Descripcion;
            existente.Precio = producto.Precio;
            existente.CategoriaId = producto.CategoriaId;
            existente.Stock = producto.Stock;
            existente.Estado = producto.Estado;
            existente.Imagen = producto.Imagen;

            await _contexto.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var existente = await _contexto.Productos.FindAsync(id);
            if (existente is null)
            {
                return false;
            }

            _contexto.Productos.Remove(existente);
            await _contexto.SaveChangesAsync();
            return true;
        }
    }
}
