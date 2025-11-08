# Result - PR #2 (ShahramAfshar)
Author: `ShahramAfshar`
Link: https://github.com/dotin-challenge/dashboard-service-optimization/pull/2
Language: [فارسی](./result.fa.md) | English

## Summary
Implements a minimal Web API with a dedicated `CacheService` wrapping `IMemoryCache` and preventing cache stampede via `SemaphoreSlim` with double-checking. The endpoint `GET /dashboard` fetches data through `GetOrCreateAsync`, setting both 1-hour absolute and 30-minute sliding expirations. Heavy computation is simulated with a 5-second delay in `DashboardService`. Code is split into Interfaces/Services/Models with clear responsibilities.

Note: Per README, each access should extend validity by 30 minutes from the read time. Combining absolute (1h) and sliding (30m) caps sliding by the absolute TTL and can violate the intended extension behavior on late reads. Consider sliding-only or refreshing the absolute window on access.

## Score Breakdown
| Criteria | Weight | Score (0-100) | Weighted | Notes |
|---------|--------|---------------|----------|-------|
| Correctness | 40% | 85 | 34.0 | Absolute+Sliding conflicts with README TTL rule |
| Code Quality & Readability | 25% | 88 | 22.0 | Good layering; minimal logging/comments |
| Concurrency & Error Handling | 10% | 85 | 8.5 | SemaphoreSlim OK; no error mapping/logging |
| Performance & Caching Behavior | 10% | 90 | 9.0 | Efficient path; negligible overhead |
| Documentation | 5% | 60 | 3.0 | README encoding/corruption; sparse run steps |
| Submission Speed | 5% | 70 | 3.5 | Second correct submission |
| Total |  |  | 80.0 |  |

## Score Notes
- Correctness: Absolute(1h)+Sliding(30m) caps sliding; per README, each access should extend TTL by 30 minutes without being constrained by a fixed absolute cap.
- Code Quality & Readability: Clear layering (Interfaces/Services/Models); could add comments and more expressive logging.
- Concurrency & Error Handling: Stampede prevention via `SemaphoreSlim` is good; lacks error mapping and cache event logging.
- Performance & Caching Behavior: Efficient `GetOrCreateAsync` path; minimal overhead and good reuse.
- Documentation: README has encoding/corruption issues and sparse run instructions; fix encoding and add steps.

## Strengths
- Proper `IMemoryCache` usage with stampede prevention and double-check locking.
- Clear separation into `Interfaces`, `Services`, and `Models`.
- Minimal endpoint surface (`/dashboard`) consistent with challenge sample.
- Generic cache helper (`GetOrCreateAsync`) is reusable and clean.

## Areas to Improve
- Logging and observability around cache hits/misses/evictions and errors.
- Explicit error handling/response shaping; consider problem+json for API errors.
- Propagate `CancellationToken` through factory and heavy computation.
- Revisit expiration policy per requirement intent; sliding-only or refresh absolute window.
- Fix README encoding and provide clear build/run instructions and rationale.
