# 🧠 Dashboard Cache Optimization (C# .NET 8 Console App)

این پروژه یک نمونه‌ راهکار از **بهینه‌سازی کش در حافظه (In-Memory Cache)** در دات‌نت ۸ است که با استفاده از `IMemoryCache`، کنترل هم‌زمانی (`SemaphoreSlim`) و **جلوگیری از Cache Stampede** طراحی شده است.

---

## 🚀 ویژگی‌ها

- ⚡ **افزایش سرعت واکشی داده‌ها** با `IMemoryCache`
- 🔄 **تمدید خودکار Sliding Expiration**
- 🧱 **جلوگیری از Cache Stampede** با `SemaphoreSlim`
- 🕒 **Expiration ترکیبی (Absolute + Sliding)**
- 🧩 **Dependency Injection کامل با Generic Host**
- 💬 **Log رویدادهای حذف Cache (Eviction)**

---

## 🧰 پیش‌نیازها

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Visual Studio 2022 یا VS Code

---

## 📦 نصب و اجرا

1. **کلون پروژه**
   ```bash
   git clone https://github.com/Saffarnejad/dashboard-service-optimization.git
   cd dashboard-service-optimization
   ```

2. **بازیابی پکیج‌ها**
   ```bash
   dotnet restore
   ```

3. **اجرا**
   ```bash
   dotnet run
   ```

---

## 🧠 ساختار کد

```text
📁 dashboard-service-optimization
└── 📁 C#
    └── 📁 Saffarnejad
        ├── Program.cs
        └── README.md
```

---

## 🧩 کلاس‌های کلیدی

### `DashboardService`
- مدیریت داده‌های داشبورد و کش
- استفاده از `SemaphoreSlim` برای جلوگیری از تولید هم‌زمان داده
- استفاده از `MemoryCacheEntryOptions` برای کنترل Expiration

### `DashboardData`
مدل داده شبیه‌سازی‌شده شامل:
- `Id`
- `GeneratedAt`
- `TotalUsers`
- `ActiveSessions`
- `Revenue`
- `ProcessingTime`

---

## ⚙️ نمونه خروجی

```
Dashboard Cache Optimization Test
=================================

Test 1: First request (cache miss)
[00:45:12] Generating new dashboard data...
[User A] Data[2a4f...] | Generated: 00:45:15 | Users: 5423 | Sessions: 754 | Revenue: $72594 | Processed in: 4s
[User A] Response time: 4003ms

Test 2: Subsequent request (cache hit)
[User B] Data[2a4f...] | Generated: 00:45:15 | Users: 5423 | Sessions: 754 | Revenue: $72594 | Processed in: 4s
[User B] Response time: 1ms
```

---

## 🔍 مفهوم Cache Stampede

> Cache Stampede حالتی است که چندین درخواست هم‌زمان هنگام منقضی‌شدن کش، باعث بازسازی هم‌زمان داده می‌شوند.  
> در این پروژه با استفاده از `SemaphoreSlim` از این اتفاق جلوگیری می‌شود تا فقط یک Thread داده را تولید کند و سایرین منتظر بمانند.

---

## 🧪 تست عملکرد

در کلاس `Program` چند سناریو شبیه‌سازی شده است:
1. اولین درخواست (Cache Miss)
2. درخواست‌های بعدی (Cache Hit)
3. درخواست‌های هم‌زمان (Concurrent Simulation)
4. پاک‌سازی کش و تست مجدد تولید داده

---

## 🛠️ تکنولوژی‌ها

| تکنولوژی | توضیح |
|-----------|--------|
| .NET 8 | محیط اجرایی اصلی |
| Microsoft.Extensions.Hosting | مدیریت DI و ساخت Host |
| IMemoryCache | کش درون‌حافظه‌ای |
| SemaphoreSlim | هم‌زمانی و جلوگیری از Stampede |

---

## 📜

Develop by [Amin Saffarnejad](https://github.com/Saffarnejad) © 2025  
