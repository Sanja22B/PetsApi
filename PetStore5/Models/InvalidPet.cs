namespace PetStore5.Models
{
    public class Invalid_Pet
    {
        public string id { get; set; }
        public string name { get; set; }
        public Category category { get; set; }
        public string[] photoUrls { get; set; }
        public Category[] tags { get; set; }
        public string status { get; set; }
    }
}