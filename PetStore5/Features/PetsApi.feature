Feature: Pet Store API

    Scenario: Create and Retrieve Pet
        Given I create a new pet using the configuration file 
        When I retrieve the pet by its ID
        Then all properties of the retrieved pet should match the created pet

    Scenario: Update and Verify Pet
        Given I have created a new dog with values from configuration
        When I update the pet with the new name and status from config file
        Then the pet should have updated name and status

    Scenario: Delete Pet and Verify Deletion
        Given a new dog is created using config file
        When I delete the pet
        Then the pet should no longer exist and GET request for its ID should return status code 404
        
    Scenario: Post Request with Invalid Input
        When I send POST request with invalid petId defined in config file
        Then the response should have status code 500 and contain an error message