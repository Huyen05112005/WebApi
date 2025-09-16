namespace WebApi.Models.Domain
{
    public class BookAuthor
    {
        public int Id { get; set; }

        public int BookID { get; set; }
        public Book Book { get; set; }

        public int AuthorID { get; set; }
        public Author Author { get; set; }
    }

}
