# Scaling Scenario: 10,000 Concurrent Users

## Scenario

Support 10,000 concurrent users submitting lists of up to 1,000 numbers each, plus two new capabilities: save/retrieve named lists, and an audit log of all submissions by user.

---

## Key Insight: Memoization

The conversion function is pure — same input always yields the same output. A `number_words` table stores every number we've ever converted exactly once. All saved lists reference these rows by FK rather than duplicating text. Data is immutable, so caches never need invalidation and the system gets faster with use.

---

## Database Design

```
┌──────────────┐
│    users     │
├──────────────┤
│ PK user_id   │
│    username   │
│    hashed_pass│
└──────┬───────┘
       │ 1
  ┌────┴────┐
  │ *       │ *
┌─┴──────────┐   ┌──────────────┐   ┌─────────────────┐
│ saved_lists │   │  audit_log   │   │  number_words   │
├────────────┤   ├──────────────┤   │ (memoization)   │
│ PK list_id  │   │ PK audit_id  │   ├─────────────────┤
│ FK user_id  │   │ FK user_id   │   │ PK number BIGINT│
│    list_name│   │    numbers[] │   │    word    TEXT  │
│    timestamps│   │    submitted_at│   │    is_over_9000 │
└──────┬─────┘   └──────────────┘   └─────────────────┘
       │ 1
       │ *
┌──────┴──────────┐
│saved_list_entries│──FK──▶ number_words
├─────────────────┤
│ PK list_id      │
│ PK position     │
│ FK number       │
└─────────────────┘
```

---

## API Design

| Operation | Method & Path | Notes |
|-----------|---------------|-------|
| Convert numbers | `POST /api/numberwords` | Check cache → convert misses → store new entries → return. Audit written async. |
| Save a list | `PUT /api/lists/{name}` | Upsert list + entries referencing `number_words` |
| Retrieve a list | `GET /api/lists/{name}` | JOIN entries → number_words. No recomputation. |
| List saved lists | `GET /api/lists` | Metadata only (name, count, timestamps) |
| Audit history | `GET /api/audit?from=&to=&page=` | Paginated on `(user_id, submitted_at DESC)` |

---

## Scaling Strategy

- **Conversion stays fast**: The response never waits on a DB write. Audit and new memoization entries are written asynchronously.
- **Caching**: In-memory cache (ConcurrentDictionary) → `number_words` table → compute on miss. Immutable data = no invalidation.
- **Connection pooling**: At 10K concurrent users, DB connections are the bottleneck. Async data access + connection pooler between app and DB.
- **Audit log growth**: Partition by `submitted_at` (monthly). Store only raw input numbers (not converted output) to keep rows lean.
- **Auth**: JWT-based, stateless. Short-lived access tokens validated by signature only — no DB lookup per request, keeping the connection pool free.

---

## Tradeoffs

| Decision | Benefit | Cost |
|----------|---------|------|
| Async audit writes | Fast response times | Eventual consistency (audit lags by seconds) |
| Memoization table | Deduplication, zero recomputation | Extra JOIN on retrieval; migration needed if conversion logic changes |
| Store raw numbers in audit | Lean write-heavy table | Recomputation on the rare read |
| JWT auth (no sessions) | Stateless horizontal scaling | Can't instantly revoke (mitigated by short expiry) |
