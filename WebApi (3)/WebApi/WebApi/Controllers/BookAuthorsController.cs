using Microsoft.AspNetCore.Mvc;
using WebApi.Data;
using WebApi.Models.Domain;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/book-authors")]
    public class BookAuthorsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public BookAuthorsController(AppDbContext db) { _db = db; }

        public record LinkDTO(int BookID, int AuthorID);

        [HttpPost]
        public IActionResult AddLink([FromBody] LinkDTO dto)
        {
            if (!_db.Books.Any(b => b.Id == dto.BookID))
                return BadRequest(new { message = "BookID does not exist" });
            if (!_db.Authors.Any(a => a.Id == dto.AuthorID))
                return BadRequest(new { message = "AuthorID does not exist" });

            bool exists = _db.BookAuthors.Any(x => x.BookID == dto.BookID && x.AuthorID == dto.AuthorID);
            if (exists) return Conflict(new { message = "Relation (BookID-AuthorID) already exists" });

            _db.BookAuthors.Add(new BookAuthor { BookID = dto.BookID, AuthorID = dto.AuthorID });
            _db.SaveChanges();
            return Ok(dto);
        }

        [HttpDelete]
        public IActionResult RemoveLink([FromBody] LinkDTO dto)
        {
            var link = _db.BookAuthors.FirstOrDefault(x => x.BookID == dto.BookID && x.AuthorID == dto.AuthorID);
            if (link == null) return NotFound();
            _db.BookAuthors.Remove(link);
            _db.SaveChanges();
            return Ok(dto);
        }
    }
}
