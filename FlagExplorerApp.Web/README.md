# Countries of the World

A modern Angular application that displays country flags and detailed information using the REST Countries API.

## Features

- **Home Screen**: Displays all country flags in a responsive grid layout
- **Detail Screen**: Shows detailed information about a selected country, including:
  - Country name (common and official)
  - Population
  - Capital
- **Error Handling**: Robust error handling with user-friendly messages and retry options
- **Responsive Design**: Fully responsive layout that works well on mobile, tablet, and desktop devices

## Technologies Used

- **Framework**: Angular 19.2.0
- **Testing**: Jest with Angular Testing Library
- **CI/CD**: GitHub Actions workflow for continuous integration and deployment
- **Code Quality**: ESLint for code linting and quality checks
- **Styling**: SCSS for advanced styling capabilities

## Getting Started

### Prerequisites

- Node.js (v18 or later recommended)
- npm (v9 or later recommended)

### Installation

1. Clone the repository:

   ```bash
   git clone <repository-url>
   cd countries-app
   ```

2. Install dependencies:

   ```bash
   npm install
   ```

3. Start the development server:

   ```bash
   npm start
   ```

4. Open your browser and navigate to `http://localhost:4200/`

## Development

### Project Structure

```
countries-app/
├── src/
│   ├── app/
│   │   ├── components/               # UI components
│   │   │   ├── home/                 # Home page with country grid
│   │   │   └── country-detail/       # Country detail page
│   │   ├── services/                 # API services
│   │   │   └── country.service.ts    # Service for API communication
│   │   ├── models/                   # Data models
│   │   │   └── country.ts            # Country interface
│   │   ├── app.component.ts          # Root component
│   │   ├── app.routes.ts             # Application routing
│   │   └── app.config.ts             # Application configuration
│   ├── styles.scss                   # Global styles
│   └── index.html                    # Main HTML file
├── .github/workflows/                # CI/CD configurations
│   └── ci-cd.yml                     # GitHub Actions workflow
├── jest.config.js                    # Jest configuration
└── eslint.config.js                  # ESLint configuration
```

### Available Scripts

- `npm start`: Starts the development server
- `npm run build`: Builds the application for production
- `npm test`: Runs the test suite
- `npm run test:watch`: Runs the test suite in watch mode
- `npm run test:coverage`: Generates test coverage report
- `npm run lint`: Runs ESLint to check for code quality issues

## Testing

This project uses Jest for unit and integration testing. Tests are located alongside their corresponding components.

Run the tests:

```bash
npm test
```

Generate a coverage report:

```bash
npm run test:coverage
```

## CI/CD

The project includes a GitHub Actions workflow for continuous integration and deployment. The workflow:

1. Builds the application
2. Runs linting checks
3. Executes all tests
4. Generates a test coverage report

## API Integration

This application integrates with the REST Countries API:

- All countries endpoint: `https://restcountries.com/v3.1/all`
- Country by code endpoint: `https://restcountries.com/v3.1/alpha/{code}`

## Deployment

The application can be deployed to various hosting platforms:

- **Firebase Hosting**: Uncomment and configure the Firebase deployment steps in the CI/CD workflow
- **Vercel**: Connect your GitHub repository to Vercel for automatic deployments
- **Netlify**: Connect your GitHub repository to Netlify for automatic deployments

## License

This project is licensed under the MIT License.
