# Result - PR #10 (High-Traffic Dashboard Optimization - H.Shamekhi)
Author: `agaheman`
Link: https://github.com/dotin-challenge/dashboard-service-optimization/pull/10
Language: [فارسی](./result.fa.md) | English

## Summary
ASP.NET Core minimal API with `IMemoryCache` and a global `SemaphoreSlim` to prevent stampede. On cache miss, data is generated (3s) and cached with `AbsoluteExpirationRelativeToNow = 1 hour`. On cache hit, the code rewrites the entry with `AbsoluteExpirationRelativeToNow = 30 minutes`, effectively shortening the TTL to 30 minutes rather than extending by 30 minutes and maintaining the 1-hour validity requirement stated in the README.

## Score Breakdown
| Criteria | Weight | Score (0-100) | Weighted | Notes |
|---------|--------|---------------|----------|-------|
| Correctness | 40% | 55 | 22.0 | Hit path sets absolute to 30m; sliding not used; conflicts with README |
| Code Quality & Readability | 25% | 82 | 20.5 | Clear service and endpoint; good logging |
| Concurrency & Error Handling | 10% | 85 | 8.5 | Global semaphore + double-check; problem response on error |
| Performance & Caching Behavior | 10% | 75 | 7.5 | No sliding; TTL shortens on access; extra rewrites |
| Documentation | 5% | 40 | 2.0 | No local README with run/usage details |
| Submission Speed | 5% | 0 | 0.0 | Outside top 3 |
| Total |  |  | 60.5 |  |

## Strengths
- Prevents stampede with lock + double-check.
- Simple endpoint returning structured JSON; logging with timing.

## Areas to Improve
- Align TTL behavior: enforce 1-hour validity and extend 30 minutes on read via sliding; avoid shortening TTL.
- Consider per-key locks and add eviction callbacks for observability.
- Provide a README with run/testing instructions.

## Score Notes
- Correctness: TTL semantics diverge from README; sliding not configured.
- Code Quality & Readability: Clean and readable; service abstraction is straightforward.
- Concurrency & Error Handling: Uses semaphore and returns 500 problem on errors.
- Performance & Caching Behavior: Lack of sliding and TTL shortening can lead to unexpected expirations.
- Documentation: Missing local documentation.
