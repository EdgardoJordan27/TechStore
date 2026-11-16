using Microsoft.EntityFrameworkCore;
using TechStore.Data;
using TechStore.Models;
using TechStore.Services.Interfaces;

namespace TechStore.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly TechStoreDbContext _contexto;

        public CategoriaService(TechStoreDbContext contexto)
        {
            _contexto = contexto;
        }

        public async Task<List<Categoria>> ObtenerTodasAsync()
        {
            return await _contexto.Categorias
                .AsNoTracking()
                .Include(c => c.Productos)
                .OrderBy(c => c.Nombre)
                .ToListAsync();
        }

        public async Task<Categoria?> ObtenerPorIdAsync(int id)
        {
            return await _contexto.Categorias
                .AsNoTracking()
                .Include(c => c.Productos)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AgregarAsync(Categoria categoria)
        {
            _contexto.Categorias.Add(categoria);
            await _contexto.SaveChangesAsync();
        }

        public async Task<bool> EditarAsync(Categoria categoria)
        {
            var existente = await _contexto.Categorias.FindAsync(categoria.Id);
            if (existente is null)
            {
                return false;
            }

            existente.Nombre = categoria.Nombre;
            existente.Descripcion = categoria.Descripcion;
            existente.Icono = categoria.Icono;

            await _contexto.SaveChangesAsync();
            return true;
        }

        public async Task<ResultadoEliminacion> EliminarAsync(int id)
        {
            var existente = await _contexto.Categorias.FindAsync(id);
            if (existente is null)
            {
                return ResultadoEliminacion.NoEncontrada;
            }

            var tieneProductos = await _contexto.Productos.AnyAsync(p => p.CategoriaId == id);
            if (tieneProductos)
            {
                return ResultadoEliminacion.TieneProductos;
            }

            _contexto.Categorias.Remove(existente);
            await _contexto.SaveChangesAsync();
            return ResultadoEliminacion.Eliminada;
        }
    }
}
