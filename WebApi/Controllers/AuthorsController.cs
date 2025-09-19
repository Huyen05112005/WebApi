using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models.DTO;
using WebApi.Repositories;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorRepository _authorRepository;
        public AuthorsController(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }
        [HttpGet]
        [Route("get-all-author")]
        public IActionResult GetAllAuthors()
        {
            var authors = _authorRepository.GetAllAuthors();
            return Ok(authors);
        }
        [HttpGet]
        [Route("get-author-by-id/{id:int}")]
        public IActionResult GetAuthorById([FromRoute] int id)
        {
            var author = _authorRepository.GetAuthorById(id);
            if (author == null)
            {
                return NotFound();
            }
            return Ok(author);
        }
        [HttpPost]
        [Route("authors")]
        public IActionResult AddAuthor([FromBody] AddAuthorRequestDTO addAuthorRequestDTO)
        {
            var author = _authorRepository.AddAuthor(addAuthorRequestDTO);
            return Ok(author);
        }
        [HttpPut]
        [Route("update-author-by-id/{id:int}")]
        public IActionResult UpdateAuthorById([FromRoute] int id, [FromBody] AuthorNoIdDTO authorNoIdDTO)
        {
            var updatedAuthor = _authorRepository.UpdateAuthorById(id, authorNoIdDTO);
            if (updatedAuthor == null)
            {
                return NotFound();
            }
            return Ok(updatedAuthor);
        }
        [HttpDelete]
        [Route("delete-author-by-id/{id:int}")]
        public IActionResult DeleteAuthorById([FromRoute] int id)
        {
            var deletedAuthor = _authorRepository.DeleteAuthorById(id);
            if (deletedAuthor == null)
            {
                return NotFound();
            }
            return Ok(deletedAuthor);
        }
    }
}
