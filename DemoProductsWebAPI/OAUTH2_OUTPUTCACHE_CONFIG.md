# OAuth2 and Output Cache Configuration Guide

## Overview
This document describes the OAuth2 authentication and 5-minute output cache configurations added to the DemoProductsWebAPI Swagger documentation.

## Changes Made

### 1. OAuth2 Authentication for Swagger (OpenApiExtensions.cs)

#### Added Features:
- **JWT Bearer Token Support** (existing enhanced):
  - Manual JWT token entry in Swagger UI
  - Users can paste bearer tokens directly
  - Header: `Authorization: Bearer {token}`

- **OAuth2 Support** (NEW):
  - **Implicit Flow**: For testing and development with Swagger UI
  - **Authorization Code Flow**: For production scenarios with PKCE
  - **PKCE Support**: Enhanced security with Proof Key for Code Exchange
  - **Scopes**: openid, profile, email

#### Swagger UI Authentication Methods:
Users can now authenticate using:
1. **Bearer Token Button**: Paste JWT token directly
2. **OAuth2 Button**: Authenticate via OAuth2 provider

#### Configuration Details:

```csharp
// OAuth2 Client Configuration in Swagger UI
c.OAuthClientId("swagger-client");           // OAuth2 client identifier
c.OAuthAppName("DemoProductsWebAPI");        // Application name shown in auth flow
c.OAuthScopes("openid", "profile", "email"); // Requested scopes
c.OAuthUsePkce();                            // Enable PKCE for security
```

#### OAuth2 Provider Integration:
The current configuration uses Google OAuth2 as an example:
- Authorization URL: `https://accounts.google.com/o/oauth2/v2/auth`
- Token URL: `https://oauth2.googleapis.com/token`

**To use your own OAuth2 provider:**
1. Replace authorization and token URLs in `AddSwaggerGenWithJwt()` method
2. Configure OAuth2 client ID and secret in your identity provider
3. Update `OAuthClientId()` in `MapSwaggerEndpoints()` to match your provider's client ID
4. Add your OAuth2 provider to Swagger UI configuration

### 2. Output Cache Configuration (Program.cs)

#### Added Feature:
- **Default Cache Duration**: 5 minutes (300 seconds)
- **Applied To**: All endpoints marked with `[OutputCache]` attribute

#### Configuration:

```csharp
builder.Services.AddOutputCache(options =>
{
    // Default cache duration: 5 minutes for all cached endpoints
    options.DefaultExpirationTimeSpan = TimeSpan.FromMinutes(5);
});
```

#### Usage in Controllers:

```csharp
[HttpGet]
[OutputCache] // Uses default 5-minute cache
public async Task<IActionResult> Get()
{
    // Endpoint response will be cached for 5 minutes
    var list = await _mediator.Send(new GetAllProductsQuery());
    return Ok(list);
}
```

#### Custom Cache Durations:

If you need different cache durations for specific endpoints, add named policies:

```csharp
builder.Services.AddOutputCache(options =>
{
    options.DefaultExpirationTimeSpan = TimeSpan.FromMinutes(5);
    
    // Short cache for frequently-changing data
    options.AddPolicy("ShortCache", builder => 
        builder.Expire(TimeSpan.FromSeconds(30)));
    
    // Long cache for static data
    options.AddPolicy("LongCache", builder => 
        builder.Expire(TimeSpan.FromMinutes(30)));
});
```

Then use in controllers:

```csharp
[OutputCache(PolicyName = "ShortCache")] // 30 seconds
public async Task<IActionResult> GetFrequentData() { }

[OutputCache(PolicyName = "LongCache")]  // 30 minutes
public async Task<IActionResult> GetStaticData() { }
```

## Testing OAuth2 in Swagger

### Step 1: Access Swagger UI
Navigate to: `https://localhost:{port}/swagger`

### Step 2: Authenticate
1. Click the **green "Authorize" button** (top-right of Swagger UI)
2. Select **OAuth2** (or Bearer Token)
3. Click **"Authorize"** button
4. You'll be redirected to the OAuth2 provider login page
5. Log in and grant permissions
6. Token is automatically injected into subsequent requests

### Step 3: Make Authorized Requests
- Swagger UI will automatically include the OAuth2 token in all requests
- No need to manually add `Authorization: Bearer` header
- Token persists for the session

## Testing Output Cache

### Verify Cache is Working:

```powershell
# First request - response from endpoint
curl -i https://localhost:5001/api/v1.0/products

# Check response headers - should include:
# Cache-Control: public, max-age=300

# Second request within 5 minutes - response served from cache
# Performance improvement: Response time significantly reduced
```

### Cache-Control Headers:
```
Cache-Control: public, max-age=300
```
- `public`: Can be cached by browser and intermediaries
- `max-age=300`: Expires after 300 seconds (5 minutes)

## Security Considerations

### OAuth2 Security Best Practices:

1. **Use HTTPS Only**
   - OAuth2 tokens should only be transmitted over HTTPS
   - Never use HTTP in production

2. **PKCE (Proof Key for Code Exchange)**
   - Enabled by default: `c.OAuthUsePkce()`
   - Protects against authorization code interception attacks
   - Required for public clients (like Swagger UI)

3. **Scope Limitations**
   - Only request necessary scopes: `openid`, `profile`, `email`
   - Don't request excessive permissions

4. **Client ID Security**
   - Store OAuth2 client credentials securely
   - Don't hardcode secrets in code (use environment variables/config)
   - Current config shows `swagger-client` for development only

5. **Token Expiration**
   - OAuth2 tokens should have short expiration times
   - Use refresh tokens for extended sessions
   - Swagger UI handles token management automatically

### Output Cache Security:

1. **Authentication Required**
   - Cache should only apply to public endpoints or endpoints visible to the authenticated user
   - Private/sensitive data should NOT be cached
   - Use `[Authorize]` attribute with caching carefully

2. **Cache Invalidation**
   - Set appropriate expiration times
   - Invalidate cache when data changes
   - Monitor cache hits vs misses

3. **CORS Considerations**
   - Cached responses include CORS headers
   - Different origins see the same cached response
   - Verify CORS policy matches cache requirements

## Configuration for Different Environments

### Development (appsettings.Development.json):
```json
{
  "OAuth2": {
    "Provider": "Google",
    "ClientId": "swagger-client"
  },
  "Cache": {
    "OutputCacheDuration": 5
  }
}
```

### Production (appsettings.json):
```json
{
  "OAuth2": {
    "Provider": "Azure AD / Identity Provider",
    "ClientId": "your-production-client-id",
    "Scopes": ["api://your-api/.default"]
  },
  "Cache": {
    "OutputCacheDuration": 5
  }
}
```

## Integration with Existing ProductsController

Current implementation in `ProductsController.cs`:

```csharp
[HttpGet]
[Microsoft.AspNetCore.OutputCaching.OutputCache]  // Uses default 5-minute cache
[ProducesResponseType(StatusCodes.Status200OK)]
public async Task<IActionResult> Get()
{
    var list = await _mediator.Send(new GetAllProductsQuery()).ConfigureAwait(false);
    return ApiResponse(list);
}
```

This endpoint now:
1. **Requires OAuth2 or JWT authentication** (via Swagger UI authorize button)
2. **Caches response for 5 minutes** automatically
3. **Improves performance** for frequently-accessed product list

## References

### Documentation:
- [ASP.NET Core Output Caching](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/output)
- [OAuth2 Authorization Code Flow](https://tools.ietf.org/html/rfc6749#section-1.3.1)
- [PKCE (RFC 7636)](https://tools.ietf.org/html/rfc7636)
- [Swagger UI OAuth2 Configuration](https://swagger.io/docs/open-source-tools/swagger-ui/usage/oauth2/)

### Files Modified:
1. `DemoProductsWebAPI.API/Extensions/OpenApiExtensions.cs` - OAuth2 and JWT configuration
2. `DemoProductsWebAPI.API/Program.cs` - Output cache registration (5-minute duration)

## Next Steps

1. **Configure OAuth2 Provider**
   - Update authorization and token URLs
   - Register OAuth2 application with your provider
   - Configure client ID and secret

2. **Test End-to-End**
   - Access Swagger UI
   - Authenticate via OAuth2
   - Test product endpoints with cache

3. **Monitor Performance**
   - Track cache hit rates
   - Adjust cache duration based on data freshness requirements
   - Monitor authentication performance

4. **Production Deployment**
   - Move to production OAuth2 provider
   - Update configuration for your environment
   - Implement token refresh strategy
   - Monitor and log authentication events
