# High-Traffic Dashboard Optimization Solution

**Author:** arash.mousavi

---

## Solution Overview

This solution implements a resilient caching strategy for a high-demand dashboard service using `IMemoryCache` with cache stampede prevention to handle concurrent requests efficiently.


---


### Build & Run

```bash
cd solutions/C#/arash.mousavi
dotnet build
dotnet run
```

The API will start on:
- HTTPS: `https://localhost:5001`
- HTTP: `http://localhost:5000`

### Access Endpoints

**Dashboard Endpoint:**
```bash
curl http://localhost:5000/api/dashboard
```

---

## Testing

### 1. Manual Testing

**First Request (Cache MISS):**
```bash
curl http://localhost:5000/api/dashboard
# Takes ~3 seconds (heavy computation simulation)
```

**Second Request (Cache HIT):**
```bash
curl http://localhost:5000/api/dashboard
# Returns instantly from cache
```

### 2. Concurrent Load Testing

Using Apache Bench (100 requests, 10 concurrent):
```bash
ab -n 100 -c 10 http://localhost:5000/api/dashboard
```
