# Flag Explorer Application

A backend application for exploring country information, including flags, population, and capital details. The project consists of:
- **Backend**: A .NET 8 Web API for managing and retrieving country data, integrated with Azure Cosmos DB.

---

## Features

### **Backend (API)**
- **Endpoints**:
  - Retrieve all countries.
  - Retrieve details about a specific country by name.
- **Database**: Azure Cosmos DB (with support for local Cosmos DB Emulator).
- **Swagger/OpenAPI**: Automatically generated API documentation.
- **Docker Support**: Fully containerized with Docker and Docker Compose.
- **CI/CD**: Azure DevOps pipeline for build, test, and deployment.

---

## Architecture

This project follows **Clean Architecture** principles to ensure separation of concerns, scalability, and maintainability. The architecture is organized into the following layers:

### **1. Domain Layer**
- Contains the core business logic and domain entities.
- Defines interfaces for repositories and unit of work to abstract data access.
- Example entities: `Country`, `CountryDetail`.

### **2. Application Layer**
- Implements the application logic using the **CQRS (Command Query Responsibility Segregation)** pattern.
- Contains:
  - **Commands**: For write operations (e.g., adding or updating data).
  - **Queries**: For read operations (e.g., retrieving data).
  - **DTOs**: Data Transfer Objects for communication between layers.
  - **Validators**: For input validation using FluentValidation.
- Example: `GetCountriesQueryHandler`, `GetCountryDetailByNameQueryHandler`.

### **3. Infrastructure Layer**
- Handles data persistence and external integrations.
- Implements repository patterns for data access using Azure Cosmos DB.
- Example: `CountryCosmosDBRepository`, `CosmosDBUnitOfWork`.

### **4. API Layer**
- Exposes the application functionality through RESTful endpoints.
- Uses controllers to handle HTTP requests and responses.
- Example: `CountryController`, `CountryDetailController`.

### **Key Design Principles**
- **Dependency Inversion**: Higher-level layers depend on abstractions defined in the domain layer.
- **Separation of Concerns**: Each layer has a distinct responsibility.
- **Testability**: Layers are loosely coupled, making it easier to write unit and integration tests.

---

## Technologies Used

### **Backend**
- **Framework**: .NET 8
- **Database**: Azure Cosmos DB
- **API Documentation**: Swagger/OpenAPI
- **Containerization**: Docker and Docker Compose
- **CI/CD**: Azure DevOps

---

## Getting Started

### **Prerequisites**
- .NET 8 SDK
- Docker and Docker Compose
- Azure Cosmos DB or Cosmos DB Emulator

---

### **Backend Setup**

1. Clone the repository:
2. Run the Cosmos DB Emulator (if not using Azure Cosmos DB):
3. Update `appsettings.Development.json` with the Cosmos DB Emulator connection string:
4. Run the application:
5. Access the Swagger UI at `https://localhost:5001/swagger`.

## Project Structure

FlagExplorerApp/ ├── FlagExplorerApp.Api/               
# Backend API │   ├── Controllers/                   
# API Controllers │   ├── Application/                   
# Application layer (CQRS, DTOs, etc.) │   ├── Domain/                        
# Domain entities and interfaces │   ├── Infrastructure/                
# Data access and persistence │   ├── Program.cs                     
# Application entry point │   ├── appsettings.json               
# Configuration files │   └── Dockerfile                     
# Docker configuration for the API ├── docker-compose.yml                 
# Docker Compose configuration └── azure-pipelines.yml                
# CI/CD pipeline configuration

---

## Testing

### **Backend**
1. Run unit tests:
2. Run integration tests (ensure the Cosmos DB Emulator is running):


---

## CI/CD Pipeline

The project includes an Azure DevOps pipeline for continuous integration and deployment. The pipeline:
1. Builds the application.
2. Runs unit tests.
3. Deploys to Azure App Service for development, testing, staging, and production environments.

### **Pipeline Configuration**
The pipeline is defined in `azure-pipelines.yml` and includes the following stages:
- **Build**: Restores dependencies, builds the project, and publishes the output.
- **Deploy to Development**: Deploys the application to the development environment.
- **Deploy to Testing**: Deploys the application to the testing environment.
- **Deploy to Staging**: Deploys the application to the staging environment.
- **Deploy to Production**: Deploys the application to the production environment.

---

## API Endpoints

### **Country API**
- **GET /countries**: Retrieve all countries.
- **GET /countries/{name}**: Retrieve details about a specific country.

---

## Deployment

The backend is deployed to Azure App Service. Update the `azure-pipelines.yml` file with your Azure subscription and resource details.

---

## License

This project is licensed under the MIT License.