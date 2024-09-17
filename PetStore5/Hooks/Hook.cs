using System;
using Microsoft.Extensions.Configuration;
using PetStore5.Models;

namespace PetStore5.Hooks
{
    [Binding]
    public class Hooks
    {
        private readonly ScenarioContext _scenarioContext;

        public Hooks(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [BeforeScenario]
        public void LoadPetNameFromConfig()
        {
            // Load configuration from configuration.json
            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("configuration.json", optional: false, reloadOnChange: true)
                .Build();

            var baseUrl = config["Urls:baseUrl"];
            _scenarioContext["BaseUrl"] = baseUrl;
            // Bind the entire Pet section to the Pet class
            var pet = config.GetSection("Pet").Get<Pet>();
            _scenarioContext["Pet"] = pet;
            
            var updatePet = config.GetSection("UpdatePet").Get<Pet>();
            _scenarioContext["UpdatePet"] = updatePet;
            _scenarioContext["UpdatePetName"] = updatePet?.Name;
            _scenarioContext["UpdatePetStatus"] = updatePet?.Status;
            
            var invalidPet = config.GetSection("InvalidPet").Get<InvalidPet>();
            _scenarioContext["InvalidPet"] = invalidPet;
            
            // var id = config["Pet:Id"];
            // var petName = config["Pet:Name"];
            // var category = config["Pet:Category"];
            // var photoUrls = config.GetSection("Pet:PhotoUrls").Get<string[]>();
            // var tags = config["Pet:Tags"];
            // var status = config["Pet:Status"];
            
            // Replace the Pets parameters in the feature file
            // _scenarioContext["PetName"] = petName;
            // _scenarioContext["Id"] = id;
            // _scenarioContext["CategoryName"] = pet?.Category.Name;
            // _scenarioContext["PhotoUrls"] = photoUrls;
            // _scenarioContext["Tags"] = tags;
            // _scenarioContext["Status"] = status;
        }
    }
}