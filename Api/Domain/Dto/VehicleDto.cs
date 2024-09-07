namespace MinimalApi.Domain.Dto;

public record VehicleDto
{
    public string Name { get;set; } = default!;
    public string Brand { get;set; } = default!;
    public int Year { get;set; } = default!;
}