# API داشبورد با کش مقاوم برای ترافیک بالا

یک API آماده تولید در ASP.NET Core که الگوهای پیشرفته کش را برای سناریوهای با ترافیک بالا نشان می‌دهد.

---

##  خلاصه پروژه

این پروژه یک API داشبورد پیاده‌سازی می‌کند با:
- کش حافظه مقاوم با جلوگیری از Cache Stampede
- استراتژی Lazy Refresh برای پاسخ‌دهی بدون تاخیر
- فشرده‌سازی پاسخ و Rate Limiting  

---

##  الگوهای طراحی استفاده‌شده

- **الگوی Strategy**: استراتژی‌های کش قابل تعویض
- **الگوی Template Method**: جریان یکپارچه دریافت از کش
- **الگوی Facade**: جداسازی وظایف

---

##  راه‌اندازی سریع
```bash
# ایجاد و اجرای پروژه
dotnet new web -n DashboardAPI
cd DashboardAPI
dotnet add package Microsoft.AspNetCore.RateLimiting
```
# فایل Program.cs را با کد ارائه‌شده جایگزین کنید
dotnet run

**تست API:**
```bash
curl http://localhost:5000/api/dashboard
curl http://localhost:5000/api/dashboard/statistics
curl -X DELETE http://localhost:5000/api/dashboard/cache
```
---

##  Endpoint های API

### GET /api/dashboard
داده‌های داشبورد را با کش هوشمند برمی‌گرداند.

### GET /api/dashboard/statistics
کارایی کش را نمایش می‌دهد (تعداد فراخوانی Factory).

### DELETE /api/dashboard/cache
کش را به صورت دستی پاک می‌کند.

---

##  تنظیمات

**تنظیمات کش** (در کلاس `ResilientMemoryCache`):
```csharp
_absoluteLifetime = TimeSpan.FromHours(1);      // حداکثر عمر کش
_sliding = TimeSpan.FromMinutes(30);            // انقضا در صورت عدم استفاده
_refreshThreshold = TimeSpan.FromMinutes(50);   // شروع Lazy Refresh
```
**محدودیت درخواست:**
```csharp
opt.PermitLimit = 100;                          // تعداد درخواست در دقیقه
```
---

##  نحوه عملکرد

**جلوگیری از Cache Stampede:**
- اولین درخواست قفل را می‌گیرد و داده را تولید می‌کند
- درخواست‌های همزمان منتظر می‌مانند، سپس از کش می‌خوانند
- فقط یک محاسبه سنگین انجام می‌شود

**Lazy Refresh:**
- کاربر همیشه پاسخ فوری دریافت می‌کند
- Refresh در پس‌زمینه قبل از انقضا شروع می‌شود
- درخواست بعدی داده تازه دریافت می‌کند

---

##  عملکرد

| معیار | مقدار |
|-------|-------|
| اولین درخواست | 3-5 ثانیه |
| درخواست‌های کش‌شده | کمتر از 10 میلی‌ثانیه |
| مدت کش | 1 ساعت / 30 دقیقه Sliding |
| محدودیت درخواست | 100 درخواست در دقیقه |

---

##  تست Cache Stampede

```bash
# ارسال 1000 درخواست همزمان
for i in {1..1000}; do curl http://localhost:5000/api/dashboard & done
wait
```
# بررسی آمار (باید factoryCallCount = 1 باشد)
curl http://localhost:5000/api/dashboard/statistics

---

##  ساختار پروژه


بخش‌های Program.cs:
- Service Registration (تنظیمات DI)
- Middleware Pipeline
- API Endpoints
- DTOs و Models
- Service Implementation
- Resilient Memory Cache (منطق اصلی)

---

##  منابع

- [Memory Caching](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/memory)
- [Rate Limiting](https://learn.microsoft.com/en-us/aspnet/core/performance/rate-limit)


