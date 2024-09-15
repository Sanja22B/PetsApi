namespace PetStore5.Models
{
    public class Pet
    {
        public int id { get; set; }
        public string name { get; set; }
        public Category category { get; set; }
        public string[] photoUrls { get; set; }
        public Category[] tags { get; set; }
        public string status { get; set; }
    }
}