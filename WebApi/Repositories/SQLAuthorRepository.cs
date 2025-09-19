using WebApi.Data;
using WebApi.Models.Domain;
using WebApi.Models.DTO;

namespace WebApi.Repositories
{
    public class SQLAuthorRepository : IAuthorRepository
    {
        private readonly AppDbContext _context;
        public SQLAuthorRepository(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }
        public AddAuthorRequestDTO AddAuthor(AddAuthorRequestDTO addAuthorRequestDTO)
        {
            var author = new Author()
            {
                FullName = addAuthorRequestDTO.FullName,
            };
            _context.Authors.Add(author);
            _context.SaveChanges();
            return addAuthorRequestDTO;
        }

        public Author? DeleteAuthorById(int id)
        {
            var author = _context.Authors.Find(id);
            if (author != null)
            {
                _context.Authors.Remove(author);
                _context.SaveChanges();
                return author;
            }
            return null;
        }

        public List<AuthorDTO> GetAllAuthors()
        {
            var authors = _context.Authors.Select(a => new AuthorDTO()
            {
                Id = a.Id,
                FullName = a.FullName,
            }).ToList();
            return authors;
        }

        public AuthorNoIdDTO GetAuthorById(int id)
        {
            var author = _context.Authors.Find(id);
            if (author != null)
            {
                var authorDTO = new AuthorNoIdDTO()
                {
                    FullName = author.FullName,
                };
                return authorDTO;
            }
            return null;
        }

        public AuthorNoIdDTO UpdateAuthorById(int id, AuthorNoIdDTO authorNoIdDTO)
        {
            var author = _context.Authors.Find(id);
            if (author != null)
            {
                author.FullName = authorNoIdDTO.FullName;
                _context.SaveChanges();
                return authorNoIdDTO;
            }
            return null;
        }
    }
}
