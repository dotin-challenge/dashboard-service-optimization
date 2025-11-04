# Dashboard Service Optimization - Solution

This project brings a tough, in-memory caching system to life—one that actually stands up to high traffic and avoids the usual headaches like cache stampedes or accidental data leaks. It handles both absolute and sliding expirations the right way, and it’s built to run safely even when a bunch of requests hit at once. The code’s tidy, easy to follow, and ready for production.

Why We Built It This Way:
- Per-key SemaphoreSlim: For each cache key, we use a separate lock. That way, if a bunch of users ask for the same dashboard, only one does the heavy lifting, and the rest just wait for the result. No global lock slowing everything down.
- Wrapper with absolute expiry: Every cached value stores its own absolute expiration from the start. Sliding expiration keeps things alive if they’re used, but nothing ever outlives its absolute limit.
- Only caching successes: If generating the data fails, we don’t cache anything. Next time, we just try again—no bad results stuck in memory.
- Simulated latency: `await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);` fakes the kind of delay you’d see from a real dashboard service. This helps us see if the cache stands up under pressure. In real life, you wouldn’t keep this delay.
- Cancellation support: Every async call checks the cancellation token, so if someone cancels a request, we bail out right away.
- Semaphore cleanup: We make sure to dispose locks as soon as we’re done with them. That keeps memory use under control.

Fixing Common Problems:
A lot of caching systems slip up in ways that are easy to miss. Here’s how we dodge those traps:
-------------------------------------------------------------------------------------------------------------------------------
| Problem                      | Typical Mistake                         | How We Fix It                                      |
|------------------------------|-----------------------------------------|----------------------------------------------------|
|Resetting absolute expiration | Data lasts longer than intended         | Absolute expiration is locked in and never reset   |
|Caching errors                | Failures end up stuck in cache          | Only successful results get stored                 |
|Global locks                  | System slows down under load            | Each key gets its own lock, so no bottleneck       |
|Memory leaks from locks       | Lock objects pile up over time          | We dispose locks once they’re not needed           |
|Storing Task<T> in cache      | Errors or old data get replayed         | We only store the actual result, not the task      |
|Ignoring cancellation         | Unnecessary CPU use and blocked threads | Requests respect cancellation from start to finish |
|No simulated latency          | Can’t see how system handles a rush     | We add a delay to test real-world conditions       |
-------------------------------------------------------------------------------------------------------------------------------

Folder Structure:
solutions/C#/M.KoraniMaskan/
  ├─ Program.cs
  └─ README.md

How to Run:
cd solutions/C#/M.KoraniMaskan
dotnet new webapi -n DashboardApp
Replace DashboardApp/Program.cs with Program.cs
dotnet run

Endpoint:
GET /dashboard

Expected Behavior:
------------------------------------------------------------------------------------------
| Scenario             | What Happens                                                    |
|----------------------|-----------------------------------------------------------------|
|Many requests at once | Only one does the heavy work, others wait for cache             |
|Factory throws error  | Nothing is cached, next try starts fresh                        |
|Cancellation          | We stop right away, nothing gets cached by accident             |
|Frequent reads        | Sliding TTL updates, but absolute TTL never goes past its limit |
------------------------------------------------------------------------------------------

Example Output:
{
  "title": "Main Dashboard",
  "generatedAt": "2025-10-31T12:00:00Z",
  "data": "Aggregated payload"
}

Summary:
This system blocks stampedes, handles expiration the right way, and never caches failures. The code stays clean and easy to work with. The fake delay proves it can handle real load without breaking a sweat. Every design choice here helps keep the cache reliable, efficient, and easy to trust in production.