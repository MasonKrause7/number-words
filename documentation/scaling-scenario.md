# Scaling Scenario: 10,000 Concurrent Users

## Requirements

1. Support 10,000 concurrent users submitting lists of up to 1,000 numbers
2. Save and retrieve named lists
3. Audit log of all submissions by user

---

## Current State

The conversion endpoint is stateless, CPU-bound, and fast — O(n log n) per request with ~200 KB of short-lived allocations. No I/O. This should handle 5,000-15,000 reqs/sec with latency in milliseconds.

Adding persistence shifts the bottleneck from CPU to I/O.

---

## Database Design

### ERD

```
┌─────────────────────────┐
│         users           │
├─────────────────────────┤
│ PK  user_id       UUID  │
│     username      TEXT  │  UNIQUE
|     hashed_pass   TEXT  |
│     created_at    TIMESTAMPTZ │
└────────┬────────────────┘
         │ 1
         │
    ┌────┴─────────────────────┐
    │ *                        │ *
┌───┴─────────────────┐  ┌────┴──────────────────────┐
│    saved_lists      │  │       audit_log           │
├─────────────────────┤  ├───────────────────────────┤
│ PK  list_id   UUID  │  │ PK  audit_id      UUID    │
│ FK  user_id   UUID  │  │ FK  user_id       UUID    │
│     list_name TEXT  │  │     numbers       BIGINT[]│
│     created_at TIMESTAMPTZ│  │     result_count  INT    │
│     updated_at TIMESTAMPTZ│  │     submitted_at  TIMESTAMPTZ│
└───┬─────────────────┘  └───────────────────────────┘
    │ 1
    │
    │ *
┌───┴─────────────────────┐       ┌─────────────────────────────┐
│  saved_list_entries     │       │       number_words          │
├─────────────────────────┤       │  (global memoization table) │
│ PK  list_id  UUID (FK)  │       ├─────────────────────────────┤
│ PK  position SMALLINT   │──FK──▶│ PK  number       BIGINT     │
│ FK  number   BIGINT     │       │     word         TEXT        │
└─────────────────────────┘       │     is_over_9000 BOOLEAN    │
                                  └─────────────────────────────┘
```

### Key Insight: Global Memoization

The conversion function is pure — same input always yields the same output. `number_words` stores every number we've ever converted exactly once. All saved lists reference these rows via FK rather than duplicating the converted text.

- Storage is deduplicated across all users
- The table converges toward a hot working set over time
- Combined with an in-memory cache, most requests at scale hit zero computation and zero DB I/O for previously-seen numbers
- Data is immutable → cache never needs invalidation

---

## Access Patterns

| Operation | Path | Strategy |
|-----------|------|----------|
| Convert numbers | `POST /api/numberwords` | Check cache/`number_words` → convert misses → store new entries → return. Audit written async. |
| Save a list | `PUT /api/lists/{name}` | Upsert `saved_lists` + insert `saved_list_entries` referencing `number_words` |
| Retrieve a list | `GET /api/lists/{name}` | JOIN `saved_list_entries` → `number_words`. No recomputation. |
| List all saved lists | `GET /api/lists` | Metadata only (name, count, timestamps) |
| Audit history | `GET /api/audit?from=&to=&page=` | Paginated query on `(user_id, submitted_at DESC)` index |
| Audit detail | `GET /api/audit/{id}` | Returns raw numbers; reconvert on demand (write-heavy table, read-rare) |

---

## Scaling Strategy

### Keep conversion fast
The conversion response never waits on a DB write. Audit entries are written asynchronously. New conversions are written asynchronously.

### Connection pool management
At 10K concurrent users, DB connections are the finite resource. Mitigations:
- Async data access (no thread blocking)
- Connection pooler between app and DB

### Caching layer
```
Request → In-memory cache (ConcurrentDictionary / IMemoryCache)
              ↓ miss
          number_words table
              ↓ miss
          ConvertNumberToWord() → write to table + cache
```
Immutable data = no invalidation needed. System gets faster with use.

### Audit log growth
- Partition `audit_log` by `submitted_at` (monthly)
- Store only raw input (not converted output) — keeps rows lean
- Archive/drop old partitions per retention policy

---

## Authentication

JWT-based, stateless auth:
- Short-lived access tokens (15–30 min) validated by signature only — no DB lookup per request
- `user_id` extracted from token claims, used to scope all queries
- ASP.NET Core: `AddAuthentication().AddJwtBearer()` middleware

Why JWTs at this scale: auth validation doesn't touch the database, keeping the connection pool free for operations that actually need persistence.

---

## Tradeoffs

| Decision | Benefit | Cost |
|----------|---------|------|
| Async audit writes | Fast response times | Eventual consistency (audit lags by seconds) |
| `number_words` memoization table | Deduplication, zero recomputation | Extra JOIN on retrieval, migration needed if conversion logic changes |
| Store raw numbers in audit (not words) | Lean write-heavy table | Recomputation on the rare read |
| JWT auth (no sessions) | Stateless horizontal scaling | Can't instantly revoke (mitigated by short expiry) |

