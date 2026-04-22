using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViNgocHiep_2123110365.Data;
using ViNgocHiep_2123110365.DTOs;
using ViNgocHiep_2123110365.Helpers;
using ViNgocHiep_2123110365.Models;

namespace ViNgocHiep_2123110365.Controllers
{
    [Route("api/admin/categories")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class AdminCategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminCategoriesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/admin/categories
        [HttpGet]
        public async Task<ActionResult<PagedResponse<IEnumerable<CategoryDTO>>>> GetCategories(
            [FromQuery] AdminCategoryFilter filter
        )
        {
            var query = _context.Categories.IgnoreQueryFilters().AsQueryable();

            if (filter.IsDeleted.HasValue)
                query = query.Where(c => c.IsDeleted == filter.IsDeleted.Value);
            if (!string.IsNullOrWhiteSpace(filter.SearchQuery))
                query = query.Where(c =>
                    c.Name.ToLower().Contains(filter.SearchQuery.ToLower().Trim())
                );

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
                    IsDeleted = c.IsDeleted,
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

        // GET: api/admin/categories/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDTO>> GetCategory(int id)
        {
            var category = await _context
                .Categories.IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.Id == id);
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
                    IsDeleted = category.IsDeleted,
                }
            );
        }

        // POST: api/admin/categories
        [HttpPost]
        public async Task<IActionResult> PostCategory([FromForm] CreateUpdateCategoryDTO request)
        {
            var category = new Category
            {
                Name = request.Name,
                Slug = StringHelper.GenerateSlug(request.Name),
                Description = request.Description,
                Image = await FileHelper.UploadFileAsync(request.ImageFile, "categories"),
                CreatedAt = DateTime.Now,
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Thêm danh mục thành công." });
        }

        // PUT: api/admin/categories/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(
            int id,
            [FromForm] CreateUpdateCategoryDTO request
        )
        {
            var oldCategory = await _context
                .Categories.IgnoreQueryFilters()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
            if (oldCategory == null)
                return NotFound(new { message = "Không tìm thấy danh mục." });

            var updatedCategory = new Category
            {
                Id = id,
                Name = request.Name,
                Slug = StringHelper.GenerateSlug(request.Name),
                Description = request.Description,
                Image =
                    request.ImageFile != null
                        ? await FileHelper.UploadFileAsync(request.ImageFile, "categories")
                        : oldCategory.Image,
                CreatedAt = oldCategory.CreatedAt,
                UpdatedAt = DateTime.Now,
                IsDeleted = oldCategory.IsDeleted,
            };

            _context.Entry(updatedCategory).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Cập nhật danh mục thành công." });
        }

        // DELETE: api/admin/categories/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context
                .Categories.IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
                return NotFound(new { message = "Không tìm thấy danh mục." });

            var activeBooksCount = await _context.Books.CountAsync(b =>
                b.CategoryId == id && !b.IsDeleted
            );

            if (activeBooksCount > 0)
            {
                return BadRequest(
                    new
                    {
                        message = $"KHÔNG THỂ XÓA! Danh mục này đang chứa {activeBooksCount} bài viết. Vui lòng chuyển hoặc xóa các bài viết này trước.",
                    }
                );
            }

            category.IsDeleted = true;
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Đã khóa danh mục thành công." });
        }

        // PUT: api/admin/categories/{id}/restore
        [HttpPut("{id}/restore")]
        public async Task<IActionResult> RestoreCategory(int id)
        {
            var category = await _context
                .Categories.IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
                return NotFound();

            category.IsDeleted = false;
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Đã khôi phục danh mục." });
        }
    }
}
