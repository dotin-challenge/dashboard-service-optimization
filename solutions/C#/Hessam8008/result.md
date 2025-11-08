# Result - PR #4 (feat: Add a solution to solve the problem.)
Author: `Hessam8008`
Link: https://github.com/dotin-challenge/dashboard-service-optimization/pull/4
Language: [فارسی](./result.fa.md) | English

## Summary
Implements a minimal endpoint `GET /{key}` that returns a string with the current time and a cached `DashboardItem`. Caching uses `IMemoryCache.GetOrCreateAsync` with `SlidingExpiration = 30 minutes` and `AbsoluteExpirationRelativeToNow = 60 minutes`. The heavy work is simulated by a 3-second delay in `DashboardItem.CreateAsync`. The code is small and separated into `DashboardRepo`, `DashboardItem`, and program setup.

Important: Per README, each access should extend the cache validity by 30 minutes from the read time. Combining absolute (1h) and sliding (30m) caps sliding by the absolute TTL and can violate the intended extension behavior on late reads. Additionally, cache stampede prevention is not implemented; concurrent misses can trigger multiple regenerations for the same key.

## Score Breakdown
| Criteria | Weight | Score (0-100) | Weighted | Notes |
|---------|--------|---------------|----------|-------|
| Correctness | 40% | 55 | 22.0 | No stampede control; Absolute+Sliding conflicts with README TTL rule |
| Code Quality & Readability | 25% | 80 | 20.0 | Small, separated files; minor naming/typos |
| Concurrency & Error Handling | 10% | 60 | 6.0 | No locking/logging; basic exception handling absent |
| Performance & Caching Behavior | 10% | 60 | 6.0 | Potential multi-run under load; unnecessary absolute cap |
| Documentation | 5% | 20 | 1.0 | No local README with run/test instructions |
| Submission Speed | 5% | 0 | 0.0 | Outside top 3 |
| Total |  |  | 55.0 |  |

## Score Notes
- Correctness: No stampede control; Absolute(1h)+Sliding(30m) caps sliding and can violate README’s per-access 30-minute extension intent.
- Code Quality & Readability: Small codebase with separation; minor typos/naming issues; returns plain string instead of structured response.
- Concurrency & Error Handling: Lacks locking on miss path and lacks error handling/logging.
- Performance & Caching Behavior: Under concurrency, multiple recomputations can occur; TTL policy could be revised to sliding-only or refreshed absolute.
- Documentation: No README; add build/run and endpoint docs.

## Strengths
- Uses `IMemoryCache` with sliding expiration aligned to the problem intent (30 minutes).
- Clear, concise code with separation between repo/model and hosting.
- Accepts and propagates `CancellationToken` to heavy work.

## Areas to Improve
- Add stampede prevention (e.g., `SemaphoreSlim` + double-check) around the cache miss path.
- Revisit TTL: prefer sliding-only or refresh absolute window on access to match README behavior.
- Introduce structured responses (JSON) and basic error handling/logging.
- Provide a README with build/run instructions and design rationale.
