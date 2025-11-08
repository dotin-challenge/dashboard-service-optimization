# Result - PR #3 (a.mansouri)
Author: `aminmansouri2000`
Link: https://github.com/dotin-challenge/dashboard-service-optimization/pull/3
Language: [فارسی](./result.fa.md) | English

## Summary
Implements a layered solution with an API (`GET /dashboard`), service (`ProductService`), and a cache abstraction (`ICacheStorageSpaceAdapter`) backed by `IMemoryCache` via `InMemoryCacheStorageSpace`. Cache stampede is prevented using a process-wide `SemaphoreSlim` and double-checking the cache. Data generation simulates heavy work with a 5-second delay. The code is organized into API/Core/Infra projects with clear responsibilities.

Important: Per README, each access should extend validity by 30 minutes from the read time. The current implementation sets `slidingExpiration = TimeSpan.FromSeconds(30)` instead of 30 minutes, and also combines absolute (1h) + sliding, which caps sliding by the absolute TTL. These conflict with the required TTL behavior.

## Score Breakdown
| Criteria | Weight | Score (0-100) | Weighted | Notes |
|---------|--------|---------------|----------|-------|
| Correctness | 40% | 65 | 26.0 | Sliding set to 30 seconds; absolute+sliding cap |
| Code Quality & Readability | 25% | 85 | 21.25 | Good layering; clear abstractions |
| Concurrency & Error Handling | 10% | 85 | 8.5 | SemaphoreSlim OK; limited API error mapping |
| Performance & Caching Behavior | 10% | 80 | 8.0 | TTL misconfig increases recomputation |
| Documentation | 5% | 30 | 1.5 | No local README with run steps |
| Submission Speed | 5% | 40 | 2.0 | Third correct submission window |
| Total |  |  | 67.25 |  |

## Score Notes
- Correctness: Sliding is configured as 30 seconds (should be 30 minutes). Combining Absolute(1h)+Sliding caps extensions and conflicts with README’s per-access 30-minute extension.
- Code Quality & Readability: Solid layering and abstractions; Redis adapter is present but not applicable—mark clearly as out of scope for this challenge.
- Concurrency & Error Handling: Stampede protection with `SemaphoreSlim`; limited controller-level error handling and no standardized error responses.
- Performance & Caching Behavior: Misconfigured TTL causes more recomputation under load; otherwise efficient path and reuse of cached data.
- Documentation: Missing local README with build/run instructions and design rationale.

## Strengths
- Proper cache abstraction with `IMemoryCache` implementation; clean separation of concerns.
- Stampede prevention using `SemaphoreSlim` with double-check pattern.
- Simple controller surface and Swagger for quick verification.

## Areas to Improve
- TTL policy: use a 30-minute sliding expiration per README (not 30 seconds), and avoid capping with absolute TTL if unlimited extension is intended.
- Add controller-level error handling or standard API error shape (e.g., problem+json) and propagate `CancellationToken`.
- Provide a local README with build/run, endpoint usage, and design choices.
- Remove or clearly mark the Redis adapter as non-applicable for this challenge to avoid confusion.
