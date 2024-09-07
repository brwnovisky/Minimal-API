using MinimalApi.Domain.Dto;
using MinimalApi.Domain.Entity;
using MinimalApi.Domain.Interface;

namespace Test.Mock;

public class AdministratorServiceMock : IAdministratorService
{   
    private static readonly List<Administrator> Administrators =
    [
        new()
        {
            Id = 1,
            Email = "adm@teste.com",
            Password = "123456",
            Profile = "Adm"
        },

        new()
        {
            Id = 2,
            Email = "editor@teste.com",
            Password = "123456",
            Profile = "Editor"
        }
    ];

    public Administrator? GetById(int id)
    {
        return Administrators.Find(a => a.Id == id);
    }

    public List<Administrator> GetAll(int? page)
    {
        return Administrators;
    }

    public Administrator? Login(LoginDto loginDto)
    {
        return Administrators.Find
        (
            a => 
                a.Email == loginDto.Email && 
                a.Password == loginDto.Password
        );
    }

    public Administrator Add(Administrator administrator)
    {
        administrator.Id = Administrators.Count + 1;
        Administrators.Add(administrator);

        return administrator;
    }
}