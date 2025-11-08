# Result - PR #6 ([Solution] High-Traffic Dashboard Optimization - M.KoraniMaskan)
Author: `KevinMKM`
Link: https://github.com/dotin-challenge/dashboard-service-optimization/pull/6
Language: [فارسی](./result.fa.md) | English

## Summary
Implements a robust per-key locking cache (`ResilientMemoryCache`) over `IMemoryCache`, preventing stampede with a `ConcurrentDictionary<string, SemaphoreSlim>` and double-check logic. The cache wraps values with an absolute expiry timestamp and uses `SlidingExpiration = 30 minutes`. On cache hit, it refreshes sliding but preserves the original absolute expiry. API endpoint: `GET /dashboard`, with simulated work (3s) and cancellation support.

Per README, each access should extend validity by 30 minutes from the read time. Preserving a fixed absolute cap prevents extension beyond the initial hour, which conflicts with the intended behavior. Consider refreshing the absolute window on access or relying on sliding-only.

## Score Breakdown
| Criteria | Weight | Score (0-100) | Weighted | Notes |
|---------|--------|---------------|----------|-------|
| Correctness | 40% | 82 | 32.8 | Absolute cap maintained; conflicts with README TTL rule |
| Code Quality & Readability | 25% | 90 | 22.5 | Clean design, per-key locks, good logging/comments |
| Concurrency & Error Handling | 10% | 92 | 9.2 | Per-key `SemaphoreSlim`, cancellation respected, error logging |
| Performance & Caching Behavior | 10% | 88 | 8.8 | Efficient path; minimal overhead; lock cleanup |
| Documentation | 5% | 70 | 3.5 | Clear rationale; run steps are a bit ad-hoc |
| Submission Speed | 5% | 0 | 0.0 | Outside top 3 |
| Total |  |  | 76.8 |  |

## Strengths
- Solid per-key locking strategy; avoids global bottlenecks and stampede.
- Cancellation and error handling considered; logs failures.
- Clear separation of concerns within a single file; readable code.

## Areas to Improve
- Align TTL semantics with README by refreshing absolute window on read or using sliding-only.
- Provide straightforward run instructions (no need to scaffold a new project).
- Consider extracting types into separate files for maintainability.

## Score Notes
- Correctness: Sliding honored, but absolute cap remains fixed; this can violate the per-access 30-minute extension intent.
- Code Quality & Readability: Well-structured; pragmatic logging; per-key lock cleanup is a plus.
- Concurrency & Error Handling: Good double-check and cancellation propagation; errors logged.
- Performance & Caching Behavior: Minimal overhead; key-scoped locks reduce contention.
- Documentation: Strong design explanation; simplify run guidance for direct usage.
