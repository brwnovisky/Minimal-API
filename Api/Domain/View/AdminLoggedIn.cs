namespace MinimalApi.Domain.View;

public record AdminLoggedIn
{
    public string Email { get;set; } = default!;
    public string Profile { get;set; } = default!;
    public string Token { get;set; } = default!;
}