using System.ComponentModel.DataAnnotations;

namespace ViNgocHiep_2123110365.Models;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string Password { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Avatar { get; set; }

    [StringLength(1000)]
    public string? Bio { get; set; }

    [StringLength(20)]
    public string Role { get; set; } = "member";

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Navigation Properties
    public ICollection<Book>? Books { get; set; }
    public ICollection<Comment>? Comments { get; set; }
    public ICollection<Favorite>? Favorites { get; set; }
}
