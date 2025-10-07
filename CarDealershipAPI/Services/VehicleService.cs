using CarDealershipAPI.Data;
using CarDealershipAPI.Models;
using Microsoft.EntityFrameworkCore;
namespace CarDealershipAPI.Services;
public class VehicleService
{
    private readonly AppDbContext _db;
    public VehicleService(AppDbContext db)
    {
        _db = db;
    }
    public IQueryable<Vehicle> Query() => _db.Vehicles.AsQueryable();
    public async Task<Vehicle?> GetByIdAsync(Guid id) => await
    _db.Vehicles.FindAsync(id);
    public async Task AddAsync(Vehicle v)
    {
        _db.Vehicles.Add(v);
        await _db.SaveChangesAsync();
    }
public async Task UpdateAsync(Vehicle v)
    {
        _db.Vehicles.Update(v);
        await _db.SaveChangesAsync();
    }
}