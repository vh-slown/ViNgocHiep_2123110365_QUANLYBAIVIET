using System.ComponentModel.DataAnnotations;

namespace ViNgocHiep_2123110365.Models;

public class Category
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public ICollection<Book>? Books { get; set; }
}
