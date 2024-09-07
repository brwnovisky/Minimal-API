using MinimalApi.Domain.Dto;
using MinimalApi.Domain.Entity;
using MinimalApi.Domain.Interface;
using MinimalApi.Infrastructure.Db;

namespace MinimalApi.Domain.Service;

public class AdministratorService(Context context) : IAdministratorService
{
    public Administrator? Login(LoginDto loginDto)
    {
        return context.Administrators.FirstOrDefault
        (
            a => a.Email == loginDto.Email &&
            a.Password == loginDto.Password
        );
    }

    public Administrator Add(Administrator administrator)
    {
        context.Administrators.Add(administrator);
        context.SaveChanges();
        
        return administrator;
    }

    public Administrator? GetById(int id)
    {
        return context.Administrators.Find(id);
    }

    public List<Administrator> GetAll(int? page)
    {
        var query = context.Administrators.AsQueryable();
        
        if (page != null) 
            query = query.Skip((page.Value - 1) * 10).Take(10);
        
        return query.ToList();
    }
}