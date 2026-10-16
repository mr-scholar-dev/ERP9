using ERP.Domain.Entities;
using ERP.Infraestructure.Data;
using ERP.Infraestructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infraestructure.Implementation;

public class WarehouseRepository : IWarehouseRepository
{
    private readonly AppDbContext _context;

    public WarehouseRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Warehouse>> GetAllAsync()
    {
        return await _context.Warehouse
            .AsNoTracking()
            .OrderBy(w => w.IdWarehouse)
            .ToListAsync();
    }

    public async Task<IEnumerable<Warehouse>> GetPaginationAsync(int pageNumber, int pageSize)
    {
        return await _context.Warehouse
            .AsNoTracking()
            .OrderBy(w => w.IdWarehouse)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.Warehouse.CountAsync();
    }

    public async Task<Warehouse?> GetByIdAsync(int id)
    {
        return await _context.Warehouse
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.IdWarehouse == id);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Warehouse.AnyAsync(w => w.IdWarehouse == id);
    }

    public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
    {
        var normalizedName = name.Trim();
        return await _context.Warehouse.AnyAsync(w =>
            w.WarehouseName == normalizedName &&
            (!excludeId.HasValue || w.IdWarehouse != excludeId.Value));
    }

    public async Task<bool> HasProductsAsync(int id)
    {
        return await _context.Product.AnyAsync(p => p.IdWarehouse == id);
    }

    public async Task AddAsync(Warehouse warehouse)
    {
        // IdWarehouse es ValueGeneratedNever: se asigna el siguiente Id disponible.
        var lastId = await _context.Warehouse.MaxAsync(w => (int?)w.IdWarehouse) ?? 0;
        warehouse.IdWarehouse = lastId + 1;

        _context.Warehouse.Add(warehouse);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Warehouse warehouse)
    {
        _context.Warehouse.Update(warehouse);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await _context.Warehouse
            .Where(w => w.IdWarehouse == id)
            .ExecuteDeleteAsync();
    }
}
