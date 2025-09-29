using System.Xml.Linq;
using WebApi.Data;
using WebApi.Models.Domain;
using WebApi.Models.DTO;

namespace WebApi.Repositories
{
    public class SQLPublisherRepository : IPublisherRepository
    {
        private readonly AppDbContext _context;
        public SQLPublisherRepository(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }
        public AddPublisherRequestDTO AddPublisher(AddPublisherRequestDTO addPublisherRequestDTO)
        {
            var exists = _context.Publishers.Any(p => p.Name.ToLower() == addPublisherRequestDTO.Name.ToLower());
            if (exists) throw new InvalidOperationException("Publisher name already exists");

            var publisherDomainModel = new Publisher
            {
                Name = addPublisherRequestDTO.Name
            };
            _context.Publishers.Add(publisherDomainModel);
            _context.SaveChanges();
            return addPublisherRequestDTO;
        }

        public Publisher? DeletePublisherById(int id)
        {
            var publisher = _context.Publishers.Find(id);
            if (publisher != null)
            {
                _context.Publishers.Remove(publisher);
                _context.SaveChanges();
                return publisher;
            }
            return null;
        }

        public List<PublisherDTO> GetAllPublishers()
        {
            var publishers = _context.Publishers.Select(p => new PublisherDTO()
            {
                Id = p.Id,
                Name = p.Name
            }).ToList();
            return publishers;
        }

        public PublisherNoIdDTO GetPublisherById(int id)
        {
            var publisher = _context.Publishers.Find(id);
            if (publisher != null)
            {
                var publisherNoIdDTO = new PublisherNoIdDTO
                {
                    Name = publisher.Name
                };
                return publisherNoIdDTO;
            }
            return null;
        }

        public PublisherNoIdDTO UpdatePublisherById(int id, PublisherNoIdDTO publisherNoIdDTO)
        {
            var publisher = _context.Publishers.Find(id);
            if (publisher != null)
            {
                var hasBooks = _context.Books.Any(b => b.PublisherID == id);
                if (hasBooks)
                    throw new InvalidOperationException("Cannot delete publisher while books still reference it.");

                publisher.Name = publisherNoIdDTO.Name;
                _context.SaveChanges();
                return publisherNoIdDTO;
            }
            return null;
        }
    }
}
