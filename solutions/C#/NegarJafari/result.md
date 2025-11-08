# Result - PR #12 ([Solution] High-Traffic Dashboard Optimization - NegarJafari)
Author: `NegarJafari`
Link: https://github.com/dotin-challenge/dashboard-service-optimization/pull/12
Language: [فارسی](./result.fa.md) | English

## Summary
Minimal API with a `DashboardCacheService` that prevents stampede using a per-key `ConcurrentDictionary<string, Lazy<Task<DashboardDto>>>`. TTL policy: initial absolute expiration is 1 hour; on cache hit before expiry, it rewrites the entry with a new absolute expiration 30 minutes from the current read time (effectively an uncapped per-read extension). If expired but a cached value exists, it returns stale data while triggering a background regeneration, still ensuring only one regeneration occurs via the shared `Lazy`.

This aligns with the README’s intent to extend validity by 30 minutes on each access without a hard cap and avoids stampedes effectively. The stale-while-revalidate behavior is a pragmatic improvement for UX under load.

## Score Breakdown
| Criteria | Weight | Score (0-100) | Weighted | Notes |
|---------|--------|---------------|----------|-------|
| Correctness | 40% | 92 | 36.8 | Per-read 30m extension; stampede prevention; SWR behavior |
| Code Quality & Readability | 25% | 88 | 22.0 | Clean service; separate DTO; concise |
| Concurrency & Error Handling | 10% | 88 | 8.8 | Lazy<Task> per-key; basic error handling and logging |
| Performance & Caching Behavior | 10% | 90 | 9.0 | Efficient and responsive; avoids thundering herd |
| Documentation | 5% | 60 | 3.0 | README present; could add run/testing steps |
| Submission Speed | 5% | 0 | 0.0 | Outside top 3 |
| Total |  |  | 79.6 |  |

## Strengths
- Correct, uncapped per-access extension behavior without a hard absolute cap.
- Stampede prevention via shared `Lazy<Task>`; returns stale data while regenerating.
- Simple, readable code with clear responsibilities.

## Areas to Improve
- Add structured error responses and more detailed logging (hits/misses).
- Provide clear run/testing instructions and expected outputs.
- Consider eviction callbacks for observability.

## Score Notes
- Correctness: Satisfies TTL semantics and stampede prevention; SWR is a sensible enhancement.
- Code Quality & Readability: Clean and minimal; easy to follow.
- Concurrency & Error Handling: Lazy-based deduplication; generic error handling.
- Performance & Caching Behavior: Good under load; low duplication.
- Documentation: Basic; expand setup details.
