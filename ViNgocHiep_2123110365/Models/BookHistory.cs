using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ViNgocHiep_2123110365.Models
{
    public class BookHistory
    {
        [Key]
        public int Id { get; set; }
        public int BookId { get; set; }
        public string OldContent { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int EditedByUserId { get; set; }

        [ForeignKey("BookId")]
        public Book? Book { get; set; }
    }
}
