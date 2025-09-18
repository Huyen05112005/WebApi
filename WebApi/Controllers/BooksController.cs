using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Models.Domain;
using WebApi.Models.DTO;

namespace WebApi.Controllers
{
    [Route("api/")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        public BooksController(AppDbContext context)
        {
            _dbContext = context;
        }
        [HttpGet("get-all-books")]
        public IActionResult GetAll()
        {
            var allBooksDomain = _dbContext.Books;

            var allBooksDTO = allBooksDomain.Select(book => new BookWithAuthorAndPublisherDTO()
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                IsRead = book.IsRead,
                DateRead = book.IsRead ? book.DateRead : null,
                Rate = book.IsRead ? book.Rate : null,
                Genre = book.Genre,
                CoverUrl = book.CoverUrl,
                DateAdded = book.DateAdded,
                PublisherName = book.Publisher.Name,
                AuthorNames = book.BookAuthors.Select(n => n.Author.FullName).ToList()
            }).ToList();

            return Ok(allBooksDTO);
        }


        [HttpGet]
        [Route("get-book-by-id/{id}")]
        public IActionResult GetBookById([FromRoute] int id)
        {
            var bookWithDomain = _dbContext.Books.Where(n => n.Id == id);

            if (!bookWithDomain.Any())
            {
                return NotFound();
            }

            var bookWithIdDTO = bookWithDomain.Select(book => new BookWithAuthorAndPublisherDTO()
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                IsRead = book.IsRead,
                DateRead = book.DateRead,
                Rate = book.Rate,
                Genre = book.Genre,
                CoverUrl = book.CoverUrl,
                PublisherName = book.Publisher.Name,
                AuthorNames = book.BookAuthors.Select(n => n.Author.FullName).ToList()
            }).FirstOrDefault();

            return Ok(bookWithIdDTO);
        }

        [HttpPost("add-book")]
        public IActionResult AddBook([FromBody] AddBookRequestDTO addBookRequestDTO)
        {
            var bookDomainModel = new Book
            {
                Title = addBookRequestDTO.Title,
                Description = addBookRequestDTO.Description,
                IsRead = addBookRequestDTO.IsRead,
                DateRead = addBookRequestDTO.DateRead,
                Rate = addBookRequestDTO.Rate,
                Genre = addBookRequestDTO.Genre,
                CoverUrl = addBookRequestDTO.CoverUrl,
                DateAdded = addBookRequestDTO.DateAdded,
                PublisherID = addBookRequestDTO.PublisherID
            };

            _dbContext.Books.Add(bookDomainModel);
            _dbContext.SaveChanges();

            foreach (var id in addBookRequestDTO.AuthorIds)
            {
                var bookAuthor = new BookAuthor
                {
                    BookID = bookDomainModel.Id,
                    AuthorID = id
                };

                _dbContext.BookAuthors.Add(bookAuthor);
                _dbContext.SaveChanges();
            }

            return Ok();
        }


        [HttpPut("update-book-by-id/{id}")]
        public IActionResult UpdateBookById(int id, [FromBody] AddBookRequestDTO bookDTO)
        {
            var bookDomain = _dbContext.Books.FirstOrDefault(n => n.Id == id);
            if (bookDomain != null)
            {
                bookDomain.Title = bookDTO.Title;
                bookDomain.Description = bookDTO.Description;
                bookDomain.IsRead = bookDTO.IsRead;
                bookDomain.DateRead = bookDTO.DateRead;
                bookDomain.Rate = bookDTO.Rate;
                bookDomain.Genre = bookDTO.Genre;
                bookDomain.CoverUrl = bookDTO.CoverUrl;
                bookDomain.DateAdded = bookDTO.DateAdded;
                bookDomain.PublisherID = bookDTO.PublisherID;

                _dbContext.SaveChanges();
            }

            var authorDomain = _dbContext.BookAuthors.Where(a => a.Id == id).ToList();
            if (authorDomain != null)
            {
                _dbContext.BookAuthors.RemoveRange(authorDomain);
                _dbContext.SaveChanges();
            }

            foreach (var authorid in bookDTO.AuthorIds)
            {
                var bookAuthor = new BookAuthor
                {
                    BookID = id,
                    AuthorID = authorid
                };
                _dbContext.BookAuthors.Add(bookAuthor);
                _dbContext.SaveChanges();
            }

            return Ok(bookDTO);
        }

        [HttpDelete("delete-book-by-id/{id}")]
        public IActionResult DeleteBookById(int id)
        {
            var bookDomain = _dbContext.Books.FirstOrDefault(n => n.Id == id);
            if (bookDomain != null)
            {
                _dbContext.Books.Remove(bookDomain);
                _dbContext.SaveChanges();
            }
            return Ok();
        }


    }
}
