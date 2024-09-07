namespace MinimalApi.Domain.View;

public record AdminLogin
{
    public int Id { get;set; } = default!;
    public string? Email { get;set; } = default!;
    public string? Profile { get;set; } = default!;
}