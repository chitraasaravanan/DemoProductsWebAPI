using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace DemoProductsWebAPI.API.Extensions
{
    public static class OpenApiExtensions
    {
        /// <summary>
        /// Extension methods to register and map OpenAPI (Swagger) services and UI for the application.
        /// </summary>
        /// <summary>
        /// Registers Swagger generation and configures JWT bearer support for the Swagger UI.
        /// Adds endpoint explorer and attempts to include XML comments if available.
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
                    Description = "Demo API for products with auth and examples"
                });

                // JWT Bearer token support in Swagger
                var securityScheme = new OpenApiSecurityScheme
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

                c.AddSecurityDefinition("Bearer", securityScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { securityScheme, Array.Empty<string>() }
                });

                // Include XML comments if available (for enriched docs)
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
                    // ignore if XML comments cannot be loaded
                }
                });

            return services;
        }

        /// <summary>
        /// Registers the Swagger middleware and the Swagger UI endpoints on the application pipeline.
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
                // host the UI at /swagger
                c.RoutePrefix = "swagger";
            });

            return app;
        }
    }
}
