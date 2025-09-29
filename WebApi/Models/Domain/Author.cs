using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.Domain
{
    public class Author
    {
        [Key]
        public int Id { get; set; }
        public string FullName { get; set; }

        public ICollection<BookAuthor> BookAuthors { get; set; }
    }

}
