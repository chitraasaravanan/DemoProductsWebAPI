RESTful Backend API Solution - Technical Assessment

## Overview
This document outlines the approach to implementing a RESTful backend API solution as requested in the technical assessment. The solution is designed with scalability, maintainability, and industry best practices in mind, utilizing modern .NET architecture patterns including CQRS, MediatR, and a multi-layer architecture.

## Problem Statement
Design a RESTful API solution around Products to perform CRUD operations, with complementary ProductCart management, user authentication, and optimized read operations.

## Test Submission
1. Do not submit/upload your code in this repository.
2. Create your own public repo and share link with us.

---

## Tech Stack
- **Framework**: .NET 9 with C# 13.0
- **API Framework**: ASP.NET Core Web API with API Versioning
- **Database**: SQL Server with Entity Framework Core (Code-First approach)
- **Read Optimization**: Dapper for high-performance read-only queries
- **Caching**: Redis for distributed caching of read operations
- **Authentication**: JWT Bearer tokens with Refresh Token rotation strategy
- **CQRS Pattern**: MediatR for Command/Query separation and handler discovery
- **Mapping**: AutoMapper for DTO/Entity transformations
- **Validation**: FluentValidation for comprehensive input validation
- **Testing**: xUnit, Moq, and FluentAssertions for unit/integration tests
- **Documentation**: Swagger/OpenAPI with Swashbuckle
- **Logging**: Serilog with structured logging and console output
- **Resilience**: Polly for retry policies and resilience patterns
- **Rate Limiting**: Built-in ASP.NET Core rate limiting middleware
- **Health Checks**: ASP.NET Core Health Checks

---

## Solution Architecture

### Architecture Overview
The solution follows a **Layered N-Tier Architecture** with **CQRS (Command Query Responsibility Segregation)** pattern:

```
┌─────────────────────────────────────────────────┐
│           Presentation Layer (API)              │
│    Controllers | Middleware | Filters           │
└────────────────┬────────────────────────────────┘
                 │
┌────────────────▼────────────────────────────────┐
│        Application Layer (CQRS)                 │
│  Commands | Queries | Handlers | Services       │
│  MediatR Pipeline | AutoMapper | Validators     │
└────────────────┬────────────────────────────────┘
                 │
┌────────────────▼────────────────────────────────┐
│         Domain Layer (Business Logic)           │
│      Entities | DTOs | Interfaces               │
└────────────────┬────────────────────────────────┘
                 │
┌────────────────▼────────────────────────────────┐
│      Infrastructure Layer (Data Access)         │
│  EF Core DbContext | Unit of Work | Dapper      │
│  Repositories | Services | Read Operations      │
└─────────────────────────────────────────────────┘
```

### Layer Responsibilities

#### 1. **API Layer (Presentation)**
- HTTP request handling and response formatting
- Controller actions for resource-oriented endpoints
- Middleware for cross-cutting concerns (exception handling, logging, rate limiting)
- Authentication/Authorization filters
- API versioning and Swagger documentation
- CORS policy enforcement

#### 2. **Application Layer (CQRS & Business Logic)**
- **Commands**: Encapsulate write operations (Create, Update, Delete)
- **Queries**: Encapsulate read operations with optimized data retrieval
- **Handlers**: Process commands/queries through MediatR pipeline
- **Services**: Business logic orchestration and domain operations
- **DTOs**: Data transfer objects for API contracts
- **Validators**: FluentValidation rules for request validation
- **Mapping**: AutoMapper profiles for entity-to-DTO transformations
- **Notifications**: Domain events published after successful commands

#### 3. **Domain Layer**
- Core business entities (Product, ProductCart, RefreshToken)
- Domain interfaces and contracts
- Enumerations and value objects
- Business rules and validation constraints

#### 4. **Infrastructure Layer**
- Entity Framework Core DbContext and entity configurations
- Unit of Work pattern for transaction management
- Repository implementations for data access
- Dapper-based read services for optimized queries
- Token generation and validation services
- Connection factory for database connections
- Dependency injection configuration


## Project Structure

```
DemoProductsWebAPI/
├── DemoProductsWebAPI.API/                    # ASP.NET Core Web API
│   ├── Controllers/                           # API Controllers (REST endpoints)
│   │   ├── ProductsController.cs
│   │   ├── ProductCartsController.cs
│   │   └── AuthController.cs
│   ├── Extensions/
│   │   ├── ServiceCollectionExtensions.cs     # DI registration for read services
│   │   └── SwaggerExtensions.cs               # Swagger/OpenAPI configuration
│   ├── Middleware/
│   │   └── ExceptionHandlingMiddleware.cs     # Global exception handler
│   ├── Program.cs                             # Application startup configuration
│   ├── appsettings.json                       # Configuration (DB, JWT, Redis, etc.)
│   └── README.md
│
├── DemoProductsWebAPI.Application/            # Application/CQRS Layer
│   ├── Products/
│   │   ├── Commands/                          # Write operations
│   │   │   ├── CreateProductCommand.cs
│   │   │   ├── UpdateProductCommand.cs
│   │   │   └── DeleteProductCommand.cs
│   │   ├── Queries/                           # Read operations
│   │   │   ├── GetAllProductsQuery.cs
│   │   │   └── GetProductByIdQuery.cs
│   │   ├── Handlers/
│   │   │   ├── ProductCommandHandler.cs       # Handles all product commands
│   │   │   └── ProductQueryHandler.cs         # Handles all product queries
│   │   └── Notifications/
│   │       └── ProductCreatedNotification.cs  # Domain events
│   ├── ProductCarts/                          # Similar structure for ProductCarts
│   ├── Auth/                                  # Authentication commands/handlers
│   ├── Services/
│   │   ├── ProductService.cs                  # Business logic orchestration
│   │   └── ProductCartService.cs
│   ├── DTOs/                                  # Data Transfer Objects
│   │   ├── ProductDto.cs
│   │   └── ProductCartDto.cs
│   ├── Mapping/
│   │   └── ProductProfile.cs                  # AutoMapper profiles
│   └── Extensions/
│       └── ServiceCollectionExtensions.cs     # DI registration
│
├── DemoProductsWebAPI.Domain/                 # Domain/Business Logic Layer
│   ├── Entities/
│   │   ├── Product.cs                         # Product entity
│   │   ├── ProductCart.cs                     # Shopping cart item entity
│   │   └── RefreshToken.cs                    # Token entity
│   └── (No interfaces - kept minimal)
│
├── DemoProductsWebAPI.Infrastructure/         # Infrastructure/Data Access Layer
│   ├── Data/
│   │   ├── ApplicationDbContext.cs             # EF Core DbContext
│   │   ├── UnitOfWork.cs                      # Transaction management
│   │   ├── Repositories/
│   │   │   ├── ProductRepository.cs
│   │   │   ├── ProductCartRepository.cs
│   │   │   └── RefreshTokenRepository.cs
│   │   └── Read/                              # Dapper-based read optimization
│   │       ├── IDbConnectionFactory.cs
│   │       ├── SqlConnectionFactory.cs
│   │       ├── IDapperExecutor.cs
│   │       ├── DapperExecutor.cs
│   │       └── ProductReadService.cs
│   ├── Services/
│   │   └── TokenService.cs                    # JWT token generation/validation
│   ├── Extensions/
│   │   └── ServiceCollectionExtensions.cs     # DI registration
│
├── DemoProductsWebAPI.Common/                 # Shared Interfaces & DTOs
│   ├── Interfaces/
│   │   ├── IUnitOfWork.cs
│   │   ├── IProductRepository.cs
│   │   ├── IProductService.cs
│   │   ├── IProductReadService.cs
│   │   ├── ITokenService.cs
│   │   └── (Other repository/service interfaces)
│   ├── DTOs/
│   │   └── (DTO definitions referenced across layers)
│
├── DemoProductsWebAPI.Tests/                  # Unit & Integration Tests
│   ├── UnitTestcase/
│   │   ├── ProductHandlersUnitTestcase.cs
│   │   ├── ProductServiceUnitTestcase.cs
│   │   └── ProductsControllerUnitTests.cs
│
└── DemoWebAPI.Core/                           # Shared utilities & extensions
    ├── DTOs/
    │   └── JwtSettings.cs
    └── Extensions/
        └── (Resilience, HTTP client factories)
```

---

## Entity Framework Core Implementation

### Overview
Entity Framework Core (EF Core) is used for write operations and transactional consistency. The implementation follows **Code-First** approach with fluent configurations.

### Key Components

#### 1. **ApplicationDbContext**
```csharp
public class ApplicationDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductCart> ProductCarts { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Fluent API configurations for:
        // - Table mappings
        // - Primary keys and indexes
        // - Column constraints
        // - Foreign key relationships
        // - Cascade delete behaviors
    }
}
```

#### 2. **Entity Configurations**
- **Product Entity**: Mapped to `Product` table with indexed `ProductName` for fast lookups
- **ProductCart Entity**: Mapped to `Item` table (legacy table name) with cascade delete for product relationship
- **RefreshToken Entity**: Mapped to `RefreshToken` table for token rotation management

#### 3. **Database Context Setup (Program.cs)**
```csharp
// Connection pooling for better throughput (128 connections for SQL Server, 8 for InMemory)
builder.Services.AddDbContextPool<ApplicationDbContext>(options =>
    options.UseSqlServer(conn, sqlOptions => sqlOptions.CommandTimeout(30))
           .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking),
    poolSize: 128);
```

#### 4. **Unit of Work Pattern**
Implements transaction management across multiple repositories:
```csharp
public class UnitOfWork : IUnitOfWork
{
    public IProductRepository Products { get; }
    public IProductCartRepository ProductCarts { get; }
    public IRefreshTokenRepository RefreshTokens { get; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    public async Task ExecuteInTransactionAsync(Func<Task> operation)
}
```

#### 5. **Repository Pattern**
Each repository encapsulates data access for specific entity types:
```csharp
public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(Product entity, CancellationToken cancellationToken = default);
    void Update(Product entity);
    void Delete(Product entity);
}
```

#### 6. **NoTracking for Read Operations**
EF Core is configured with `NoTracking` behavior to improve read performance:
- No change tracking overhead
- Reduced memory consumption
- Faster query execution

#### 7. **Query Tracking Behavior**
- **Write Operations (Commands)**: Use default tracking for change detection
- **Read Operations (Queries)**: Use `AsNoTracking()` for better performance

### Performance Optimizations
- Database context pooling (reuses DbContext instances)
- Connection string timeout: 30 seconds
- Query tracking disabled for read-only scenarios
- Indexed columns on frequently searched fields (ProductName)
- Cascade delete configured for data integrity

### Database Migrations
- Located in `DemoProductsWebAPI.Infrastructure` project
- Applied automatically on application startup
- Support for both SQL Server (production) and InMemory (testing/development)

---

## Dapper Implementation

### Overview
Dapper is used exclusively for **read-only optimized queries**, enabling:
- Direct SQL execution without ORM overhead
- Reduced latency for high-traffic read operations
- Minimal memory footprint
- Seamless parameter binding and auto-mapping

### Architecture Components

#### 1. **IDbConnectionFactory**
Manages database connection creation and lifecycle:
```csharp
public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}

public class SqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}
```
- Abstraction over connection management
- Easy to swap for different databases
- Connection pooling handled by ADO.NET

#### 2. **IDapperExecutor**
Executes parameterized Dapper queries:
```csharp
public interface IDapperExecutor
{
    Task<IEnumerable<T>> QueryAsync<T>(IDbConnection conn, CommandDefinition command);
    Task<T?> QuerySingleOrDefaultAsync<T>(IDbConnection conn, CommandDefinition command);
}
```
- Wraps Dapper's query methods
- Ensures proper async/await usage
- Handles connection state management

#### 3. **ProductReadService**
Implements `IProductReadService` with optimized Dapper queries:
```csharp
public class ProductReadService : IProductReadService
{
    public async Task<IEnumerable<ProductDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var sql = "SELECT Id, ProductName, CreatedBy, CreatedOn FROM Product";
        return await _executor.QueryAsync<ProductDto>(conn, command);
    }

    public async Task<ProductDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var sql = "SELECT Id, ProductName, CreatedBy, CreatedOn FROM Product WHERE Id = @Id";
        return await _executor.QuerySingleOrDefaultAsync<ProductDto>(conn, command);
    }
}
```

### Read Query Optimization

#### SQL Queries Used:
1. **GetAll Products**:
   ```sql
   SELECT Id, ProductName, CreatedBy, CreatedOn, ModifiedBy, ModifiedOn FROM Product
   ```
   - Projects only needed columns (no unused fields)
   - Fast full table scan with minimal data transfer

2. **GetById Product**:
   ```sql
   SELECT Id, ProductName, CreatedBy, CreatedOn, ModifiedBy, ModifiedOn FROM Product WHERE Id = @Id
   ```
   - Uses parameterized query to prevent SQL injection
   - Leverages primary key index for O(1) lookup

### Performance Benefits
- **Lower Latency**: ~5-10x faster than ORM for read operations
- **Reduced Memory**: No change tracking or proxy objects
- **Better Control**: Direct SQL optimization options (hints, indexes)
- **Scalability**: Handles high read-volume scenarios efficiently

### Read Cache Layer (Redis)
Output caching (ASP.NET Core 7.1+) caches query results:
```csharp
[HttpGet]
[OutputCache(Duration = 300)] // 5-minute cache
public async Task<IActionResult> Get()
{
    var results = await _mediator.Send(new GetAllProductsQuery());
    return Ok(results);
}
```
- Configured via Redis connection string
- Automatic cache invalidation on product updates
- Reduces database load for frequently accessed data

### Comparison: EF Core vs Dapper

| Aspect | EF Core (Write) | Dapper (Read) |
|--------|-----------------|---------------|
| **Use Case** | CRUD operations, transactions | Optimized read queries |
| **Overhead** | Change tracking, lazy loading | Minimal |
| **Performance** | Good for complex logic | Excellent for simple queries |
| **Memory** | Higher | Lower |
| **Flexibility** | Limited by abstraction | Full SQL control |
| **Type Safety** | Full IntelliSense | DTO mapping |

---

## Authentication & Authorization

### JWT (JSON Web Token) Strategy

#### 1. **Token Architecture**

**Access Token**:
- Short-lived (default: 15 minutes)
- Contains claims: `NameIdentifier` (UserId)
- Algorithm: HMAC-SHA256
- Issued with: Issuer, Audience, Expiration

**Refresh Token**:
- Long-lived (typically 7 days)
- Cryptographically secure random 64-byte value
- Base64 encoded for storage
- Persisted in `RefreshToken` table with UserId and expiration

#### 2. **Token Generation Service (ITokenService)**

```csharp
public class TokenService : ITokenService
{
    // Generate Access Token
    public string GenerateAccessToken(string userId)
    {
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userId) };
        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_settings.ExpireMinutes),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );
        return tokenHandler.WriteToken(token);
    }

    // Generate Refresh Token
    public string GenerateRefreshToken()
    {
        var bytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

    // Validate Token
    public bool ValidateAccessToken(string token, out string? userId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            tokenHandler.ValidateToken(token, parameters, out SecurityToken validatedToken);
            userId = ((JwtSecurityToken)validatedToken).Claims
                .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            return true;
        }
        catch
        {
            userId = null;
            return false;
        }
    }
}
```

#### 3. **JWT Configuration (appsettings.json)**
```json
{
    "Jwt": {
        "Key": "your-secret-key-minimum-32-characters",
        "Issuer": "DemoProductsWebAPI",
        "Audience": "DemoProductsAPIClients",
        "ExpireMinutes": 15
    }
}
```

#### 4. **Authentication Pipeline (Program.cs)**

```csharp
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;
    var jwt = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwt.Issuer,
        ValidAudience = jwt.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key))
    };
});
```

#### 5. **Authentication Flow**

```
┌─────────────────┐
│   Client App    │
└────────┬────────┘
         │
         ├─ 1. POST /auth/authenticate (username, password)
         │
         ▼
┌────────────────────────────┐
│   AuthenticateHandler      │
│   (MediatR Command)        │
└────────┬───────────────────┘
         │
         ├─ 2. Validate credentials (against User/Identity store)
         ├─ 3. If valid: Generate Access Token + Refresh Token
         ├─ 4. Save Refresh Token to database
         │
         ▼
┌────────────────────────────┐
│   AuthResult               │
│   - AccessToken            │
│   - RefreshToken           │
│   - ExpiresIn              │
└────────┬───────────────────┘
         │
         │ 5. Return to client
         ▼
┌─────────────────┐
│   Client App    │
│ Stores tokens   │
└─────────────────┘
```

#### 6. **Token Usage in API Requests**

```http
GET /api/v1/products HTTP/1.1
Host: api.example.com
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

#### 7. **Refresh Token Flow**

```
Client has:
- Access Token (expired/expiring)
- Refresh Token (valid)

┌────────────────────────────┐
│  POST /auth/refresh-token  │
│  Body: { refreshToken }    │
└────────┬───────────────────┘
         │
         ▼
┌────────────────────────────┐
│ RefreshTokenHandler        │
│ (MediatR Command)          │
└────────┬───────────────────┘
         │
         ├─ Validate refresh token from database
         ├─ Check expiration
         ├─ Generate new Access Token
         ├─ (Optional) Rotate Refresh Token
         │
         ▼
┌────────────────────────────┐
│   New AuthResult           │
│   - New Access Token       │
│   - New Refresh Token      │
└────────┬───────────────────┘
         │
         │ Return new tokens
         ▼
┌─────────────────┐
│   Client App    │
│ Updates tokens  │
└─────────────────┘
```

#### 8. **Security Best Practices Implemented**

- **HTTPS Only**: `RequireHttpsMetadata = true`
- **Signature Validation**: All tokens verified with signing key
- **Expiration Checks**: Tokens expire and require refresh
- **Issuer/Audience Validation**: Ensures tokens from trusted sources
- **Secure Key Storage**: JWT key stored in configuration/Key Vault
- **Token Rotation**: Refresh tokens rotated on renewal (optional)
- **CORS Protection**: Configured in middleware
- **Input Validation**: All auth inputs validated with FluentValidation

#### 9. **Protected Endpoints Example**

```csharp
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize] // Requires valid JWT token
public class ProductsController : ControllerBase
{
    [HttpPost]
    [Authorize] // Can specify roles if needed
    public async Task<IActionResult> Create([FromBody] ProductDto dto)
    {
        var command = new CreateProductCommand(dto);
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("{id}")]
    [AllowAnonymous] // Optional: allow public access to reads
    public async Task<IActionResult> GetById(int id)
    {
        var query = new GetProductByIdQuery(id);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
```

---

## CQRS & MediatR Pattern

### Command Query Responsibility Segregation

#### Commands (Write Operations)
- Modify state (Create, Update, Delete)
- Return result or void
- Must be validated before execution
- Published to event handlers on success

```csharp
public class CreateProductCommand : IRequest<ProductDto>
{
    public ProductDto Product { get; init; }
}

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // 1. Validate input
        // 2. Create entity
        // 3. Persist via Unit of Work
        // 4. Publish ProductCreatedNotification
        // 5. Return mapped DTO
    }
}
```

#### Queries (Read Operations)
- Never modify state
- Return data without side effects
- Optimized for speed and memory efficiency

```csharp
public class GetAllProductsQuery : IRequest<IEnumerable<ProductDto>>
{
}

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, IEnumerable<ProductDto>>
{
    public async Task<IEnumerable<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        // Use IProductReadService (Dapper-based) for optimal performance
        return await _readService.GetAllAsync(cancellationToken);
    }
}
```

### MediatR Pipeline Benefits
- **Separation of Concerns**: Commands and Queries logically separated
- **Handler Discovery**: Automatic registration and invocation
- **Testability**: Easy to mock and test handlers independently
- **Extensibility**: Pre/post behaviors via pipeline behaviors
- **Single Responsibility**: Each handler has one job

---

## API Design

### Resource-Oriented Endpoints

```http
# Get all products
GET /api/v1/products

# Get product by ID
GET /api/v1/products/{id}

# Create product
POST /api/v1/products
Content-Type: application/json
Authorization: Bearer <token>
{
    "productName": "Laptop",
    "createdBy": "user@example.com"
}

# Update product
PUT /api/v1/products/{id}
Content-Type: application/json
Authorization: Bearer <token>
{
    "productName": "Updated Laptop",
    "modifiedBy": "user@example.com"
}

# Delete product
DELETE /api/v1/products/{id}
Authorization: Bearer <token>

# Get all product carts
GET /api/v1/productcarts

# Get product cart by ID
GET /api/v1/productcarts/{id}

# Create product cart
POST /api/v1/productcarts
Authorization: Bearer <token>
{
    "productId": 1,
    "quantity": 5
}

# Update product cart
PUT /api/v1/productcarts/{id}
Authorization: Bearer <token>
{
    "quantity": 10
}

# Delete product cart
DELETE /api/v1/productcarts/{id}
Authorization: Bearer <token>
```

### API Versioning
- Configured with `ApiVersioning` middleware
- Version specified in URL path: `/api/v{version}/resource`
- Current version: 1.0
- Supports multiple versions simultaneously for backward compatibility

### Response Format
- **Success (200/201)**: Returns resource data
- **No Content (204)**: For DELETE operations
- **Bad Request (400)**: Validation errors with details
- **Unauthorized (401)**: Missing/invalid token
- **Forbidden (403)**: Insufficient permissions
- **Not Found (404)**: Resource doesn't exist
- **Server Error (500)**: Unhandled exceptions with correlation ID

### Error Response Format
```json
{
    "type": "https://example.com/errors/validation-error",
    "title": "One or more validation errors occurred.",
    "status": 400,
    "detail": "See errors property for details.",
    "errors": {
        "ProductName": ["The ProductName field is required."],
        "CreatedBy": ["The CreatedBy field must not be empty."]
    },
    "traceId": "0HN1GPBQ3:00000001"
}
```

---

## Middleware & Cross-Cutting Concerns

### 1. **Exception Handling Middleware**
```csharp
public class ExceptionHandlingMiddleware
{
    // Catches all unhandled exceptions
    // Logs error details via Serilog
    // Returns standardized error response
    // Includes correlation ID for tracing
}
```

### 2. **Request/Response Logging**
```csharp
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Handling {method} {path}", context.Request.Method, context.Request.Path);
    await next();
    logger.LogInformation("Handled {status}", context.Response.StatusCode);
});
```

### 3. **Rate Limiting**
- Per-IP fixed window (100 requests per minute)
- Configurable via middleware
- Prevents abuse and DDoS attacks

### 4. **CORS Policy**
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", p => p
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod());
});
```

### 5. **Output Caching**
- Caches read operation results
- Duration configurable per endpoint
- Redis-backed for distributed scenarios
- Automatically invalidated on writes

### 6. **Response Compression**
- Reduces bandwidth usage
- Transparent to clients
- Configurable compression algorithms

---

## Database Schema

```sql
-- Products Table
CREATE TABLE [dbo].[Product]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY (1,1),
    [ProductName] NVARCHAR(255) NOT NULL,
    [CreatedBy] NVARCHAR(100) NOT NULL,
    [CreatedOn] DATETIME NOT NULL,
    [ModifiedBy] NVARCHAR(100) NULL,
    [ModifiedOn] DATETIME NULL,
    INDEX IX_Product_ProductName (ProductName)
);

-- Product Carts (mapped to Item table)
CREATE TABLE [dbo].[Item]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY (1,1),
    [ProductId] INT NOT NULL FOREIGN KEY REFERENCES Product(Id) ON DELETE CASCADE,
    [Quantity] INT NOT NULL
);

-- Refresh Tokens Table
CREATE TABLE [dbo].[RefreshToken]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY (1,1),
    [UserId] NVARCHAR(255) NOT NULL,
    [Token] NVARCHAR(MAX) NOT NULL,
    [ExpiresOn] DATETIME NOT NULL
);
```

---

## Testing Strategy

### Unit Tests
- **Framework**: xUnit
- **Mocking**: Moq
- **Assertions**: FluentAssertions
- **Coverage**: Handlers, Services, Repositories

### Integration Tests
- In-memory database for isolation
- Real Unit of Work instance
- Mocked external services
- Full request/response pipeline testing

### Test Examples
```csharp
[Fact]
public async Task ProductCommandHandler_Create_PersistsEntity_And_PublishesNotification()
{
    // Arrange
    var db = new ApplicationDbContext(inMemoryOptions);
    var uow = new UnitOfWork(db);
    var handler = new ProductCommandHandler(uow, mapper, logger, mediator);

    // Act
    var result = await handler.Handle(
        new CreateProductCommand(dto), 
        CancellationToken.None
    );

    // Assert
    result.Should().NotBeNull();
    result.Id.Should().BeGreaterThan(0);
}
```

---

## Performance Optimizations

### Database Level
- Connection pooling (128 connections for SQL Server)
- NoTracking for read operations
- Indexed columns (ProductName)
- Cascade delete for referential integrity
- 30-second command timeout

### Application Level
- DbContext pooling
- Dapper for read queries (5-10x faster)
- Redis caching for hot data
- Output caching middleware
- Async/await throughout

### Network Level
- Response compression
- Output caching
- Rate limiting
- Minimal payload size (column projection in Dapper)

### Memory Level
- No change tracking on reads
- No lazy loading
- Minimal object creation
- Efficient DTO mapping

---

## Logging

### Serilog Configuration
```json
{
    "Serilog": {
        "MinimumLevel": "Information",
        "WriteTo": [
            {
                "Name": "Console",
                "Args": {
                    "outputTemplate": "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
                }
            }
        ],
        "Enrich": ["FromLogContext"]
    }
}
```

### Log Points
- API request/response logging
- Database query execution
- Authentication/authorization attempts
- Exception details with stack traces
- Business operation results

---

## Dependency Injection Container

All services registered in Program.cs with proper lifetimes:

```csharp
// Infrastructure Services
builder.Services.AddInfrastructureServices(configuration);

// Application Services
builder.Services.AddApplicationServices(configuration);

// Framework Services
builder.Services.AddAutoMapper(...);
builder.Services.AddMediatR(...);
builder.Services.AddAuthentication(...);
builder.Services.AddHealthChecks();
builder.Services.AddApiVersioning(...);
builder.Services.AddCors(...);
```

### Service Lifetimes
- **Singleton**: Factory, Config, Static services
- **Scoped**: DbContext, UnitOfWork, Services (per HTTP request)
- **Transient**: Stateless utilities, handlers

---

## Security Measures

✅ **Implemented**:
- JWT authentication with HMAC-SHA256
- Token expiration and refresh rotation
- HTTPS-only enforcement
- CORS policy restriction
- Input validation (FluentValidation)
- SQL injection prevention (parameterized queries)
- Exception details not exposed to clients
- Correlation IDs for request tracing
- Rate limiting per IP address
- Secure token generation (RNG)

✅ **Recommended for Production**:
- API Key for service-to-service communication
- OAuth2/OpenID Connect for user identity
- Refresh token rotation strategy
- Azure Key Vault for secrets
- WAF (Web Application Firewall)
- HSTS headers
- Content Security Policy headers

---

## Health Checks

Configured to monitor application health:
```http
GET /health
```

Checks performed:
- Database connectivity
- External service availability
- Memory usage
- Response time

---

## Documentation

### Swagger/OpenAPI
- Auto-generated from controller attributes
- Full endpoint documentation
- Request/response schemas
- JWT authentication support
- Try-it-out functionality

Access Swagger UI: `http://localhost:5000/swagger`

### Code Documentation
- XML comments on public APIs
- Clear naming conventions
- Architecture diagrams
- Configuration examples

---

## Development Setup

### Prerequisites
- .NET 9 SDK
- SQL Server (LocalDB or full edition)
- Optional: Redis (for caching)
- Visual Studio 2022+ or VS Code

### Quick Start
```bash
# Clone repository
git clone https://github.com/chitraasaravanan/DemoProductsWebAPI.git
cd DemoProductsWebAPI

# Update connection strings in appsettings.json
# Run migrations (automatic on startup)

# Run application
dotnet run --project DemoProductsWebAPI.API

# Run tests
dotnet test
```

### Configuration (appsettings.json)
```json
{
    "ConnectionStrings": {
        "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=DemoProductsDb;",
        "RedisConnection": "localhost:6379"
    },
    "Jwt": {
        "Key": "your-secret-key-minimum-32-characters",
        "Issuer": "DemoProductsWebAPI",
        "Audience": "DemoProductsAPIClients",
        "ExpireMinutes": 15
    }
}
```

---

## API Design Expectation
