# گزارش نهایی چالش — بهینه‌سازی سرویس داشبورد پر‌ترافیک

مخزن: https://github.com/dotin-challenge/dashboard-service-optimization
تاریخ ارزیابی: 2025-11-08

زبان: فارسی | [English](./result.md)

## معیارهای ارزیابی (بر اساس README)

| معیار | وزن |
|------|-----|
| درستی پیاده‌سازی | 40% |
| کیفیت و خوانایی کد | 25% |
| همزمانی و مدیریت خطا | 10% |
| کارایی و رفتار کش | 10% |
| مستندسازی | 5% |
| سرعت ارسال (زمان PR) | 5% |

نکات
- امتیاز «سرعت ارسال» به‌صورت گسسته اعمال شد: نفر اول=100، نفر دوم=70، نفر سوم=40 (با وزن 5%). تنها برای راهکارهایی که شرایط «درستی» را رعایت کرده‌اند لحاظ شده است.
- قواعد TTL طبق README الزام‌آور بود: اعتبار ۱ ساعت، و با هر دسترسی ۳۰ دقیقه از زمان خواندن تمدید شود (بدون سقف مطلق).

## جدول نهایی رتبه‌بندی

| رتبه | PR | نویسنده | امتیاز کل | لینک |
|-----|----|---------|-----------|------|
| 1 | #13 | HamedMortezaee | 82.5 | https://github.com/dotin-challenge/dashboard-service-optimization/pull/13 |
| 2 | #1 | arash-mosavi | 82.25 | https://github.com/dotin-challenge/dashboard-service-optimization/pull/1 |
| 3 | #2 | ShahramAfshar | 80.0 | https://github.com/dotin-challenge/dashboard-service-optimization/pull/2 |
| 4 | #12 | NegarJafari | 79.6 | https://github.com/dotin-challenge/dashboard-service-optimization/pull/12 |
| 5 | #11 | parsapanahpoor | 78.7 | https://github.com/dotin-challenge/dashboard-service-optimization/pull/11 |
| 6 | #5 | Saffarnejad | 77.4 | https://github.com/dotin-challenge/dashboard-service-optimization/pull/5 |
| 7 | #6 | KevinMKM | 76.8 | https://github.com/dotin-challenge/dashboard-service-optimization/pull/6 |
| 8 | #7 | r-poorbageri | 74.75 | https://github.com/dotin-challenge/dashboard-service-optimization/pull/7 |
| 9 | #3 | aminmansouri2000 | 67.25 | https://github.com/dotin-challenge/dashboard-service-optimization/pull/3 |
| 10 | #10 | agaheman | 60.5 | https://github.com/dotin-challenge/dashboard-service-optimization/pull/10 |
| 11 | #9 | ehsan12021 | 56.3 | https://github.com/dotin-challenge/dashboard-service-optimization/pull/9 |
| 12 | #4 | Hessam8008 | 55.0 | https://github.com/dotin-challenge/dashboard-service-optimization/pull/4 |

جایزه سرعت (اعمال‌شده در امتیاز کل)
- نفر اول ارسال صحیح: arash-mosavi (100 → 5.0+ وزنی)
- نفر دوم: ShahramAfshar (70 → 3.5+ وزنی)
- نفر سوم: aminmansouri2000 (40 → 2.0+ وزنی)

## مرور سه راهکار برتر
- PR #13 — HamedMortezaee
  - تمدید ۳۰ دقیقه‌ای بر اساس هر خواندن بدون سقف مطلق؛ تازه‌سازی پیش‌دستانه نزدیک انقضا.
  - مدل همزمانی قوی با تفکیک قفل‌های build و refresh؛ نگاشت مناسب خطا/لغو (کد شبیه 499).
  - لایه‌بندی و لاگ‌گذاری تمیز؛ مسیر hit بهینه و رقابت کم.

- PR #1 — arash-mosavi
  - جلوگیری خوب از Stampede با `SemaphoreSlim` + double‑check؛ سرویس/کنترلر روشن و لاگ مناسب.
  - TTL به‌صورت Absolute(1h)+Sliding(30m) است؛ برای انطباق کامل با README، refresh absolute روی خواندن (یا Sliding-only) پیشنهاد می‌شود.
  - پاسخ JSON ساختاریافته و پایه قابل نگهداری.

- PR #2 — ShahramAfshar
  - لایه‌بندی تمیز با helper قابل‌استفاده مجدد (`GetOrCreateAsync`).
  - جلوگیری از Stampede با `SemaphoreSlim`؛ TTL فعلی با absolute سقف‌دار است — با تمدید بر مبنای خواندن وفق داده شود.
  - لاگ/مشاهده‌پذیری بیشتر و رفع مشکل encoding README پیشنهاد می‌شود.

## لینک نتایج هر PR
- PR #1 (arash-mosavi): `solutions/C#/arash-mosavi/result.md`, `solutions/C#/arash-mosavi/result.fa.md`
- PR #2 (ShahramAfshar): `solutions/C#/shahramafshar/result.md`, `solutions/C#/shahramafshar/result.fa.md`
- PR #3 (aminmansouri2000): `solutions/C#/aminmansouri2000/result.md`, `solutions/C#/aminmansouri2000/result.fa.md`
- PR #4 (Hessam8008): `solutions/C#/Hessam8008/result.md`, `solutions/C#/Hessam8008/result.fa.md`
- PR #5 (Saffarnejad): `solutions/C#/Saffarnejad/result.md`, `solutions/C#/Saffarnejad/result.fa.md`
- PR #6 (KevinMKM): `solutions/C#/KevinMKM/result.md`, `solutions/C#/KevinMKM/result.fa.md`
- PR #7 (r-poorbageri): `solutions/C#/r-poorbageri/result.md`, `solutions/C#/r-poorbageri/result.fa.md`
- PR #9 (ehsan12021): `solutions/C#/ehsan12021/result.md`, `solutions/C#/ehsan12021/result.fa.md`
- PR #10 (agaheman): `solutions/C#/agaheman/result.md`, `solutions/C#/agaheman/result.fa.md`
- PR #11 (parsapanahpoor): `solutions/C#/parsapanahpoor/result.md`, `solutions/C#/parsapanahpoor/result.fa.md`
- PR #12 (NegarJafari): `solutions/C#/NegarJafari/result.md`, `solutions/C#/NegarJafari/result.fa.md`
- PR #13 (HamedMortezaee): `solutions/C#/HamedMortezaee/result.md`, `solutions/C#/HamedMortezaee/result.fa.md`

## مشاهدات کلی
- قواعد TTL: امتیاز بالاتر برای راهکارهایی بود که absolute را روی هر خواندن تازه‌سازی کردند یا Sliding-only به‌کار بردند تا تمدید ۳۰ دقیقه‌ای تضمین شود.
- الگوهای ضد‑Stampede: `SemaphoreSlim` per‑key و `Lazy<Task<T>>` مؤثر بودند؛ قفل‌های per‑key مقیاس‌پذیرتر از قفل سراسری‌اند.
- همزمانی و خطا: propagation `CancellationToken` و پاسخ‌های خطای ساختاریافته شفافیت و تاب‌آوری را افزایش داد.
- رفتار کارایی: double‑check و مقاطع بحرانی کوچک بازمحاسبه را کم کرد؛ refresh پس‌زمینه (SWR) شروع‌های سرد را کاهش داد.
- مستندسازی و DX: READMEهای کوتاه و دقیق با گام‌های اجرا/آزمون و توضیح طراحی، اطمینان را بالا برد؛ به مشکلات encoding توجه شود.
