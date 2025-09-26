using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.DTO
{
    public class AddPublisherRequestDTO
    {
        [Required, MaxLength(200)]
        public string Name { set; get; }
    }
}
