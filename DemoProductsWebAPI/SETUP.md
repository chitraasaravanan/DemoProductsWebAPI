# Local Development Setup Guide

This guide helps you set up DemoProductsWebAPI for local development.

## Prerequisites

- **.NET 9 SDK** (or later)
  - Download: https://dotnet.microsoft.com/download/dotnet/9.0
  - Verify: `dotnet --version`

- **SQL Server**
  - Option 1: SQL Server LocalDB (lightweight, included with Visual Studio)
  - Option 2: Full SQL Server Express or Standard edition
  - Connection: See appsettings.json configuration below

- **Git**
  - Download: https://git-scm.com/
  - Verify: `git --version`

- **Visual Studio or Code Editor**
  - VS 2022 Community: https://visualstudio.microsoft.com/community/
  - VS Code: https://code.visualstudio.com/
  - JetBrains Rider: https://www.jetbrains.com/rider/

## Installation Steps

### 1. Clone Repository

```bash
git clone https://github.com/chitraasaravanan/DemoProductsWebAPI.git
cd DemoProductsWebAPI/DemoProductsWebAPI
```

### 2. Restore NuGet Packages

```bash
dotnet restore
```

This downloads all required dependencies defined in .csproj files.

### 3. Configure Connection Strings

Edit `DemoProductsWebAPI.API/appsettings.json`:

#### Option A: SQL Server LocalDB (Recommended for Development)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=DemoProductsWebAPI;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False"
  }
}
```

**Verify LocalDB is installed:**
```bash
sqllocaldb info
```

#### Option B: SQL Server Express

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=DemoProductsWebAPI;Integrated Security=true;Encrypt=True;Trust Server Certificate=True;"
  }
}
```

#### Option C: Use In-Memory Database (No Database Setup)

Leave the connection string empty or remove it:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": ""
  }
}
```

The app will automatically use an in-memory database for development.

### 4. Configure JWT Secret

Edit `DemoProductsWebAPI.API/appsettings.json` - Replace the placeholder JWT key:

```json
{
  "Jwt": {
    "Key": "your-super-secret-key-minimum-32-characters-long-12345678",
    "Issuer": "DemoProductsWebAPI",
    "Audience": "DemoProductsWebAPIUsers",
    "ExpireMinutes": 60
  }
}
```

⚠️ **Security Note**: Generate a strong key for production:
```bash
# PowerShell
[Convert]::ToBase64String((1..32 | ForEach-Object {[byte](Get-Random -Max 256)}))

# Bash
openssl rand -base64 32
```

### 5. Build Solution

```bash
dotnet build
```

Verify no compiler errors or warnings appear.

### 6. Run Migrations (If Using SQL Server)

**Initial database setup** (auto-runs on application startup, but you can manually run):

```bash
dotnet ef database update -p DemoProductsWebAPI.Infrastructure -s DemoProductsWebAPI.API
```

The database will be created automatically with all required tables:
- Product
- Item (ProductCart)
- RefreshToken

### 7. Run Tests (Optional but Recommended)

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test DemoProductsWebAPI.Tests

# Run with verbose output
dotnet test -v detailed
```

Expected result: ✅ All tests pass

### 8. Start the Application

```bash
dotnet run --project DemoProductsWebAPI.API
```

You should see output:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
```

The application is now running!

## Accessing the Application

### 1. Swagger UI (API Documentation)

Open in browser: **https://localhost:5001/swagger**

Features:
- View all API endpoints
- Test endpoints interactively
- See request/response schemas
- Authenticate with JWT tokens

### 2. Health Check

**GET** `https://localhost:5001/health`

Returns system health status.

### 3. Sample API Call

**Get All Products:**
```bash
curl -X GET "https://localhost:5001/api/v1/products" \
  -H "accept: application/json"
```

## Development Workflow

### Making Code Changes

1. **Edit code** in your editor
2. **Save file** (auto-rebuild if using IDE with live reload)
3. **Run application**: `dotnet run --project DemoProductsWebAPI.API`
4. **Test changes** in Swagger UI or via curl

### Running Tests While Developing

```bash
# Watch mode - re-runs tests on code changes
dotnet watch test
```

### Code Formatting

Format all code before committing:
```bash
dotnet format
```

This ensures consistent code style.

### Building for Release

```bash
# Build release version (optimized)
dotnet build -c Release

# Publish for deployment
dotnet publish -c Release -o ./publish
```

## Debugging

### Visual Studio

1. Open `DemoProductsWebAPI.sln`
2. Set breakpoints by clicking line numbers
3. Press **F5** or **Debug > Start Debugging**
4. Step through code with F10 (step over) or F11 (step into)

### VS Code

1. Install C# extension (powered by OmniSharp)
2. Open folder: `code .`
3. Create `.vscode/launch.json` or use command palette
4. Press **F5** to start debugging

### Command Line Debugging

```bash
# Run with debugging enabled
dotnet run --project DemoProductsWebAPI.API --no-hot-reload
```

## Common Issues & Solutions

### Issue: "LocalDB not found"
```bash
# Solution: Install or initialize LocalDB
sqllocaldb create
sqllocaldb start mssqllocaldb
```

### Issue: "Port 5001 already in use"
```bash
# Solution: Use different port
dotnet run --project DemoProductsWebAPI.API --urls="https://localhost:5002"
```

### Issue: "HTTPS certificate not trusted"
```bash
# Solution: Trust development certificate
dotnet dev-certs https --trust
```

### Issue: "NuGet package restore fails"
```bash
# Solution: Clear cache and retry
dotnet nuget locals all --clear
dotnet restore
```

### Issue: "EF Core migrations not applied"
```bash
# Solution: Manually update database
dotnet ef database update -p DemoProductsWebAPI.Infrastructure -s DemoProductsWebAPI.API
```

## Project Structure Reference

```
DemoProductsWebAPI/
├── DemoProductsWebAPI.API/              # REST API layer
│   ├── Controllers/                     # API endpoints
│   ├── appsettings.json                 # Configuration
│   ├── Program.cs                       # Startup configuration
│   └── Properties/launchSettings.json   # Launch configuration
│
├── DemoProductsWebAPI.Application/      # Business logic (CQRS)
│   ├── Commands/                        # Write operations
│   ├── Queries/                         # Read operations
│   └── Services/                        # Service implementations
│
├── DemoProductsWebAPI.Domain/           # Domain entities
│   └── Entities/                        # Business entities
│
├── DemoProductsWebAPI.Infrastructure/   # Data access
│   ├── Data/
│   │   ├── ApplicationDbContext.cs      # EF Core context
│   │   └── Repositories/                # Repository implementations
│   └── Extensions/                      # DI configuration
│
├── DemoProductsWebAPI.Common/           # Shared contracts
│   └── Interfaces/                      # Service interfaces
│
├── DemoWebAPI.Core/                     # Shared utilities
│   ├── Models/                          # Common models
│   └── Extensions/                      # Common extensions
│
└── DemoProductsWebAPI.Tests/            # Unit & integration tests
    └── UnitTestcase/                    # Test files
```

## Environment Variables (Production)

For production deployment, use environment variables instead of appsettings.json:

```bash
# JWT Configuration
export Jwt__Key="your-production-key"
export Jwt__Issuer="YourIssuer"

# Database
export ConnectionStrings__DefaultConnection="production-connection-string"

# Redis (optional)
export ConnectionStrings__RedisConnection="redis-server:6379"

# Logging
export Logging__LogLevel__Default="Warning"
```

## IDE Extensions (Recommended)

### Visual Studio
- **EditorConfig Language Service** - Consistent code style
- **Microsoft Visual C# Tools**
- **Roslyn Analyzers**

### VS Code
- **C#** (OmniSharp)
- **C# Extensions** (kreativ_labs)
- **.NET Core Test Explorer**

## Additional Resources

- **Official .NET Docs**: https://docs.microsoft.com/dotnet/
- **Entity Framework Core**: https://docs.microsoft.com/ef/core/
- **ASP.NET Core**: https://docs.microsoft.com/aspnet/core/
- **JWT Authentication**: https://jwt.io/
- **Swagger/OpenAPI**: https://swagger.io/

## Getting Help

- 📖 Check [README.md](README.md) for architecture details
- 🤝 See [CONTRIBUTING.md](CONTRIBUTING.md) for contribution guidelines
- 🐛 Open an issue on GitHub for bugs
- 💬 Start a discussion for questions

---

**Ready to develop?** Start with: `dotnet run --project DemoProductsWebAPI.API`

Happy coding! 🚀
