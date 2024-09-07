using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinimalApi.Domain.Entity;

public class Administrator
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get;set; }

    [Required]
    [StringLength(255)]
    public string? Email { get;set; }

    [Required]
    [StringLength(50)]
    public string? Password { get;set; }

    [Required]
    [StringLength(10)]
    public string? Profile { get;set; }
}