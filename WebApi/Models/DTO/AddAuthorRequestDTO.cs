using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.DTO
{
    public class AddAuthorRequestDTO
    {
        [Required, MinLength(3)]
        public string FullName { set; get; }
    }
}
