using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViNgocHiep_2123110365.Data;
using ViNgocHiep_2123110365.DTOs;
using ViNgocHiep_2123110365.Helpers;

namespace ViNgocHiep_2123110365.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<PagedResponse<IEnumerable<CategoryDTO>>>> GetCategories(
            [FromQuery] PublicCategoryFilter filter
        )
        {
            var query = _context.Categories.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.SearchQuery))
            {
                query = query.Where(c =>
                    c.Name.ToLower().Contains(filter.SearchQuery.ToLower().Trim())
                );
            }

            var totalRecords = await query.CountAsync();
            var pagedData = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(c => new CategoryDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    Slug = c.Slug,
                    Image = c.Image,
                    Description = c.Description,
                    CreatedAt = c.CreatedAt,
                })
                .ToListAsync();

            return Ok(
                new PagedResponse<IEnumerable<CategoryDTO>>(
                    pagedData,
                    filter.PageNumber,
                    filter.PageSize,
                    totalRecords
                )
            );
        }

        // GET: api/Categories/{slug}
        [HttpGet("{slug}")]
        public async Task<ActionResult<CategoryDTO>> GetCategory(string slug)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Slug == slug);
            if (category == null)
                return NotFound(new { message = "Không tìm thấy danh mục." });

            return Ok(
                new CategoryDTO
                {
                    Id = category.Id,
                    Name = category.Name,
                    Slug = category.Slug,
                    Image = category.Image,
                    Description = category.Description,
                    CreatedAt = category.CreatedAt,
                }
            );
        }
    }
}
