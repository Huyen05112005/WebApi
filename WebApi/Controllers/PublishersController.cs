using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Repositories;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublishersController : ControllerBase
    {
        private readonly IPublisherRepository _publisherRepository;
        public PublishersController(IPublisherRepository publisherRepository)
        {
            _publisherRepository = publisherRepository;
        }
        [HttpGet]
        [Route("get-all-publishers")]
        public IActionResult GetAllPublishers()
        {
            var publishers = _publisherRepository.GetAllPublishers();
            return Ok(publishers);
        }
        [HttpGet]
        [Route("get-publisher-by-id/{id:int}")]
        public IActionResult GetPublishers(int id)
        {
            var publisher = _publisherRepository.GetPublisherById(id);
            if (publisher != null)
            {
                return Ok(publisher);
            }
            return NotFound();
        }

        [HttpPost]
        [Route("add-publisher")]
        public IActionResult AddPublisher([FromBody] Models.DTO.AddPublisherRequestDTO dto)
        {
            try
            {
                var publisher = _publisherRepository.AddPublisher(dto);
                return Ok(publisher);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut]
        [Route("update-publisher-by-id/{id:int}")]
        public IActionResult UpdatePublisherById(int id, [FromBody] Models.DTO.PublisherNoIdDTO publisherNoIdDTO)
        {
            var updatedPublisher = _publisherRepository.UpdatePublisherById(id, publisherNoIdDTO);
            if (updatedPublisher == null)
            {
                return NotFound();
            }
            return Ok(updatedPublisher);
        }
        [HttpDelete]
        [Route("delete-publisher-by-id/{id:int}")]
        public IActionResult DeletePublisherById(int id)
        {
            try
            {
                var deleted = _publisherRepository.DeletePublisherById(id);
                if (deleted == null) return NotFound();
                return Ok(deleted);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
