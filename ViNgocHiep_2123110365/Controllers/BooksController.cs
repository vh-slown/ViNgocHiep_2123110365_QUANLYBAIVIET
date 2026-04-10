using System.Security.Claims;
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
    public class BooksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BooksController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookListResponseDTO>>> GetBooks()
        {
            int? currentUserId = null;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int uid))
            {
                currentUserId = uid;
            }

            var books = await _context
                .Books.Include(b => b.Category)
                .Include(b => b.User)
                .Include(b => b.Favorites)
                .OrderByDescending(b => b.CreatedAt)
                .Select(b => new BookListResponseDTO
                {
                    Id = b.Id,
                    Title = b.Title,
                    Slug = b.Slug,
                    Thumbnail = b.Thumbnail,
                    Summary = b.Summary,
                    ViewCount = b.ViewCount,
                    Status = b.Status,
                    CreatedAt = b.CreatedAt,

                    IsFavorited =
                        currentUserId.HasValue
                        && b.Favorites.Any(f => f.UserId == currentUserId.Value),

                    Category = new CategoryDTO
                    {
                        Id = b.Category!.Id,
                        Name = b.Category.Name,
                        Slug = b.Category.Slug,
                    },
                    User = new UserDTO
                    {
                        Id = b.User!.Id,
                        FullName = b.User.FullName,
                        Username = b.User.Username,
                        Avatar = b.User.Avatar,
                    },
                })
                .ToListAsync();

            return Ok(books);
        }

        // GET: api/Books/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<BookDetailResponseDTO>> GetBook(int id)
        {
            int? currentUserId = null;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int uid))
            {
                currentUserId = uid;
            }

            var book = await _context
                .Books.Include(b => b.Category)
                .Include(b => b.User)
                .Include(b => b.Favorites)
                .Include(b => b.Comments)
                .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
                return NotFound();

            var bookDetail = new BookDetailResponseDTO
            {
                Id = book.Id,
                Title = book.Title,
                Slug = book.Slug,
                Thumbnail = book.Thumbnail,
                Summary = book.Summary,
                Content = book.Content,
                ViewCount = book.ViewCount,
                Status = book.Status,
                CreatedAt = book.CreatedAt,

                IsFavorited =
                    currentUserId.HasValue
                    && book.Favorites.Any(f => f.UserId == currentUserId.Value),

                Category = new CategoryDTO
                {
                    Id = book.Category!.Id,
                    Name = book.Category.Name,
                    Slug = book.Category.Slug,
                },
                User = new UserDTO
                {
                    Id = book.User!.Id,
                    FullName = book.User.FullName,
                    Username = book.User.Username,
                    Avatar = book.User.Avatar,
                },

                Comments = book
                    .Comments.Select(c => new CommentDTO
                    {
                        Id = c.Id,
                        Content = c.Content,
                        CreatedAt = c.CreatedAt,
                        BookId = c.BookId,
                        User = new UserDTO
                        {
                            Id = c.User!.Id,
                            FullName = c.User.FullName,
                            Username = c.User.Username,
                            Avatar = c.User.Avatar,
                        },
                    })
                    .OrderByDescending(c => c.CreatedAt)
                    .ToList(),
            };

            return Ok(bookDetail);
        }

        // POST: api/Books
        [Authorize(Roles = "admin,member")]
        [HttpPost]
        public async Task<ActionResult<Book>> PostBook(Book book)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int uid))
            {
                book.UserId = uid;
            }

            book.CreatedAt = DateTime.Now;

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBook", new { id = book.Id }, book);
        }

        // PUT: api/Books/{id}
        [Authorize(Roles = "admin,member")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, Book book)
        {
            if (id != book.Id)
                return BadRequest();

            var oldBook = await _context.Books.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);
            if (oldBook == null)
                return NotFound();

            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var isUserAdmin = User.IsInRole("admin");

            if (!isUserAdmin && oldBook.UserId != currentUserId)
            {
                return Forbid();
            }

            var history = new BookHistory
            {
                BookId = id,
                OldContent = oldBook.Content,
                EditedByUserId = currentUserId,
                EditedAt = DateTime.Now,
            };
            _context.BookHistories.Add(history);

            book.UpdatedAt = DateTime.Now;

            book.UserId = oldBook.UserId;

            _context.Entry(book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Books/{id}
        [Authorize(Roles = "admin,member")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                return NotFound();

            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var isUserAdmin = User.IsInRole("admin");

            if (!isUserAdmin && book.UserId != currentUserId)
            {
                return Forbid();
            }

            book.IsDeleted = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Books/{id}/increment-view
        [HttpPost("{id}/increment-view")]
        public async Task<IActionResult> IncrementViewCount(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                return NotFound();

            book.ViewCount += 1;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Đã tăng lượt xem", newViewCount = book.ViewCount });
        }

        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<BookListResponseDTO>>> GetBooksByCategories(
            [FromQuery] int[] categoryIds
        )
        {
            var query = _context.Books.Include(b => b.Category).Include(b => b.User).AsQueryable();

            if (categoryIds != null && categoryIds.Length > 0)
            {
                query = query.Where(b => categoryIds.Contains(b.CategoryId));
            }

            var books = await query
                .OrderByDescending(b => b.CreatedAt)
                .Select(b => new BookListResponseDTO
                {
                    Id = b.Id,
                    Title = b.Title,
                    Slug = b.Slug,
                    Thumbnail = b.Thumbnail,
                    Summary = b.Summary,
                    ViewCount = b.ViewCount,
                    Status = b.Status,
                    CreatedAt = b.CreatedAt,
                    Category = new CategoryDTO { Id = b.Category!.Id, Name = b.Category.Name },
                    User = new UserDTO { Id = b.User!.Id, FullName = b.User.FullName },
                })
                .ToListAsync();

            return Ok(books);
        }

        [HttpGet("{id}/related")]
        public async Task<ActionResult<IEnumerable<BookListResponseDTO>>> GetRelatedBooks(
            int id,
            [FromQuery] int limit = 4
        )
        {
            var currentBook = await _context.Books.FindAsync(id);
            if (currentBook == null)
                return NotFound();

            var relatedBooks = await _context
                .Books.Include(b => b.Category)
                .Include(b => b.User)
                .Where(b => b.CategoryId == currentBook.CategoryId && b.Id != id)
                .OrderByDescending(b => b.CreatedAt)
                .Take(limit)
                .Select(b => new BookListResponseDTO
                {
                    Id = b.Id,
                    Title = b.Title,
                    Slug = b.Slug,
                    Thumbnail = b.Thumbnail,
                    Summary = b.Summary,
                    ViewCount = b.ViewCount,
                    Status = b.Status,
                    CreatedAt = b.CreatedAt,
                    Category = new CategoryDTO { Id = b.Category!.Id, Name = b.Category.Name },
                    User = new UserDTO { Id = b.User!.Id, FullName = b.User.FullName },
                })
                .ToListAsync();

            return Ok(relatedBooks);
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}
