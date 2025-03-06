Solution Overview
-----------------
This overview provides a high-level understanding of the projects involved in the solution and their roles.
The target framework for each project in the application is .net 9.


Projects Involved
-----------------
1.	TechnicalChallenge.WebApp
2.	TechnicalChallenge.BusinessService
3.	TechnicalChallenge.Common
4.	TechnicalChallenge.Data
5.	TechnicalChallenge.Tests


Project Descriptions
--------------------
1. TechnicalChallenge.WebApp
  •	Type: .net MVC application
  •	Purpose: This project serves as the front-end of the application. It handles user interactions, displays data, and communicates with the back-end services.
    MVC was choosen rather than Blazor or any other rich feature front end tool as it the tasks to be carried out by the application did not require a feature rich UI experience.
  •	Key Components:
    -	Controllers: Manages the HTTP requests and responses. Examples include AccountController, HomeController, and SecurityController.
    -	Views: Razor views that render the HTML content for the user interface.
    -	Models: ViewModels used to transfer data between the controllers and views.
    -	Program.cs: Configures the application, sets up services, and defines the middleware pipeline.

2. TechnicalChallenge.BusinessService
  •	Type: Class Library
  •	Purpose: This project contains the business logic and service layer of the application. It handles data processing, business rules, and interactions with data sources.
  •	Key Components:
    -	Services: Interfaces and implementations for business operations, such as ICustomerService and ILoanService.
    -	DTOs (Data Transfer Objects): Classes used to transfer data between different layers of the application.
    -	BusinessConfiguration: Contains methods for setting up demo data and configuring business services.

3. TechnicalChallenge.Common
  •	Type: Class Library
  •	Purpose: This project contains common utilities, constants, and helper methods that are used across multiple projects in the solution.

5. TechnicalChallenge.Data
  •	Type: Class Library
  •	Purpose: This project handles data access and storage. It interacts with the database and provides data to the business service layer.
  •	Key Components:
    -	Repositories: Interfaces and implementations for data access operations.
    -	Entities: Entity classes representing the database schema.
    -	DbContext: The Entity Framework Core DbContext for managing database connections and operations.
  •	For the purposes of this technical challenge I have used an im memory database which is created each time the application is started.
    In a full production version we would swap this for a SQL database.
    Then using entity code first and migrations we would manage the creation and management of database changes.

5. TechnicalChallenge.Tests
  •	Type: Unit Test Project
  •	Purpose: This project contains unit tests to ensure the correctness and reliability of the business service code. 
    Due to time contraints only this layer was unit tested. A more robust approach would be to unit test all major components.
  •	Key Components:
    -	Test Classes: Classes containing unit tests for various components and services.
    -	Mocking: Use of mocking frameworks to simulate dependencies and isolate the code under test.
    -	Test Data: Sample data used for testing purposes.


How They Work Together
----------------------

TechnicalChallenge.WebApp
•	Uses controllers to handle HTTP requests and return appropriate views.
•	Communicates with the TechnicalChallenge.BusinessService project to perform business operations.
•	Uses dependency injection to inject services from TechnicalChallenge.BusinessService into controllers.
•	Configures authentication and authorization to secure the application.

TechnicalChallenge.BusinessService:
•	Provides business logic and data processing capabilities.
•	Defines interfaces for services that are implemented to handle specific business operations.
•	Supplies data transfer objects (DTOs) to ensure consistent data exchange between layers.
•	Utilizes common utilities and constants from TechnicalChallenge.Common.

TechnicalChallenge.Common:
•	Provides shared utilities, constants, and helper methods used across multiple projects.
•	Enhances code reusability and maintainability by centralizing common functionality.

TechnicalChallenge.Data:
•	Manages data access and storage operations.
•	Provides repositories for data access and interacts with the database using Entity Framework Core.
•	Supplies data entities to the business service layer.
•	TechnicalChallenge.Tests:
•	Ensures the correctness and reliability of the application's code through unit tests.
•	Uses mocking frameworks to simulate dependencies and isolate the code under test.
•	Provides test data and scenarios to validate the functionality of various components and services.

  
Example Workflow
----------------
1.	User Login:
  •	The user navigates to the login page (/Security/Login).
  •	The SecurityController handles the login request, validates the user credentials using ICustomerService, and sets up authentication cookies.
  •	Upon successful login, the user is redirected to the home page (/Home/Index).
2.	Account Details:
  •	The user navigates to the account details page (/Account/AccountDetail/{id}).
  •	The AccountController retrieves account details using ICustomerService and displays them in a view.
  •	The user can perform actions such as transferring funds, which are handled by the AccountController and processed by ICustomerService.
3.	Loan Application:
  •	The user navigates to the loan application page (/Account/Loan).
  •	The AccountController retrieves available loan options using ILoanService and displays them in a view.
  •	The user submits a loan application, which is processed by ILoanService.

