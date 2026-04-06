# Contributing to DemoProductsWebAPI

Thank you for your interest in contributing to DemoProductsWebAPI! This document provides guidelines and instructions for contributing.

## Code of Conduct

- Be respectful and inclusive
- Welcome people of all backgrounds and experience levels
- Focus on constructive feedback
- Report inappropriate behavior to project maintainers

## Getting Started

### Prerequisites
- .NET 9 SDK or later
- SQL Server (LocalDB or full edition)
- Visual Studio 2022+ or VS Code
- Git

### Local Development Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/chitraasaravanan/DemoProductsWebAPI.git
   cd DemoProductsWebAPI
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Configure local settings**
   - Update `DemoProductsWebAPI.API/appsettings.json` with your connection strings
   - Replace JWT Key with a strong secret (minimum 32 characters)

4. **Run migrations**
   ```bash
   dotnet ef database update -p DemoProductsWebAPI.Infrastructure -s DemoProductsWebAPI.API
   ```

5. **Run the application**
   ```bash
   dotnet run --project DemoProductsWebAPI.API
   ```

6. **Run tests**
   ```bash
   dotnet test
   ```

## Development Workflow

### Branch Naming
- `feature/short-description` for new features
- `bugfix/short-description` for bug fixes
- `docs/short-description` for documentation
- `refactor/short-description` for refactoring

### Commit Messages
Follow conventional commits:
- `feat: add new feature`
- `fix: resolve issue`
- `docs: update documentation`
- `style: fix formatting`
- `refactor: improve code quality`
- `test: add/update tests`

### Code Style

We follow C# and .NET conventions:

1. **Naming**
   - Classes: PascalCase
   - Methods: PascalCase
   - Properties: PascalCase
   - Private fields: _camelCase
   - Local variables: camelCase

2. **Formatting**
   - Use `dotnet format` before committing
   - 4 spaces for indentation
   - Line length: prefer under 120 characters

3. **Documentation**
   - Add XML comments to public methods/classes
   - Use `///` for documentation comments
   - Include parameter, return, and exception documentation

### Code Quality

- **No compiler warnings** - Fix all warnings before submitting PR
- **100% test coverage for new code** - Add unit tests
- **No hardcoded secrets** - Use configuration files
- **SOLID principles** - Follow SOLID design principles
- **Clean code** - Keep methods focused and small

## Pull Request Process

1. **Create a feature branch**
   ```bash
   git checkout -b feature/my-feature
   ```

2. **Make your changes**
   - Implement the feature
   - Add tests
   - Update documentation
   - Run `dotnet format`

3. **Commit your changes**
   ```bash
   git add .
   git commit -m "feat: add my new feature"
   ```

4. **Push to GitHub**
   ```bash
   git push origin feature/my-feature
   ```

5. **Create a Pull Request**
   - Provide a clear description
   - Reference related issues (#issue-number)
   - Include screenshots/demos if relevant
   - Ensure all checks pass

## PR Review Guidelines

A PR should:
- [ ] Have a clear title and description
- [ ] Have no conflicts with `main` branch
- [ ] Include tests for new functionality
- [ ] Have no compiler warnings
- [ ] Follow code style guidelines
- [ ] Include XML documentation for public APIs
- [ ] Update README if behavior changes

## Testing

### Running Tests
```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test DemoProductsWebAPI.Tests

# Run with coverage
dotnet test /p:CollectCoverage=true
```

### Writing Tests

1. **Unit Tests** - Test individual components
   ```csharp
   [Fact]
   public async Task MethodName_Scenario_ExpectedResult()
   {
       // Arrange
       var input = new TestData();
       
       // Act
       var result = await _service.MethodName(input);
       
       // Assert
       result.Should().NotBeNull();
   }
   ```

2. **Integration Tests** - Test with real dependencies
   - Use in-memory databases
   - Create realistic test data
   - Verify end-to-end behavior

## Documentation

When contributing, please update:
- `README.md` for user-facing changes
- `CONTRIBUTING.md` for process changes
- Code comments for complex logic
- XML documentation for public APIs

## Security

- **Never commit secrets** (API keys, connection strings, passwords)
- Use `appsettings.json` for local development only
- Use environment variables or Key Vault for production
- Report security issues privately to maintainers

## Architecture Guidelines

This project follows:
- **Layered Architecture** (API, Application, Domain, Infrastructure)
- **CQRS Pattern** (Commands and Queries separated)
- **Repository Pattern** for data access
- **Dependency Injection** for loose coupling
- **Unit of Work** for transaction management

When adding new features:
1. Create domain entities in `Domain` layer
2. Add application services in `Application` layer
3. Create repositories in `Infrastructure` layer
4. Expose via controllers in `API` layer

## Asking Questions

- Check existing issues and documentation first
- Use GitHub Discussions for questions
- Be specific and include error messages/screenshots
- Provide minimal reproducible examples

## Recognition

Contributors will be recognized in:
- GitHub contributors page
- Release notes
- Hall of Fame (major contributors)

## Questions or Feedback?

- Open an issue for bug reports
- Start a discussion for suggestions
- Contact maintainers for security concerns

Thank you for contributing to DemoProductsWebAPI! 🚀
