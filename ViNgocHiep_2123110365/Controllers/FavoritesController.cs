using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViNgocHiep_2123110365.Data;
using ViNgocHiep_2123110365.DTOs;
using ViNgocHiep_2123110365.Models;

namespace ViNgocHiep_2123110365.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FavoritesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FavoritesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Favorites/User/{id}
        [HttpGet("User/{userId}")]
        public async Task<ActionResult<IEnumerable<BookListResponseDTO>>> GetUserFavorites(
            int userId
        )
        {
            var favoriteBooks = await _context
                .Favorites.Where(f => f.UserId == userId)
                .Include(f => f.Book)
                .ThenInclude(b => b.Category)
                .Include(f => f.Book)
                .ThenInclude(b => b.User)
                .OrderByDescending(f => f.SavedAt)
                .Select(f => new BookListResponseDTO
                {
                    Id = f.Book!.Id,
                    Title = f.Book.Title,
                    Slug = f.Book.Slug,
                    Thumbnail = f.Book.Thumbnail,
                    Summary = f.Book.Summary,
                    ViewCount = f.Book.ViewCount,
                    Status = f.Book.Status,
                    CreatedAt = f.Book.CreatedAt,
                    Category = new CategoryDTO
                    {
                        Id = f.Book.Category!.Id,
                        Name = f.Book.Category.Name,
                        Slug = f.Book.Category.Slug,
                    },
                    User = new UserDTO
                    {
                        Id = f.Book.User!.Id,
                        FullName = f.Book.User.FullName,
                        Username = f.Book.User.Username,
                        Avatar = f.Book.User.Avatar,
                    },
                })
                .ToListAsync();

            return Ok(favoriteBooks);
        }

        // POST: api/Favorites
        [HttpPost]
        public async Task<IActionResult> PostFavorite(Favorite favorite)
        {
            var exists = await _context.Favorites.AnyAsync(f =>
                f.UserId == favorite.UserId && f.BookId == favorite.BookId
            );

            if (exists)
            {
                return BadRequest(new { message = "Bạn đã yêu thích bài viết này rồi!" });
            }

            favorite.SavedAt = DateTime.Now;

            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã thêm vào danh sách yêu thích" });
        }

        // DELETE: api/Favorites/Book/1/User/1
        [HttpDelete("Book/{bookId}/User/{userId}")]
        public async Task<IActionResult> DeleteFavorite(int bookId, int userId)
        {
            var favorite = await _context.Favorites.FirstOrDefaultAsync(f =>
                f.BookId == bookId && f.UserId == userId
            );

            if (favorite == null)
            {
                return NotFound(new { message = "Chưa yêu thích bài viết này" });
            }

            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã bỏ yêu thích" });
        }
    }
}
