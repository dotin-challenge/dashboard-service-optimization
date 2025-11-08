# Challenge Result — High‑Traffic Dashboard Optimization

Repository: https://github.com/dotin-challenge/dashboard-service-optimization
Assessment Date: 2025-11-08

Language: [فارسی](./result.fa.md) | English

## Evaluation Criteria (from README)

| Criteria                       | Weight |
|--------------------------------|--------|
| Correctness                    | 40%    |
| Code Quality & Readability     | 25%    |
| Concurrency & Error Handling   | 10%    |
| Performance & Caching Behavior | 10%    |
| Documentation                  | 5%     |
| Submission Speed (PR Time)     | 5%     |

Notes
- “Submission Speed” awarded discretely: 1st=100, 2nd=70, 3rd=40 (weighted by 5%). Only applied to solutions that satisfy correctness requirements.
- TTL semantics enforced per README: 1h validity, extend by 30m on each access without a hard absolute cap.

## Final Leaderboard

| Rank | PR | Author | Total | Link |
|------|----|--------|-------|------|
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

Speed Awards (applied in totals)
- 1st to submit a correct solution: arash-mosavi (100 → +5.0 weighted)
- 2nd: ShahramAfshar (70 → +3.5 weighted)
- 3rd: aminmansouri2000 (40 → +2.0 weighted)

## Top 3 Highlights
- PR #13 — HamedMortezaee
  - Correct per‑read 30‑minute extension without a hard cap; proactive refresh near expiry.
  - Strong concurrency model separating build vs. refresh locks; clear error/cancellation handling (499 mapping).
  - Clean layering and logging; efficient hit path and minimal contention.

- PR #1 — arash-mosavi
  - Solid stampede prevention with `SemaphoreSlim` + double‑check; clear service/controller shape and logs.
  - TTL uses Absolute(1h)+Sliding(30m); consider refreshing absolute on reads (or sliding‑only) to fully match README.
  - Simple, structured JSON responses; maintainable baseline.

- PR #2 — ShahramAfshar
  - Clean layering with a reusable cache helper (`GetOrCreateAsync`).
  - Stampede prevention with `SemaphoreSlim`; TTL currently capped by absolute — align to per‑read extension.
  - Add richer logging/observability; fix README encoding.

## Per‑PR Results
- PR #1 (arash-mosavi): results → `solutions/C#/arash-mosavi/result.md`, `solutions/C#/arash-mosavi/result.fa.md`
- PR #2 (ShahramAfshar): results → `solutions/C#/shahramafshar/result.md`, `solutions/C#/shahramafshar/result.fa.md`
- PR #3 (aminmansouri2000): results → `solutions/C#/aminmansouri2000/result.md`, `solutions/C#/aminmansouri2000/result.fa.md`
- PR #4 (Hessam8008): results → `solutions/C#/Hessam8008/result.md`, `solutions/C#/Hessam8008/result.fa.md`
- PR #5 (Saffarnejad): results → `solutions/C#/Saffarnejad/result.md`, `solutions/C#/Saffarnejad/result.fa.md`
- PR #6 (KevinMKM): results → `solutions/C#/KevinMKM/result.md`, `solutions/C#/KevinMKM/result.fa.md`
- PR #7 (r-poorbageri): results → `solutions/C#/r-poorbageri/result.md`, `solutions/C#/r-poorbageri/result.fa.md`
- PR #9 (ehsan12021): results → `solutions/C#/ehsan12021/result.md`, `solutions/C#/ehsan12021/result.fa.md`
- PR #10 (agaheman): results → `solutions/C#/agaheman/result.md`, `solutions/C#/agaheman/result.fa.md`
- PR #11 (parsapanahpoor): results → `solutions/C#/parsapanahpoor/result.md`, `solutions/C#/parsapanahpoor/result.fa.md`
- PR #12 (NegarJafari): results → `solutions/C#/NegarJafari/result.md`, `solutions/C#/NegarJafari/result.fa.md`
- PR #13 (HamedMortezaee): results → `solutions/C#/HamedMortezaee/result.md`, `solutions/C#/HamedMortezaee/result.fa.md`

## Observations
- TTL semantics: Highest‑scoring solutions either refreshed absolute on each read or used sliding‑only to honor the 30‑minute per‑read extension.
- Anti‑stampede patterns: Per‑key `SemaphoreSlim` and `Lazy<Task<T>>` effectively eliminated thundering herds; per‑key locks scale better than process‑wide locks.
- Concurrency & errors: Propagating `CancellationToken` and returning structured errors improved robustness and clarity under load.
- Performance behavior: Double‑check locking and slim critical sections minimized recomputation; background refresh (SWR) reduced cold starts.
- Documentation & DX: Short, accurate READMEs with run/test steps and design notes raised confidence; watch out for encoding issues.
