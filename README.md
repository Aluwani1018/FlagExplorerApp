# FlagExplorerApp

FlagExplorerApp is a .NET 8 application designed to manage and explore country and country detail data using Azure Cosmos DB. The project is structured with a clean architecture approach, separating concerns across multiple layers.

## Project Structure

- **FlagExplorerApp.Api**: The API layer for handling HTTP requests and responses.  
  [Read more in the API README](./FlagExplorerApp.Api/README.md)

- **FlagExplorerApp.Web**: The frontend application (if applicable).  
  [Read more in the Web README](./FlagExplorerApp.Web/README.md)

- **FlagExplorerApp.Application**: Contains business logic, query/command handlers, and DTOs.

- **FlagExplorerApp.Infrastructure**: Handles database access, external integrations, and repository implementations.

- **FlagExplorerApp.Domain**: Defines core entities, interfaces, and domain logic.

- **FlagExplorerApp.Tests**: Unit tests for application components.

- **FlagExplorerApp.IntegrationTests**: Integration tests for end-to-end scenarios.

- **CI-CD**: YML files configured to execute on github actions, this is in (./.github\workflows), file name ci-cd-api.yml for api and ci-cd-api-web.yml for web.

## Getting Started

Refer to the individual README files in the `FlagExplorerApp.Api` and `FlagExplorerApp.Web` folders for specific setup instructions for the API and frontend.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
