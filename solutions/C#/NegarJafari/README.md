# Dashboard Cache Service Solution

## Overview

This solution implements a resilient caching mechanism for a high-demand dashboard service that prevents cache stampede problems when cached data expires.

## Implementation Details

### Key Features

1. **Cache Stampede Prevention**: Uses `ConcurrentDictionary` with `Lazy<Task<DashboardDto>>` to ensure only one request regenerates data when cache expires.

2. **Initial Cache Duration**: Data is cached for 1 hour initially.

3. **Sliding Expiration Extension**: Each read operation extends the cache lifetime by 30 minutes from the current read time.

4. **Stale Data Return**: When cache expires and regeneration is in progress, the stale data is returned to avoid blocking requests.

### Architecture

- **DashboardCacheService**: Main service that handles all caching logic
- **DashboardDto**: Data transfer object for dashboard data
- **Program.cs**: Web API endpoint implementation

### How It Works

1. **Cache Hit**: If data exists in cache and hasn't expired, it's returned immediately and expiration is extended by 30 minutes.

2. **Cache Miss**: If cache is empty or expired:
   - A `Lazy<Task>` is created to regenerate data
   - Multiple concurrent requests will share the same `Lazy<Task`, preventing multiple regenerations
   - Stale data (if available) is returned while regeneration is in progress
   - New data is cached for 1 hour after regeneration completes

3. **Thread Safety**: All operations are thread-safe using `ConcurrentDictionary` and proper synchronization.

## Running the Solution

```bash
dotnet restore
dotnet build
dotnet run
```

The service will be available at `http://localhost:5000` or `http://localhost:5001`.

### Endpoints

- `GET /` - Health check
- `GET /dashboard` - Get dashboard data (cached)

## Testing

You can test the cache behavior by:

1. Making multiple requests to `/dashboard` - first request will take ~3 seconds, subsequent requests will be instant.
2. Testing concurrent requests using tools like Apache Bench:

```bash
ab -n 100 -c 10 http://localhost:5000/dashboard
```

This will show that only one regeneration occurs even with 100 concurrent requests.

