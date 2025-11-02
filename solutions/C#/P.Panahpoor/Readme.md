# High-Traffic Resilient Dashboard API

A production-ready ASP.NET Core API demonstrating advanced caching patterns for high-traffic scenarios.

---

##  Project Overview

This project implements a Dashboard API with:
- Resilient in-memory caching with Cache Stampede prevention
- Lazy Refresh strategy for zero-latency responses
- Rate Limiting and Response Compression

---

##  Design Patterns Used

- **Strategy Pattern**: Pluggable caching strategies
- **Template Method Pattern**: Unified cache retrieval flow
- **Facade Pattern**: Separation of concerns

---

##  Quick Start
```bash
# Create and run the project
dotnet new web -n DashboardAPI
cd DashboardAPI
dotnet add package Microsoft.AspNetCore.RateLimiting
# Replace Program.cs with the provided code
dotnet run
```

**Test the API:**
```bash
curl http://localhost:5000/api/dashboard
curl http://localhost:5000/api/dashboard/statistics
curl -X DELETE http://localhost:5000/api/dashboard/cache
```
---

## API Endpoints

### GET /api/dashboard
Returns dashboard data with intelligent caching.

### GET /api/dashboard/statistics
Displays cache performance (factory call count).

### DELETE /api/dashboard/cache
Manually clears the cache.

---

## Configuration

**Cache Settings** (in `ResilientMemoryCache` class):
```csharp
_absoluteLifetime = TimeSpan.FromHours(1);      // Maximum cache lifetime
_sliding = TimeSpan.FromMinutes(30);            // Expiration on inactivity
_refreshThreshold = TimeSpan.FromMinutes(50);   // Lazy Refresh trigger
```
**Rate Limiting:**
```csharp
opt.PermitLimit = 100;                          // Requests per minute
```
---

## How It Works

**Cache Stampede Prevention:**
- First request acquires lock and generates data
- Concurrent requests wait, then read from cache
- Only one expensive computation occurs

**Lazy Refresh:**
- User always receives immediate response
- Background refresh starts before expiration
- Next request gets fresh data

---

## Performance Metrics

| Metric | Value |
|--------|-------|
| First Request | 3-5 seconds |
| Cached Requests | < 10 milliseconds |
| Cache Duration | 1 hour / 30 min Sliding |
| Rate Limit | 100 requests/minute |

---

## Testing Cache Stampede

```bash
# Send 1000 concurrent requests
for i in {1..1000}; do curl http://localhost:5000/api/dashboard & done
wait
```

# Check statistics (factoryCallCount should be 1)
curl http://localhost:5000/api/dashboard/statistics

---

##  Project Structure


Program.cs sections:
- Service Registration (DI configuration)
- Middleware Pipeline
- API Endpoints
- DTOs and Models
- Service Implementation
- Resilient Memory Cache (core logic)

---

## Resources

- [Memory Caching](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/memory)
- [Rate Limiting](https://learn.microsoft.com/en-us/aspnet/core/performance/rate-limit)


You can copy this content directly into a `.md` file on GitHub. All standard Markdown tags are properly formatted.
