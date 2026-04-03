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
    public class CommentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CommentsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Comments/Book/{id}
        [HttpGet("Book/{bookId}")]
        public async Task<ActionResult<IEnumerable<CommentDTO>>> GetCommentsByBook(int bookId)
        {
            var comments = await _context
                .Comments.Where(c => c.BookId == bookId)
                .Include(c => c.User)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new CommentDTO
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
                .ToListAsync();

            return comments;
        }

        // GET: api/Comments/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Comment>> GetComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

            return comment;
        }

        // POST: api/Comments
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Comment>> PostComment(Comment comment)
        {
            comment.CreatedAt = DateTime.Now;

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetComment", new { id = comment.Id }, comment);
        }

        // PUT: api/Comments/{id}
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutComment(int id, Comment comment)
        {
            if (id != comment.Id)
            {
                return BadRequest();
            }

            _context.Entry(comment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentExists(id))
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

        // DELETE: api/Comments/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.Id == id);
        }
    }
}
