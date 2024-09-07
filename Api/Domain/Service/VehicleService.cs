using MinimalApi.Domain.Entity;
using MinimalApi.Domain.Interface;
using MinimalApi.Infrastructure.Db;

namespace MinimalApi.Domain.Service;

public class VehicleService(Context context) : IVehicleService
{
    public List<Vehicle> GetAll(int? page = 1, string? name = null)
    {
        IQueryable<Vehicle> query = context.Vehicles.AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(name))
        {
            query = context.Vehicles.Where
            (
                x => x.Name.ToLower().Contains(name.ToLower())
            );
        }

        if (page!= null) 
            return query.Skip((page.Value - 1) * 10).Take(10).ToList();
        
        return query.ToList();
    }

    public Vehicle? GetById(int id)
    {
        return context.Vehicles.Find(id);
    }

    public void Add(Vehicle vehicle)
    {
        context.Vehicles.Add(vehicle);
        context.SaveChanges();
    }

    public void Update(Vehicle vehicle)
    {
        context.Vehicles.Update(vehicle);
        context.SaveChanges();
    }

    public void Delete(Vehicle vehicle)
    {
        context.Vehicles.Remove(vehicle);
        context.SaveChanges();
    }
}