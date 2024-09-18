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
        }
    }
}