using MinimalApi.Domain.Entity;

namespace MinimalApi.Domain.Interface;

public interface IVehicleService
{
    List<Vehicle> GetAll(int? page = 1, string? name = null);
    
    Vehicle? GetById(int id);
    
    void Add(Vehicle vehicle);
    
    void Update(Vehicle vehicle);
    
    void Delete(Vehicle vehicle);
}