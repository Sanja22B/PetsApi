using System.Net.Http.Json;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using PetStore5.Models; 

namespace PetStore5.Steps
{
    [Binding]
    public class PetStoreSteps
    {
        private readonly HttpClient _httpClient;
        private HttpResponseMessage _response = null!;
        private Pet _newPet = null!;
        private int _createdPetId;

        public PetStoreSteps()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://petstore.swagger.io/v2/");  
        }

        [Given(@"I create a new pet with the name ""(.*)"", category ""(.*)"", photoUrls, tags, and status ""(.*)""")]
        public async Task GivenICreateANewPetWithAllProperties(string name, string category, string status)
        {
            var random = new Random();
            _newPet = new Pet
            {
                id = random.Next(1, 10000),  // Random pet ID
                name = name,
                category = new Category { id = 1, name = category },
                photoUrls = new[] { "https://example.com/fluffy.jpg" },
                tags = new[] { new Category { id = 1, name = "cute" } },
                status = status
            };

            _response = await _httpClient.PostAsJsonAsync("pet", _newPet);
            _response.EnsureSuccessStatusCode();  // Throws if not successful

            // Save the created pet ID
            var content = await _response.Content.ReadAsStringAsync();
            var jsonResponse = JObject.Parse(content);
            _createdPetId = (int)jsonResponse["id"];
        }

        [When(@"I retrieve the pet by its ID")]
        public async Task WhenIRetrieveThePetById()
        {
            _response = await _httpClient.GetAsync($"pet/{_createdPetId}");
            _response.EnsureSuccessStatusCode();
        }

        [Then(@"all properties of the retrieved pet should match the created pet")]
        public async Task ThenAllPropertiesOfTheRetrievedPetShouldMatchTheCreatedPet()
        {
            var content = await _response.Content.ReadAsStringAsync();
            var retrievedPet = JObject.Parse(content);

            // Check all properties against the original created pet
            Assert.AreEqual(_newPet.id, (int)retrievedPet["id"]);
            Assert.AreEqual(_newPet.name, (string)retrievedPet["name"]);
            Assert.AreEqual(_newPet.category.id, (int)retrievedPet["category"]["id"]);
            Assert.AreEqual(_newPet.category.name, (string)retrievedPet["category"]["name"]);
            Assert.AreEqual(_newPet.status, (string)retrievedPet["status"]);

            // Check photoUrls and tags arrays
            var photoUrls = retrievedPet["photoUrls"].ToObject<string[]>();
            Assert.AreEqual(_newPet.photoUrls.Length, photoUrls.Length);
            for (int i = 0; i < _newPet.photoUrls.Length; i++)
            {
                Assert.AreEqual(_newPet.photoUrls[i], photoUrls[i]);
            }

            var tags = retrievedPet["tags"].ToObject<Category[]>();
            Assert.AreEqual(_newPet.tags.Length, tags.Length);
            for (int i = 0; i < _newPet.tags.Length; i++)
            {
                Assert.AreEqual(_newPet.tags[i].id, tags[i].id);
                Assert.AreEqual(_newPet.tags[i].name, tags[i].name);
            }
        }

        [Given(@"I have created a new dog with the name ""(.*)"" and status ""(.*)""")]
        public async Task GivenIHaveCreatedAPetWithNameAndStatus(string name, string status)
        {
            await GivenICreateANewPetWithAllProperties(name, "Cats", status);  // Assuming default category and photoUrls/tags for this step
        }

        [When(@"I update the pet with the name ""(.*)"" and status ""(.*)""")]
        public async Task WhenIUpdateThePetWithNameAndStatus(string newName, string newStatus)
        {
            var updatedPet = new Pet
            {
                id = _createdPetId,  // Use the same pet ID for the update
                name = newName,
                category = new Category { id = 1, name = "Cats" },
                photoUrls = new[] { "https://example.com/fluffy.jpg" },
                tags = new[] { new Category { id = 1, name = "cute" } },
                status = newStatus
            };

            _response = await _httpClient.PutAsJsonAsync("pet", updatedPet);
            _response.EnsureSuccessStatusCode();  // Throws if not successful
        }

        [Then(@"the pet name should be ""(.*)"" and status should be ""(.*)""")]
        public async Task ThenThePetNameShouldBeAndStatusShouldBe(string expectedName, string expectedStatus)
        {
            var content = await _response.Content.ReadAsStringAsync();
            var updatedPet = JObject.Parse(content);

            // Check that the pet's name and status match the expected values
            Assert.AreEqual(expectedName, (string)updatedPet["name"]);
            Assert.AreEqual(expectedStatus, (string)updatedPet["status"]);
        }

        [Given(@"a new dog creation with the name ""(.*)"" and status ""(.*)""")]
        public async Task GivenIHaveCreatedAgainAPetWithNameAndStatus(string name, string status)
        {
            await GivenICreateANewPetWithAllProperties(name, "Dogs", status);  // Assuming default category and photoUrls/tags for this step
        }
        
        [When(@"I delete the pet")]
        public async Task WhenIDeleteThePet()
        {
            _response = await _httpClient.DeleteAsync($"pet/{_createdPetId}");
            _response.EnsureSuccessStatusCode();  // Throws if not successful
        }

        [Then(@"the pet should no longer exist and GET request for its ID should return status code 404")]
        public async Task ThenThePetShouldNoLongerExistAndGetRequestForItsIdShouldReturnStatusCode404()
        {
            _response = await _httpClient.GetAsync($"pet/{_createdPetId}");
            Assert.AreEqual(System.Net.HttpStatusCode.NotFound, _response.StatusCode);
        }
        
        [When(@"I send POST request with petId '(.*)'")]
        public async Task WhenISendAPostRequestToCreateAPetWithInvalidPetId(string invalidPetId)
        {
            var invalidPet = new Invalid_Pet
            {
                id = invalidPetId,
                name = "InvalidPet",
                category = new Category { id = 1, name = "Cats" },
                photoUrls = new[] { "https://example.com/fluffy.jpg" },
                tags = new[] { new Category { id = 1, name = "cute" } },
                status = "available"
            };

            try
            {
                _response = await _httpClient.PostAsJsonAsync("pet", invalidPet);
            }
            catch (Exception ex)
            {
                // Handle any exception that might occur due to invalid input
                _response = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(ex.Message)
                };
            }
        }

        [Then(@"the response should have status code 500 and contain an error message")]
        public async Task ThenTheResponseShouldHaveStatusCode500AndContainAnErrorMessage()
        {
            Assert.AreEqual(System.Net.HttpStatusCode.InternalServerError, _response.StatusCode);
            var content = await _response.Content.ReadAsStringAsync();
            StringAssert.Contains("message", content, "something bad happened");
        }
    }
}
