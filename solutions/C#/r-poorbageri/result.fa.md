# نتیجه - PR #7 ([Solution] High-Traffic Dashboard Optimization - r.pourbagheri)
نویسنده: `r-poorbageri`
لینک: https://github.com/dotin-challenge/dashboard-service-optimization/pull/7
زبان: فارسی | [English](./result.md)

## خلاصه
Minimal API با `IMemoryCache` و دو استراتژی ضد-Stampede: `Semaphore` (قفل + double-check) و `Lazy<Task<T>>` (اشتراک محاسبه). سیاست کش: `Absolute=60m` و `Sliding=30m` از پیکربندی. مسیرها: `GET /dashboard` و `POST /dashboard/invalidate`. تأخیر ۳ ثانیه برای کار سنگین؛ محدودیت Size و Priority نیز تنظیم شده است.

طبق README، هر خواندن باید ۳۰ دقیقه تمدید شود؛ نگه داشتن absolute ثابت و عدم refresh آن، تمدید را سقف‌دار می‌کند و با نیت مسئله در تضاد است.

## امتیازدهی
| معیار | وزن | امتیاز (0-100) | وزنی | توضیحات |
|------|-----|-----------------|------|---------|
| درستی پیاده‌سازی | 40% | 80 | 32.0 | سقف Absolute ثابت؛ Sliding اعمال می‌شود ولی محدود است |
| کیفیت کد و خوانایی | 25% | 88 | 22.0 | سازماندهی واضح در یک فایل؛ انتزاع استراتژی |
| همزمانی و مدیریت خطا | 10% | 90 | 9.0 | استراتژی Semaphore/Lazy؛ پشتیبانی cancellation |
| کارایی و رفتار کش | 10% | 85 | 8.5 | کارا؛ Lazy<Task> پذیرفتنی اما غیرمتداول |
| مستندسازی | 5% | 65 | 3.25 | README با پیکربندی؛ کوتاه |
| سرعت ارسال | 5% | 0 | 0.0 | خارج از سه نفر اول |
| مجموع |  |  | 74.75 |  |

## نقاط قوت
- دو استراتژی ضد-Stampede قوی (Semaphore و Lazy<Task>). 
- TTL، SizeLimit و endpoint ابطال کش قابل پیکربندی.
- پشتیبانی از cancellation و DTO مناسب.

## موارد قابل بهبود
- هم‌راستاسازی TTL با README با refresh absolute روی خواندن یا Sliding-only.
- پرهیز از کش مستقیم `Lazy<Task<T>>`؛ ترجیحاً مقدار نهایی را کش کنید.
- لاگ وضعیت کش و پاسخ‌های خطای ساختاریافته را اضافه کنید.

## توضیحات امتیاز
- Correctness: Sliding صحیح اما به سقف absolute محدود است.
- Quality: یک فایل اما با بخش‌بندی واضح.
- Concurrency/Error: هر دو استراتژی مؤثرند؛ cancellation لحاظ شده است.
- Performance: سربار کم؛ الگوی Lazy کمتر مرسوم.
- Docs: مناسب اما می‌تواند کامل‌تر شود.

