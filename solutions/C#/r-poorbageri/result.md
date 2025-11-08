# Result - PR #7 ([Solution] High-Traffic Dashboard Optimization - r.pourbagheri)
Author: `r-poorbageri`
Link: https://github.com/dotin-challenge/dashboard-service-optimization/pull/7
Language: [فارسی](./result.fa.md) | English

## Summary
ASP.NET Core minimal API with `IMemoryCache` and two stampede-prevention strategies: `Semaphore` (lock + double-check) and `Lazy<Task<T>>` (shared computation). Cache policy uses configurable `AbsoluteExpirationRelativeToNow` (default 60m) and `SlidingExpiration` (30m). Endpoint: `GET /dashboard`, plus `POST /dashboard/invalidate`. Heavy work is simulated via a 3s delay. Size limit and priority are configured.

Per README, each access should extend validity by 30 minutes from the read time. This implementation preserves a fixed absolute cap and does not refresh it on read; sliding is honored but constrained by the absolute TTL, which can conflict with the intended behavior.

## Score Breakdown
| Criteria | Weight | Score (0-100) | Weighted | Notes |
|---------|--------|---------------|----------|-------|
| Correctness | 40% | 80 | 32.0 | Absolute cap remains; sliding honored but capped |
| Code Quality & Readability | 25% | 88 | 22.0 | Clean organization in one file; strategy abstraction |
| Concurrency & Error Handling | 10% | 90 | 9.0 | Semaphore or Lazy strategies; cancellation respected |
| Performance & Caching Behavior | 10% | 85 | 8.5 | Efficient; Lazy<Task> caching acceptable but uncommon |
| Documentation | 5% | 65 | 3.25 | README present with config; concise but basic |
| Submission Speed | 5% | 0 | 0.0 | Outside top 3 |
| Total |  |  | 74.75 |  |

## Strengths
- Offers two solid anti-stampede strategies (Semaphore and Lazy<Task>). 
- Configurable TTLs, size limit, and explicit invalidation endpoint.
- Cancellation support and reasonable DTO design.

## Areas to Improve
- Align TTL semantics with README by refreshing absolute window on read or relying on sliding-only.
- Consider avoiding caching `Lazy<Task<T>>` directly; cache resolved values to simplify error handling/telemetry.
- Add more observability (hit/miss logs, eviction reasons) and structured error responses.

## Score Notes
- Correctness: Sliding is correct; absolute cap not refreshed; may violate per-access extension intent.
- Code Quality & Readability: Single-file organization but with clear sections and interfaces.
- Concurrency & Error Handling: Both strategies control stampede; cancellation is passed through.
- Performance & Caching Behavior: Low overhead; Lazy<Task> pattern works but is less conventional.
- Documentation: Includes config and endpoints; could expand run/testing details.
