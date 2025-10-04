namespace Library_web.Models
{
    public class editBookDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsRead { get; set; }
        public DateTime? DateRead { get; set; }
        public int? Rate { get; set; }
        public string Genre { get; set; }
        public string CoverUrl { get; set; }
        public DateTime DateAdded { get; set; }
        public int PublisherID { get; set; }
        public List<int> AuthorIDs { get; set; }
    }

    public class authorDTO
    {
        public int Id { get; set; }
        public string FullName { get; set; }
    }

    public class publisherDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class publisherNoIdDTO
    {
        public string Name { get; set; }
    }

}
