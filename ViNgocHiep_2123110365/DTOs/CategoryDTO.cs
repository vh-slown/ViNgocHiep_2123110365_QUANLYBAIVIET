using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using ViNgocHiep_2123110365.Helpers;

namespace ViNgocHiep_2123110365.DTOs
{
    public class PublicCategoryFilter : PaginationFilter { }

    public class AdminCategoryFilter : PaginationFilter
    {
        public bool? IsDeleted { get; set; }
    }

    public class CategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Image { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class CreateUpdateCategoryDTO
    {
        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public string? Slug { get; set; }

        public string? Description { get; set; }

        public IFormFile? ImageFile { get; set; }
    }
}
