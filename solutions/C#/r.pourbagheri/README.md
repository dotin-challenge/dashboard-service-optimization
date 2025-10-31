# 📊 High-Traffic Dashboard Optimization

This project is a simple **ASP.NET Core** service that manages dashboard data using **MemoryCache** and applies strategies to prevent **Cache Stampede**.  
All code is contained in a single `Program.cs` file.  

---

## 🚀 Features
- Uses **MemoryCache** with:
  - Absolute Expiration  
  - Sliding Expiration  
  - Size Limit (configurable via `appsettings.json`)  
- Supports two strategies to prevent Cache Stampede:
  - **Semaphore Strategy** → only one request regenerates new data while others wait.  
  - **Lazy Strategy** → uses `Lazy<Task>` so concurrent requests share the same computation.  
- Provides APIs for:
  - Retrieving dashboard data (with caching)  
  - Invalidating the cache manually  

---

## ⚙️ Configuration (appsettings.json)
```json
{
  "CacheSettings": {
    "AbsoluteExpirationMinutes": 60,
    "SlidingExpirationMinutes": 30,
    "StampedeStrategy": "Semaphore",
    "SizeLimit": 1024
  }
}
