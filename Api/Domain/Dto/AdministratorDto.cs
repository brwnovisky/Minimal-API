using MinimalApi.Domain.Enum;

namespace MinimalApi.Domain.Dto;

public class AdministratorDto
{
    public string Email { get;set; } = default!;
    public string Password { get;set; } = default!;
    public Profile? Profile { get;set; } = default!;
}