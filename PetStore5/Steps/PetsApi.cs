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
        private HttpResponseMessage? _response;
        private Pet? _newPet;
        private int _createdPetId;

        private readonly ScenarioContext _scenarioContext;

        public PetStoreSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _httpClient = new HttpClient();
            // Retrieve the base address from ScenarioContext
            if (_scenarioContext.TryGetValue("BaseUrl", out string baseAddress))
            {
                _httpClient.BaseAddress = new Uri(baseAddress);
            }
            else
            {
                throw new InvalidOperationException("Base address is not set in the configuration file.");
            }
        }

        [Given(@"I create a new pet using the configuration file")]
        public async Task GivenICreateANewPetWithAllProperties()
        {
            _newPet = _scenarioContext["Pet"] as Pet;

            _response = await _httpClient.PostAsJsonAsync("pet", _newPet);
            _response.EnsureSuccessStatusCode(); // Throws if not successful

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
            Assert.NotNull(_response);

            var content = await _response.Content.ReadAsStringAsync();
            var retrievedPet = JObject.Parse(content);

            // Check all properties against the original created pet
            Assert.NotNull(_newPet);

            Assert.AreEqual(_newPet.Id, (int)retrievedPet["id"]);
            Assert.AreEqual(_newPet.Name, (string)retrievedPet["name"]);
            Assert.AreEqual(_newPet.Category.Id, (int)retrievedPet["category"]["id"]);
            Assert.AreEqual(_newPet.Category.Name, (string)retrievedPet["category"]["name"]);
            Assert.AreEqual(_newPet.Status, (string)retrievedPet["status"]);

            // Check photoUrls and tags arrays
            var photoUrls = retrievedPet["photoUrls"].ToObject<string[]>();
            Assert.AreEqual(_newPet.PhotoUrls.Length, photoUrls.Length);
            for (int i = 0; i < _newPet.PhotoUrls.Length; i++)
            {
                Assert.AreEqual(_newPet.PhotoUrls[i], photoUrls[i]);
            }

            var tags = retrievedPet["tags"].ToObject<Category[]>();
            Assert.AreEqual(_newPet.Tags.Length, tags.Length);
            for (int i = 0; i < _newPet.Tags.Length; i++)
            {
                Assert.AreEqual(_newPet.Tags[i].Id, tags[i].Id);
                Assert.AreEqual(_newPet.Tags[i].Name, tags[i].Name);
            }
        }

        [Given(@"I have created a new dog with values from configuration")]
        public async Task GivenIHaveCreatedAPetWithNameAndStatus()
        {
            await GivenICreateANewPetWithAllProperties();
        }

        [When(@"I update the pet with the new name and status from config file")]
        public async Task WhenIUpdateThePetWithNameAndStatus()
        {
            var updatedPet = _scenarioContext["UpdatePet"] as Pet;
            _response = await _httpClient.PutAsJsonAsync("pet", updatedPet);
            _response?.EnsureSuccessStatusCode(); // Throws if not successful
        }

        [Then(@"the pet should have updated name and status")]
        public async Task ThenThePetNameShouldBeAndStatusShouldBe()
        {
            Assert.NotNull(_response);

            var content = await _response.Content.ReadAsStringAsync();
            var updatedPet = JObject.Parse(content);

            // Check that the pet's name and status match the expected values
            Assert.AreEqual(_scenarioContext["UpdatePetName"].ToString(), (string)updatedPet["name"]);
            Assert.AreEqual(_scenarioContext["UpdatePetStatus"].ToString(), (string)updatedPet["status"]);
        }

        [Given(@"a new dog is created using config file")]
        public async Task GivenIHaveCreatedAgainAPetWithNameAndStatus()
        {
            await GivenICreateANewPetWithAllProperties();
        }

        [When(@"I delete the pet")]
        public async Task WhenIDeleteThePet()
        {
            _response = await _httpClient.DeleteAsync($"pet/{_createdPetId}");
            _response.EnsureSuccessStatusCode(); // Throws if not successful
        }

        [Then(@"the pet should no longer exist and GET request for its ID should return status code 404")]
        public async Task ThenThePetShouldNoLongerExistAndGetRequestForItsIdShouldReturnStatusCode404()
        {
            _response = await _httpClient.GetAsync($"pet/{_createdPetId}");
            Assert.AreEqual(System.Net.HttpStatusCode.NotFound, _response.StatusCode);
        }

        [When(@"I send POST request with invalid petId defined in config file")]
        public async Task WhenISendAPostRequestToCreateAPetWithInvalidPetId()
        {
            var invalidPet = _scenarioContext["InvalidPet"] as InvalidPet;
            _response = await _httpClient.PostAsJsonAsync("pet", invalidPet);
        }

        [Then(@"the response should have status code 500 and contain an error message")]
        public async Task ThenTheResponseShouldHaveStatusCode500AndContainAnErrorMessage()
        {
            Assert.NotNull(_response);
            Assert.AreEqual(System.Net.HttpStatusCode.InternalServerError, _response.StatusCode);
            var content = await _response.Content.ReadAsStringAsync();
            var jsonObject = JObject.Parse(content);
            var message = jsonObject["message"]?.ToString();
            Console.WriteLine(message);
            Assert.AreEqual(message, "something bad happened", "error message is wrong");
        }
    }
}