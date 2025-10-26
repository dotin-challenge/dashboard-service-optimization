# High-Traffic Dashboard Optimization

[![Difficulty](https://img.shields.io/badge/difficulty-medium-orange)]()
[![Languages](https://img.shields.io/badge/languages-C%23-informational)]()
[![Deadline](https://img.shields.io/badge/deadline-2025--12--02-critical)]()

> Design a resilient caching strategy for a high-demand dashboard service to prevent concurrent heavy loads when cached data expires.

---

## Table of Contents

- [High-Traffic Dashboard Optimization](#high-traffic-dashboard-optimization)
  - [Table of Contents](#table-of-contents)
  - [Requirements](#requirements)
  - [Problem Description](#problem-description)
  - [Rules and Constraints](#rules-and-constraints)
  - [Example Behavior](#example-behavior)
  - [How to Run \& Test](#how-to-run--test)
    - [1) Clone the Project](#1-clone-the-project)
    - [2) Build \& Run](#2-build--run)
      - [▶ C#](#-c)
    - [3) Simulate Concurrent Load (Optional)](#3-simulate-concurrent-load-optional)
  - [How to Submit (PR)](#how-to-submit-pr)
  - [Evaluation Criteria](#evaluation-criteria)
  - [Timeline](#timeline)
  - [Contact](#contact)

---

## Requirements

* Allowed Languages: **C#**
* Recommended Versions: `.NET 6+`
* OS: Any (Windows / Linux / macOS)
* Familiarity with:

  * Caching using `IMemoryCache`
  * Thread-safety and concurrency control
  * Basic API or console application setup

---

## Problem Description

You are a **backend developer** in an Enterprise software company. One of the most frequently used parts of the system is a **dashboard service** that displays key analytical data. Generating this data requires a **complex analytical computation** that takes several seconds to complete.

**The problem:**
During peak hours, many users access the dashboard simultaneously. When the cached data expires, **multiple concurrent requests** trigger the same heavy computation at once. This leads to high CPU load and degraded performance.

Your goal is to design and implement a **resilient caching mechanism** that prevents such performance degradation.

---

## Rules and Constraints

1. Use `IMemoryCache` to cache dashboard data.
2. Cached data must remain valid for **1 hour**.
3. If data is accessed before expiry, **extend its lifetime by 30 minutes** from the current read time.
4. Prevent the **cache stampede** problem — when cache expires, only **one request** should regenerate data.
5. No external distributed caches (Redis, Memcached, etc.) may be used.
6. Focus on backend performance and correctness — not UI or visuals.

---

## Example Behavior

**Scenario:**

1. User A requests the dashboard → cache miss → heavy computation runs → result cached for 1 hour.
2. User B requests 5 minutes later → cache hit → instant response.
3. When cache is about to expire, multiple users hit the dashboard simultaneously — only **one** regenerates new data.
4. Each successful read extends the cache validity by 30 minutes.

---

## How to Run & Test

### 1) Clone the Project

```bash
git clone https://github.com/dotin-challenge/dashboard-cache.git
cd dashboard-cache
```

### 2) Build & Run

#### ▶ C#

```bash
dotnet build
dotnet run
```

### 3) Simulate Concurrent Load (Optional)

You can simulate concurrent requests using:

```bash
ab -n 100 -c 10 http://localhost:5000/dashboard
```

---

## How to Submit (PR)

1. **Fork** the repository.
2. Create a new branch:

   ```bash
   git checkout -b solution/<username>
   ```
3. Place your solution inside:

   ```text
   solutions/C#/<username>/
     ├─ Program.cs
     └─ README.md
   ```
4. Example:

   ```text
   solutions/C#/john-doe/
     ├─ Program.cs
     └─ README.md
   ```
5. Open a Pull Request titled:

   ```text
   [Solution] High-Traffic Dashboard Optimization - <username>
   ```

---

## Evaluation Criteria

| Criteria                       | Weight |
| ------------------------------ | ------ |
| Correctness                    | 40%    |
| Code Quality & Readability     | 25%    |
| Concurrency & Error Handling   | 10%    |
| Performance & Caching Behavior | 10%    |
| Documentation                  | 5%     |
| **Submission Speed (PR Time)** | **5%** |

> The earlier you submit a correct and working PR before the deadline, the higher your chance to earn these extra 5%.

---

## Timeline

* Start: `2025-11-26`
* Deadline: `2025-12-02`

---

## Contact

* **GitHub Issues**
* **.NET Community Group**
