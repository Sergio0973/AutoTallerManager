using Application.Abstractions;
using Domain.Entities;
using Domain.ValueObjects.Clientes;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class ClienteRepository : IClienteRepository
{
    private readonly AutoTallerDbContext _context;

    public ClienteRepository(AutoTallerDbContext context)
    {
        _context = context;
    }

    public async Task<Cliente?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Clientes
            .Include(c => c.Vehiculos)
            .Include(c => c.Telefonos)
            .Include(c => c.Correos)
            .Include(c => c.Direcciones)
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public async Task<Cliente?> GetByDocumentoAsync(Documento documento, CancellationToken ct = default)
    {
        return await _context.Clientes
            .FirstOrDefaultAsync(c => c.DocumentoIdentidad == documento, ct);
    }

    public async Task<IReadOnlyList<Cliente>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Clientes.ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Cliente>> GetPagedAsync(int pageNumber, int pageSize, string? search = null, CancellationToken ct = default)
    {
        var query = _context.Clientes.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchTerm = search.ToLower();
            query = query.Where(c => c.NombreCompleto.Nombres.ToLower().Contains(searchTerm) || 
                                     c.NombreCompleto.Apellidos.ToLower().Contains(searchTerm));
        }

        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(string? search = null, CancellationToken ct = default)
    {
        var query = _context.Clientes.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchTerm = search.ToLower();
            query = query.Where(c => c.NombreCompleto.Nombres.ToLower().Contains(searchTerm) || 
                                     c.NombreCompleto.Apellidos.ToLower().Contains(searchTerm));
        }

        return await query.CountAsync(ct);
    }

    public async Task AddAsync(Cliente cliente, CancellationToken ct = default)
    {
        await _context.Clientes.AddAsync(cliente, ct);
    }

    public Task UpdateAsync(Cliente cliente, CancellationToken ct = default)
    {
        _context.Clientes.Update(cliente);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(Cliente cliente, CancellationToken ct = default)
    {
        _context.Clientes.Remove(cliente);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsDocumentoAsync(Documento documento, CancellationToken ct = default)
    {
        return await _context.Clientes.AnyAsync(c => c.DocumentoIdentidad == documento, ct);
    }
}
