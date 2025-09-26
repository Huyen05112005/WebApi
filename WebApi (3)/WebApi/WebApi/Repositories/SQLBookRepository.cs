using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Models.Domain;
using WebApi.Models.DTO;
using WebApi.Models.Validation;

namespace WebApi.Repositories
{
    public class SQLBookRepository : IBookRepository
    {
        private readonly AppDbContext _dbContext;
        public SQLBookRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<BookWithAuthorAndPublisherDTO> GetAllBooks(string? filterOn = null, string?
 filterQuery = null,
             string? sortBy = null, bool isAscending = true, int pageNumber = 1, int
 pageSize = 1000)
        {
            var allBooks = _dbContext.Books.Select(Books => new
BookWithAuthorAndPublisherDTO()
            {
                Id = Books.Id,
                Title = Books.Title,
                Description = Books.Description,
                IsRead = Books.IsRead,
                DateRead = Books.IsRead ? Books.DateRead : null,
                Rate = Books.IsRead ? Books.Rate : null,
                Genre = Books.Genre,
                CoverUrl = Books.CoverUrl,
                PublisherName = Books.Publisher.Name,
                AuthorNames = Books.BookAuthors.Select(n => n.Author.FullName).ToList()
            }).AsQueryable();
            //filtering 
            if (string.IsNullOrWhiteSpace(filterOn) == false &&
string.IsNullOrWhiteSpace(filterQuery) == false)
            {
                if (filterOn.Equals("title", StringComparison.OrdinalIgnoreCase))
                {
                    allBooks = allBooks.Where(x => x.Title.Contains(filterQuery));
                }
            }

            //sorting 
            if (string.IsNullOrWhiteSpace(sortBy) == false)
            {
                if (sortBy.Equals("title", StringComparison.OrdinalIgnoreCase))
                {
                    allBooks = isAscending ? allBooks.OrderBy(x => x.Title) :
allBooks.OrderByDescending(x => x.Title);
                }
            }
            //pagination 
            var skipResults = (pageNumber - 1) * pageSize;
            return allBooks.Skip(skipResults).Take(pageSize).ToList();

        }
        public BookWithAuthorAndPublisherDTO GetBookById(int id)
        {
            var bookWithDomain = _dbContext.Books.Where(n => n.Id == id);
            //Map Domain Model to DTOs 
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
            return bookWithIdDTO;
        }

        public AddBookRequestDTO AddBook(AddBookRequestDTO addBookRequestDTO)
        {
            if (!Validators.IsValidTitle(addBookRequestDTO.Title))
                throw new ArgumentException("Book.Title is required and must not contain special characters.");

            var publisher = _dbContext.Publishers.FirstOrDefault(p => p.Id == addBookRequestDTO.PublisherID);
            if (publisher == null)
                throw new KeyNotFoundException("PublisherID does not exist.");

            if (addBookRequestDTO.AuthorIds == null || addBookRequestDTO.AuthorIds.Count == 0)
                throw new InvalidOperationException("Book must have at least one author.");

            var distinctAuthorIds = addBookRequestDTO.AuthorIds.Distinct().ToList();
            var existAuthors = _dbContext.Authors.Where(a => distinctAuthorIds.Contains(a.Id)).Select(a => a.Id).ToList();
            if (existAuthors.Count != distinctAuthorIds.Count)
                throw new KeyNotFoundException("Some AuthorID(s) do not exist.");

            bool duplicateTitle = _dbContext.Books.Any(b => b.PublisherID == addBookRequestDTO.PublisherID && b.Title.ToLower() == addBookRequestDTO.Title.ToLower());
            if (duplicateTitle)
                throw new InvalidOperationException("Duplicated book title within the same publisher.");

            foreach (var aid in distinctAuthorIds)
            {
                int count = _dbContext.BookAuthors.Count(x => x.AuthorID == aid);
                if (count >= 20)
                    throw new InvalidOperationException($"Author {aid} exceeded max limit (20) books.");
            }

            var year = addBookRequestDTO.DateAdded != null ? addBookRequestDTO.DateAdded.Year : DateTime.UtcNow.Year;
            int pubCountThisYear = _dbContext.Books.Count(b => b.PublisherID == addBookRequestDTO.PublisherID && (b.DateAdded != null ? b.DateAdded.Year : DateTime.UtcNow.Year) == year);
            if (pubCountThisYear >= 100)
                throw new InvalidOperationException($"Publisher exceeded yearly limit (100) books in {year}.");

            //map DTO to Domain Model 
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
            //Use Domain Model to add Book 
            _dbContext.Books.Add(bookDomainModel);
            _dbContext.SaveChanges();

            foreach (var id in addBookRequestDTO.AuthorIds)
            {
                var _book_author = new BookAuthor()
                {
                    BookID = bookDomainModel.Id,
                    AuthorID = id
                };
                _dbContext.BookAuthors.Add(_book_author);
                _dbContext.SaveChanges();
            }
            return addBookRequestDTO;
        }


        public AddBookRequestDTO? UpdateBookById(int id, AddBookRequestDTO dto)
        {
            var book = _dbContext.Books.Include(b => b.BookAuthors).FirstOrDefault(n => n.Id == id);
            if (book == null) return null;

            if (!Validators.IsValidTitle(dto.Title))
                throw new ArgumentException("Book.Title is required and must not contain special characters.");

            var publisher = _dbContext.Publishers.Find(dto.PublisherID);
            if (publisher == null)
                throw new KeyNotFoundException("PublisherID does not exist.");

            if (dto.AuthorIds == null || dto.AuthorIds.Count == 0)
                throw new InvalidOperationException("Book must have at least one author.");

            var distinctAuthorIds = dto.AuthorIds.Distinct().ToList();
            var existAuthors = _dbContext.Authors.Where(a => distinctAuthorIds.Contains(a.Id)).Select(a => a.Id).ToList();
            if (existAuthors.Count != distinctAuthorIds.Count)
                throw new KeyNotFoundException("Some AuthorID(s) do not exist.");

            bool duplicateTitle = _dbContext.Books.Any(b => b.Id != id && b.PublisherID == dto.PublisherID && b.Title.ToLower() == dto.Title.ToLower());
            if (duplicateTitle)
                throw new InvalidOperationException("Duplicated book title within the same publisher.");

            foreach (var aid in distinctAuthorIds)
            {
                int count = _dbContext.BookAuthors.Count(x => x.AuthorID == aid && x.BookID != id);
                if (count >= 20)
                    throw new InvalidOperationException($"Author {aid} exceeded max limit (20) books.");
            }

            var year = dto.DateAdded != null ? book.DateAdded.Year : DateTime.UtcNow.Year;
            int pubCountThisYear = _dbContext.Books.Count(b => b.Id != id && b.PublisherID == dto.PublisherID && (b.DateAdded == null ? b.DateAdded.Year : DateTime.UtcNow.Year) == year);
            if (pubCountThisYear >= 100)
                throw new InvalidOperationException($"Publisher exceeded yearly limit (100) books in {year}.");

            using var tran = _dbContext.Database.BeginTransaction();

            book.Title = dto.Title.Trim();
            book.Description = dto.Description;
            book.IsRead = dto.IsRead;
            book.DateRead = dto.IsRead ? dto.DateRead : DateTime.MinValue;
            book.Rate = dto.IsRead ? dto.Rate : 0;
            book.Genre = dto.Genre;
            book.CoverUrl = dto.CoverUrl;
            book.DateAdded = dto.DateAdded;
            book.PublisherID = dto.PublisherID;
            _dbContext.SaveChanges();

            var old = _dbContext.BookAuthors.Where(a => a.BookID == id);
            _dbContext.BookAuthors.RemoveRange(old);
            _dbContext.SaveChanges();

            foreach (var aid in distinctAuthorIds)
            {
                var link = new BookAuthor { BookID = id, AuthorID = aid };
                _dbContext.BookAuthors.Add(link);
            }
            _dbContext.SaveChanges();

            tran.Commit();
            return dto;
        }

        public Book? DeleteBookById(int id)
        {
            var book = _dbContext.Books.FirstOrDefault(n => n.Id == id);
            if (book == null) return null;

            _dbContext.Books.Remove(book);
            _dbContext.SaveChanges();
            return book;
        }
    }
}
