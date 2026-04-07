using Microsoft.OpenApi.Models;
using System.Reflection;

namespace DemoProductsWebAPI.API.Extensions
{
    public static class OpenApiExtensions
    {
        /// <summary>
        /// Extension methods to register and map OpenAPI (Swagger) services and UI for the application.
        /// </summary>
        /// <summary>
        /// Registers Swagger generation and configures JWT bearer and OAuth2 support for the Swagger UI.
        /// Adds endpoint explorer and includes XML comments if available.
        /// Supports both direct JWT bearer token input and OAuth2 implicit/authorization code flows.
        /// </summary>
        /// <param name="services">The service collection to add the OpenAPI services to.</param>
        /// <returns>The original service collection for chaining.</returns>
        public static IServiceCollection AddSwaggerGenWithJwt(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "DemoProductsWebAPI",
                    Version = "v1",
                    Description = "Demo API for products with JWT authentication and OAuth2 support"
                });

                // JWT Bearer token support in Swagger - allows manual token entry
                var jwtSecurityScheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Enter JWT bearer token as: Bearer {token}",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };

                c.AddSecurityDefinition("Bearer", jwtSecurityScheme);

                // OAuth2 authentication support in Swagger
                // Enables interactive login and token acquisition in Swagger UI
                var oAuth2SecurityScheme = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Scheme = "oauth2",
                    Name = "oauth2",
                    Description = "OAuth2 authentication for accessing protected endpoints",
                    In = ParameterLocation.Header,
                    Flows = new OpenApiOAuthFlows
                    {
                        // Implicit flow for testing/development (Swagger UI)
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri("https://accounts.google.com/o/oauth2/v2/auth"), // Example: Google OAuth2
                            Scopes = new Dictionary<string, string>
                            {
                                { "openid", "OpenID Connect profile scope" },
                                { "profile", "User profile information" },
                                { "email", "User email address" }
                            }
                        },
                        // Authorization code flow for production scenarios
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri("https://accounts.google.com/o/oauth2/v2/auth"),
                            TokenUrl = new Uri("https://oauth2.googleapis.com/token"),
                            Scopes = new Dictionary<string, string>
                            {
                                { "openid", "OpenID Connect profile scope" },
                                { "profile", "User profile information" },
                                { "email", "User email address" }
                            }
                        }
                    },
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "OAuth2"
                    }
                };

                c.AddSecurityDefinition("OAuth2", oAuth2SecurityScheme);

                // Security requirements: endpoints require either Bearer token OR OAuth2
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { jwtSecurityScheme, Array.Empty<string>() },
                    { oAuth2SecurityScheme, new[] { "openid", "profile", "email" } }
                });

                // Include XML comments if available (for enriched documentation)
                try
                {
                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    if (File.Exists(xmlPath))
                    {
                        c.IncludeXmlComments(xmlPath);
                    }
                }
                catch
                {
                    // Ignore if XML comments cannot be loaded
                }
            });

            return services;
        }

        /// <summary>
        /// Registers the Swagger middleware and the Swagger UI endpoints on the application pipeline.
        /// Configures Swagger UI with OAuth2 client settings for interactive authentication.
        /// </summary>
        /// <param name="app">The web application to map the Swagger endpoints to.</param>
        /// <returns>The web application for chaining.</returns>
        public static WebApplication MapSwaggerEndpoints(this WebApplication app)
        {
            // Serve the generated Swagger as JSON endpoint and the UI
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "DemoProductsWebAPI v1");
                c.RoutePrefix = "swagger";

                // OAuth2 configuration for Swagger UI
                // Users can interactively authenticate and obtain tokens directly in Swagger UI
                c.OAuthClientId("swagger-client"); // OAuth2 client ID for Swagger UI
                c.OAuthAppName("DemoProductsWebAPI - Swagger UI");
                c.OAuthScopes("openid", "profile", "email");

                // Persist authentication to maintain token across page refreshes
                c.OAuthUsePkce(); // Use PKCE (Proof Key for Code Exchange) for enhanced security
            });

            return app;
        }
    }
}
