using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ViNgocHiep_2123110365.Models;

public class Favorite
{
    public int UserId { get; set; }
    public int BookId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Navigation Properties
    [ForeignKey("UserId")]
    public User? User { get; set; }

    [ForeignKey("BookId")]
    public Book? Book { get; set; }
}
