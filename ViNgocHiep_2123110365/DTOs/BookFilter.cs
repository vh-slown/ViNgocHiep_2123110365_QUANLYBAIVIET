using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using ViNgocHiep_2123110365.Helpers;

namespace ViNgocHiep_2123110365.DTOs
{
    public class PublicBookFilter : PaginationFilter
    {
        public int? CategoryId { get; set; }
    }

    public class MyBookFilter : PaginationFilter
    {
        public byte? Status { get; set; }
    }

    public class AdminBookFilter : PaginationFilter
    {
        public int? CategoryId { get; set; }
        public byte? Status { get; set; }
        public bool? IsDeleted { get; set; }
    }

    public class CreateUpdateBookDTO
    {
        [Required]
        [StringLength(255)]
        public string Title { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Slug { get; set; }

        [StringLength(500)]
        public string? Summary { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        [Required]
        public int CategoryId { get; set; }

        public IFormFile? ThumbnailFile { get; set; }
    }
}
