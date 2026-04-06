# Quick Reference Guide

Fast reference for common tasks and commands.

## 🚀 Quick Start

```bash
# Clone repo
git clone https://github.com/chitraasaravanan/DemoProductsWebAPI.git
cd DemoProductsWebAPI/DemoProductsWebAPI

# Setup & run
dotnet restore
dotnet build
dotnet run --project DemoProductsWebAPI.API

# Access API
# Swagger: https://localhost:5001/swagger
# Health: https://localhost:5001/health
```

## 📚 Key Files

| File | Purpose |
|------|---------|
| `README.md` | Project overview & architecture |
| `SETUP.md` | Local development setup |
| `CONTRIBUTING.md` | Contribution guidelines |
| `CLEANUP_SUMMARY.md` | Recent refactoring details |
| `GITHUB_READY.md` | GitHub readiness checklist |

## 🛠️ Common Commands

### Build & Run
```bash
# Restore dependencies
dotnet restore

# Build solution
dotnet build

# Build release version
dotnet build -c Release

# Run application
dotnet run --project DemoProductsWebAPI.API

# Run with specific port
dotnet run --project DemoProductsWebAPI.API --urls="https://localhost:5002"
```

### Testing
```bash
# Run all tests
dotnet test

# Run specific test class
dotnet test --filter "FullyQualifiedName~ProductHandlersUnitTestcase"

# Run with coverage
dotnet test /p:CollectCoverage=true

# Watch mode (re-run on changes)
dotnet watch test
```

### Code Quality
```bash
# Format code
dotnet format

# Check formatting only
dotnet format --verify-no-changes

# Analyze code
dotnet build --no-incremental -p:EnforceCodeStyleInBuild=true
```

### Database
```bash
# Create/update database
dotnet ef database update -p DemoProductsWebAPI.Infrastructure -s DemoProductsWebAPI.API

# Add migration
dotnet ef migrations add "MigrationName" -p DemoProductsWebAPI.Infrastructure -s DemoProductsWebAPI.API

# Remove last migration
dotnet ef migrations remove -p DemoProductsWebAPI.Infrastructure -s DemoProductsWebAPI.API

# Script database
dotnet ef migrations script -p DemoProductsWebAPI.Infrastructure
```

### Deployment
```bash
# Publish for deployment
dotnet publish -c Release -o ./publish

# Create Docker image (if Dockerfile exists)
docker build -t demoproductswebapi:latest .

# Run Docker container
docker run -p 5001:5001 demoproductswebapi:latest
```

## 🔐 Security Setup

### JWT Secret Generation
```powershell
# PowerShell
[Convert]::ToBase64String((1..32 | ForEach-Object {[byte](Get-Random -Max 256)}))
```

```bash
# Bash
openssl rand -base64 32
```

Update in `appsettings.json`:
```json
{
  "Jwt": {
    "Key": "YOUR_GENERATED_KEY_HERE"
  }
}
```

## 📡 API Quick Reference

### Authentication
```bash
# Get access token
curl -X POST "https://localhost:5001/api/v1/auth/token" \
  -H "Content-Type: application/json" \
  -d '{"username":"demo","password":"demo"}'

# Use token
curl -X GET "https://localhost:5001/api/v1/products" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

### Products
```bash
# Get all products
GET /api/v1/products

# Get product by ID
GET /api/v1/products/1

# Create product
POST /api/v1/products
{
  "productName": "Widget",
  "createdBy": "user@example.com"
}

# Update product
PUT /api/v1/products/1
{
  "id": 1,
  "productName": "Updated Widget",
  "createdBy": "user@example.com"
}

# Delete product
DELETE /api/v1/products/1
```

### Health
```bash
# Check API health
GET /health
```

## 🗂️ Project Structure Quick Reference

```
├── DemoProductsWebAPI.API/         ← REST endpoints
├── DemoProductsWebAPI.Application/ ← Business logic (CQRS)
├── DemoProductsWebAPI.Domain/      ← Entities
├── DemoProductsWebAPI.Infrastructure/ ← Data access
├── DemoProductsWebAPI.Common/      ← Shared interfaces
├── DemoWebAPI.Core/                ← Shared utilities
└── DemoProductsWebAPI.Tests/       ← Unit & integration tests
```

## 🐛 Debugging

### Visual Studio
```
1. Set breakpoint (click line number)
2. Press F5 to start debugging
3. F10 = step over
4. F11 = step into
5. Shift+F11 = step out
```

### VS Code
```
1. Install C# extension
2. Press F5 to start debugging
3. Select .NET Core environment
4. Set breakpoints and step through
```

### Command Line
```bash
# Run with debugging
dotnet run --project DemoProductsWebAPI.API --no-hot-reload

# Verbose logging
dotnet run --project DemoProductsWebAPI.API --verbosity diagnostic
```

## 📊 Architecture Quick Overview

```
API Layer (Controllers)
    ↓
Application Layer (CQRS Commands/Queries)
    ↓
Domain Layer (Entities & Business Logic)
    ↓
Infrastructure Layer
    ├─ Repositories (Data access)
    ├─ Unit of Work (Transaction management)
    ├─ DbContext (EF Core)
    └─ Read Services (Dapper optimized)
```

## 🔄 Development Workflow

1. **Create feature branch**
   ```bash
   git checkout -b feature/my-feature
   ```

2. **Make changes**
   ```bash
   # Edit code
   ```

3. **Format & test**
   ```bash
   dotnet format
   dotnet test
   ```

4. **Commit**
   ```bash
   git add .
   git commit -m "feat: add my feature"
   ```

5. **Push & PR**
   ```bash
   git push origin feature/my-feature
   # Create PR on GitHub
   ```

## 🚨 Common Issues

| Issue | Solution |
|-------|----------|
| Port 5001 in use | `dotnet run --urls="https://localhost:5002"` |
| LocalDB not found | `sqllocaldb start mssqllocaldb` |
| HTTPS cert error | `dotnet dev-certs https --trust` |
| Package restore fails | `dotnet nuget locals all --clear && dotnet restore` |
| Test failures | `dotnet test -v detailed` |

## 📖 Documentation Links

- **MS Learn**: https://docs.microsoft.com/learn/
- **ASP.NET Core**: https://docs.microsoft.com/aspnet/core/
- **Entity Framework Core**: https://docs.microsoft.com/ef/core/
- **MediatR**: https://github.com/jbogard/MediatR/wiki
- **xUnit**: https://xunit.net/docs/getting-started
- **Swagger**: https://swagger.io/

## 🤝 Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md) for:
- Code style guidelines
- Testing requirements
- Commit message format
- PR process

## ❓ Getting Help

- 📖 Check [README.md](README.md)
- 🔧 Check [SETUP.md](SETUP.md)
- 🤝 See [CONTRIBUTING.md](CONTRIBUTING.md)
- 🐛 Open GitHub issue
- 💬 Start GitHub discussion

---

**Last Updated**: 2024
**Maintained By**: Development Team
