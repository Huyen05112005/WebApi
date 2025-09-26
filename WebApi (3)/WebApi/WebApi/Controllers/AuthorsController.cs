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
        public IActionResult AddAuthor([FromBody] AddAuthorRequestDTO dto)
        {
            try
            {
                var author = _authorRepository.AddAuthor(dto);
                return Ok(author);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message }); // Bài 2
            }
        }

        [HttpPut]
        [Route("update-author-by-id/{id:int}")]
        public IActionResult UpdateAuthorById([FromRoute] int id, [FromBody] AuthorNoIdDTO dto)
        {
            try
            {
                var updated = _authorRepository.UpdateAuthorById(id, dto);
                if (updated == null) return NotFound();
                return Ok(updated);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete]
        [Route("delete-author-by-id/{id:int}")]
        public IActionResult DeleteAuthorById([FromRoute] int id)
        {
            try
            {
                var deleted = _authorRepository.DeleteAuthorById(id);
                if (deleted == null) return NotFound();
                return Ok(deleted);
            }
            catch (InvalidOperationException ex)
            {
                // Bài 15: gợi ý
                return BadRequest(new { message = "Hãy gỡ liên kết trong Book_Author trước khi xóa", detail = ex.Message });
            }
        }

        #region Private methods 
        private bool ValidateAddAuthor(AddAuthorRequestDTO addAuthorRequestDTO)
        {
            if (addAuthorRequestDTO == null)
            {
                ModelState.AddModelError(nameof(addAuthorRequestDTO), $"Please add author data");
                return false;
            }
            if (string.IsNullOrEmpty(addAuthorRequestDTO.FullName))
            {
                ModelState.AddModelError(nameof(addAuthorRequestDTO.FullName),
$"{nameof(addAuthorRequestDTO.FullName)} cannot be null");
            }

            if (ModelState.ErrorCount > 0)
            {
                return false;
            }

            return true;
        }
        #endregion

    }
}
