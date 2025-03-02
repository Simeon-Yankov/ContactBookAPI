# ContactBookAPI
**Web API** that creates and maintains a phone and address book.



### Running the solution with Docker Compose

> [!WARNING]
> Remember to ensure that Docker Engine is started

#### 1. Clone the Repository

```powershell
git clone https://github.com/Simeon-Yankov/ContactBookAPI.git
cd ContactBookAPI
```

#### 2. Build and Run with Docker Compose

```powershell
docker-compose up --build
```

#### 3.  Access the API

* API Base URL: http://localhost:8080

* Swagger Documentation: http://localhost:8080/api/index.html


## 🚀 Tech Stack

* ASP.NET Core 8.0

* Entity Framework Core

* PostgreSQL

* MediatR

* Dapper

* Swagger/OpenAPI

* Docker

  

## ✨ Features

* Clean Architecture with Domain-Driven Design

* CQRS pattern with MediatR

* Domain validation with custom exceptions

* Comprehensive error handling

* Performance monitoring for requests

* Extensive test coverage (Application.FunctionalTests, Domain.UnitTests)



## 🔑 Key Implementation Details

* Dual Query Implementation: Queries use both EF Core (v1) and Dapper (v2) for performance comparison.

* Middleware Logging: The API includes middleware that logs the person ID from the request path using Serilog file sink.

* CRUD Operations:

  - Create, edit, and delete contacts with full name, addresses, and phone numbers.

  - Retrieve all contacts or filter them with pagination.
 
  

## 🏗️ Project Structure

* Domain - Business entities, value objects, and domain exceptions

* Application - Business logic, CQRS commands/queries, and validation

* Infrastructure - Database context, migrations, and external service implementations

* Web - API controllers, middleware, and configuration



## 📜 License

This project is licensed under the MIT License - see the LICENSE file for details.
