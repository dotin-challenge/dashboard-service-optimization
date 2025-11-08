# Result - PR #5 (Saffarnejad)
Author: `Saffarnejad`
Link: https://github.com/dotin-challenge/dashboard-service-optimization/pull/5
Language: [فارسی](./result.fa.md) | English

## Summary
Console-hosted implementation using `IMemoryCache` with cache stampede prevention via a process-wide `SemaphoreSlim` and double-checking. On cache hit, the code explicitly re-writes the cache entry with `SlidingExpiration = 30 minutes` and `AbsoluteExpirationRelativeToNow = 1 hour`, effectively refreshing both windows on every read. Heavy work is simulated (3–5s). Eviction logging is hooked via `RegisterPostEvictionCallback`.

Per README, each access should extend validity by 30 minutes from the read time. This implementation achieves the sliding extension and, by refreshing the absolute window on access, avoids a hard absolute cap. The periodic re-write on hit adds a small overhead but keeps semantics aligned with the requirement.

## Score Breakdown
| Criteria | Weight | Score (0-100) | Weighted | Notes |
|---------|--------|---------------|----------|-------|
| Correctness | 40% | 92 | 36.8 | Sliding 30m + refreshed absolute; stampede prevented |
| Code Quality & Readability | 25% | 82 | 20.5 | Multiple types in one file; clear naming; concise |
| Concurrency & Error Handling | 10% | 88 | 8.8 | `SemaphoreSlim` + double-check; minimal error handling |
| Performance & Caching Behavior | 10% | 88 | 8.8 | Small overhead from re-set on hit; good behavior |
| Documentation | 5% | 50 | 2.5 | README present but encoding/corruption issues |
| Submission Speed | 5% | 0 | 0.0 | Outside top 3 |
| Total |  |  | 77.4 |  |

## Strengths
- Prevents cache stampede with lock + double-check.
- Refreshes TTL on read to match 30-minute sliding intent without a hard cap.
- Eviction callback and timing logs aid observability.
- Clean, minimal code; easy to follow.

## Areas to Improve
- Split classes (`DashboardService`, `DashboardData`, `Program`) into separate files; consider API surface if desired.
- Add structured error handling/logging; propagate `CancellationToken`.
- Consider avoiding re-set on hit by relying on sliding expiration alone unless absolute refresh is strictly needed.
- Fix README encoding and expand build/run/testing instructions.

## Score Notes
- Correctness: Sliding 30m is honored and absolute is refreshed on access, avoiding a cap; aligns with README intent.
- Code Quality & Readability: Single source file carries several types; otherwise, naming and structure are clear.
- Concurrency & Error Handling: Locking correct; error handling minimal for a console app; logging via console only.
- Performance & Caching Behavior: Re-setting cache on hit incurs minor overhead; behavior is otherwise efficient.
- Documentation: README exists but has encoding issues; include clear steps and rationale.
