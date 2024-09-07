using MinimalApi.Domain.Dto;
using MinimalApi.Domain.Entity;

namespace MinimalApi.Domain.Interface;

public interface IAdministratorService
{
    Administrator? Login(LoginDto loginDto);
    Administrator Add(Administrator administrator);
    Administrator? GetById(int id);
    List<Administrator> GetAll(int? page);
}