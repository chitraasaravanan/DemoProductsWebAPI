
using DemoProductsWebAPI.API.Extensions;
using DemoProductsWebAPI.API.Middleware;
using DemoProductsWebAPI.Application.Extensions;
using DemoProductsWebAPI.Application.Services;
using DemoProductsWebAPI.Common.Interfaces;
using DemoWebAPI.Core.DTOs;
using DemoWebAPI.Core.Extensions;
using DemoWebAPI.Core.Http;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Threading.RateLimiting;

namespace DemoProductsWebAPI.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure Serilog (reads configuration from appsettings)
            Serilog.Log.Logger = new Serilog.LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

            // Add services to the container.
            builder.Services.AddControllers();
            // OpenAPI / Swagger (lightweight minimal setup)
            builder.Services.AddSwaggerGenWithJwt();

            // DbContext - prefer SQL Server (LocalDB). Fall back to InMemory if not configured.
            var conn = builder.Configuration.GetConnectionString("DefaultConnection");
            if (!string.IsNullOrEmpty(conn))
            {
                // Use DbContext pooling for better performance under high throughput
                builder.Services.AddDbContextPool<ApplicationDbContext>(options =>
                    options.UseSqlServer(conn, sqlOptions => sqlOptions.CommandTimeout(30))
                           .UseQueryTrackingBehavior(Microsoft.EntityFrameworkCore.QueryTrackingBehavior.NoTracking),
                    poolSize: 128);
            }
            else
            {
                builder.Services.AddDbContextPool<ApplicationDbContext>(options =>
                    options.UseInMemoryDatabase("DemoProductsDb")
                           .UseQueryTrackingBehavior(Microsoft.EntityFrameworkCore.QueryTrackingBehavior.NoTracking),
                    poolSize: 8);
            }

            // Dapper read DB connection factory
            builder.Services.AddSingleton<Infrastructure.Data.Read.IDbConnectionFactory, Infrastructure.Data.Read.SqlConnectionFactory>();

            // Redis cache for read queries
            var redisConn = builder.Configuration.GetConnectionString("RedisConnection");
            if (!string.IsNullOrEmpty(redisConn))
            {
                builder.Services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = redisConn;
                });
            }

            // Authentication - JWT Bearer (skeleton)
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                // configure TokenValidationParameters from settings
                var jwt = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();
                if (jwt != null)
                {
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidIssuer = jwt.Issuer,
                        ValidAudience = jwt.Audience,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwt.Key))
                    };
                }
            });

            // Repositories & Unit of Work (application-level abstractions)
            // Infrastructure services are registered via the Application layer to avoid direct project reference from API.
            builder.Services.AddApplicationServices(builder.Configuration);

            // AutoMapper (will scan assemblies for profiles)
            // Use explicit overload to avoid ambiguity with other AddAutoMapper extensions
            builder.Services.AddAutoMapper(cfg => { }, typeof(Application.Mapping.ProductProfile).Assembly);

            // MediatR for CQRS - register handlers from the Application assembly so command/query handlers are discovered
            builder.Services.AddMediatR(typeof(Application.Products.Handlers.ProductCommandHandler).Assembly, typeof(Application.ProductCarts.Handlers.ProductCartCommandHandler).Assembly);

            // Response caching (output caching alternative)
            builder.Services.AddResponseCaching();
            // Output caching (new in ASP.NET Core) - used by [OutputCache] attribute on controllers
            builder.Services.AddOutputCache();

            // Rate limiting (per IP fixed window)
            builder.Services.AddRateLimiter(options =>
            {
                options.AddPolicy<string>("Fixed", httpContext =>
                {
                    var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                    return RateLimitPartition.GetFixedWindowLimiter(ip, _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 100,
                        Window = TimeSpan.FromMinutes(1),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 0
                    });
                });
            });

            // Health checks
            builder.Services.AddHealthChecks();

            // API Versioning
            builder.Services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
                options.ReportApiVersions = true;
            });
            builder.Services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            // FluentValidation
            builder.Services.AddFluentValidationAutoValidation();

            // Resilience policies (Polly) and typed HttpClients - use common shared implementation
            builder.Services.AddCommonResilience(builder.Configuration);
            // Example typed client registration via common factory
            TypedHttpClientFactory.RegisterTypedClient(builder.Services, "ProductService", builder.Configuration.GetValue<string>("ExternalServices:ProductServiceBaseUrl"));

            // Application services
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IProductCartService, ProductCartService>();
            // Register read-optimized services used by query handlers
            // We rely on output caching in the API layer rather than an application-level cached read service.
            builder.Services.AddScoped<DemoProductsWebAPI.Common.Interfaces.IProductReadService, DemoProductsWebAPI.Infrastructure.Data.Read.ProductReadService>();

            // Token service with IOptions
            builder.Services.AddScoped<Common.Interfaces.ITokenService, Infrastructure.Services.TokenService>();

            // Bind Jwt settings
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

            // Dapper-based read services (optimized queries) are registered in Application layer when required.

            // CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });

            var app = builder.Build();

            // Ensure database is created and migrations are applied in development
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                // Only apply migrations for relational providers (e.g., SQL Server).
                // InMemory provider doesn't support migrations and will throw if Migrate is called.
                if (db.Database.IsRelational())
                {
                    db.Database.Migrate();
                }
                else
                {
                    db.Database.EnsureCreated();
                }
            }

            // Configure the HTTP request pipeline.
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            // Request/Response logging middleware
            app.Use(async (context, next) =>
            {
                var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogInformation("Handling {method} {path}", context.Request.Method, context.Request.Path);
                await next();
                logger.LogInformation("Handled {status}", context.Response.StatusCode);
            });

            app.UseRateLimiter();
            // Enable Output Caching middleware
            app.UseOutputCache();
            app.UseResponseCaching();

            if (app.Environment.IsDevelopment())
            {
                app.MapSwaggerEndpoints();
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowAll");

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
