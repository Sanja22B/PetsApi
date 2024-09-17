namespace PetStore5.Models
{
    public class InvalidPet
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Category Category { get; set; }
        public string[] PhotoUrls { get; set; }
        public Category[] Tags { get; set; }
        public string Status { get; set; }
    }
}