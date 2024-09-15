Feature: Pet Store API

    Scenario: Create and Retrieve Pet
        Given I create a new pet with the name "Scary", category "Dogs", photoUrls, tags, and status "pending"
        When I retrieve the pet by its ID
        Then all properties of the retrieved pet should match the created pet

    Scenario: Update and Verify Pet
        Given I have created a new dog with the name "Fluffy" and status "available"
        When I update the pet with the name "Snowball" and status "sold"
        Then the pet name should be "Snowball" and status should be "sold"

    Scenario: Delete Pet and Verify Deletion
        Given a new dog creation with the name "Fluffy" and status "available"
        When I delete the pet
        Then the pet should no longer exist and GET request for its ID should return status code 404
        
    Scenario: Post Request with Invalid Input
        When I send POST request with petId 'abc'
        Then the response should have status code 500 and contain an error message