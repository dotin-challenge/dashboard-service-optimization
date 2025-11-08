# Result - PR #9 ([Solution] High-Traffic Dashboard Optimization - e.soltani)
Author: `ehsan12021`
Link: https://github.com/dotin-challenge/dashboard-service-optimization/pull/9
Language: [فارسی](./result.fa.md) | English

## Summary
Console-style solution with a custom cache wrapper that stores a `CacheItem<T>` with an absolute expiration and triggers a background refresh when within a “soft expiration” window. A global `SemaphoreSlim` prevents stampede. However, TTL values and behavior diverge from the challenge: cache lifetime is set to 1 minute and soft refresh at ~40 seconds; sliding extension on each read (30 minutes) is not implemented per README.

## Score Breakdown
| Criteria | Weight | Score (0-100) | Weighted | Notes |
|---------|--------|---------------|----------|-------|
| Correctness | 40% | 50 | 20.0 | TTL is 1 minute; no 30-minute sliding extension |
| Code Quality & Readability | 25% | 80 | 20.0 | Clear layering; straightforward cache wrapper |
| Concurrency & Error Handling | 10% | 78 | 7.8 | Global lock; background refresh; limited error handling |
| Performance & Caching Behavior | 10% | 70 | 7.0 | Frequent refresh due to short TTL; possible extra work |
| Documentation | 5% | 30 | 1.5 | No README with run/usage instructions |
| Submission Speed | 5% | 0 | 0.0 | Outside top 3 |
| Total |  |  | 56.3 |  |

## Strengths
- Simple and readable cache abstraction with soft-refresh concept.
- Stampede prevention with `SemaphoreSlim`; background refresh avoids blocking readers.

## Areas to Improve
- Align TTL policy with README: 1-hour validity and 30-minute sliding extension on read.
- Consider per-key locks, structured API, and standardized error handling.
- Provide README with build/run instructions and endpoints.

## Score Notes
- Correctness: Diverges from required TTLs and sliding semantics.
- Code Quality & Readability: Clean separations and naming.
- Concurrency & Error Handling: Basic stampede control; lack of detailed error mapping/logging.
- Performance & Caching Behavior: Short TTL forces frequent recomputation.
- Documentation: Missing local documentation.
