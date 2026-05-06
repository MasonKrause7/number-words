# AI.md

## Tools Used

**Cursor IDE** — used as an AI-augmented copilot in early development to assist with project setup.

**Kiro IDE** - once project structure was established, generated steering docs and used chat interface to iterate on styling and frontend functionality. Provided specific references to relevant files and context while prompting to give accurate full stack context.

### Why Copilot Mode, Not Spec-Driven Development

I did *not* use a spec-driven workflow for the setup of this project and this early functionality. Spec-driven development shines in established codebases where there's existing context, patterns, and architecture that the AI needs to understand and build upon. For a new project like this, I find that manual implementation is preferable for the foundational work — establishing project structure, wiring up connections between layers, and making early architectural decisions. AI still boosted productivity significantly as a copilot (quick lookups, rubber-ducking design choices), but I drove the structure and decisions myself to ensure everything was connected correctly from the start.

If this project were to grow to add more features, that's where I'd shift into spec-driven mode, because the AI would then have meaningful context to reason about. Part 2 of the interview should give a chance to showcase the spec-driven workflow live.

---

## What I Decided and Built Manually

### Tech Stack
- **C# / .NET 10 backend** — Considered a serverless function on AWS Lambda (would probably use that in production for a stateless on-demand function like this), but chose a lightweight API hosted on Elastic Beanstalk to demonstrate project structure and learn more about the framework. Read and followed official ASP.NET core documentation.
- **Vite + React (TypeScript) frontend** — Handles validation with reactive feedback before calling the API.

### Backend Architecture (hands-on, light QA in chat, heavy human oversight)
- Scaffolded with `dotnet new webapi`.
- Created `/Controllers` with `NumberWordsController.cs`, following [ASP.NET MVC docs](https://learn.microsoft.com/en-us/aspnet/core/mvc/overview?view=aspnetcore-10.0).
- Designed the `/Models` directory structure:
  - `/RequestDtos` — inbound request shapes with DataAnnotation validation per [official docs](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations?view=net-10.0)
  - `/ResponseDtos` — outbound response shapes
  - `/DomainModel` — business logic (conversion service, domain types)

### Frontend Architecture (hands-on, light QA in chat, heavy human oversight)
- Scaffolded with `npm create vite@latest frontend -- --template react-ts`.
- Built the page component (`Home.tsx`) with function signatures for full feature logic (`handleInputChange`, `handleSubmit`, `parseAndValidateNumbers`), then iteratively prompted with a function description for each, testing in parallel. Then prompted to refactor into smaller components (`NumberForm`, `ErrorList`, `ResultsList`) for better structure and maintainability. 
- Wrote the input validation requirements and prompted to implement them in a utility (`parseAndValidateNumbers.ts`).

### API Design
- Single endpoint: `POST /api/numberwords` with `{"numbers": [...]}` body
- Controller → Service pattern with DI registration
- Request/response contract designed for clarity and extensibility (`NumberWordItem` includes original number, word, and the over-9000 flag)

---

## Where AI Added the Most Value

1. **C# syntax and idioms** — Since I'm more experienced in Java/Python, Cursor was invaluable for quickly getting idiomatic C# (array initializer syntax, DataAnnotation attributes, DI registration).
2. **CSS styling** — AI generated the bulk of the visual styling. I directed the aesthetic (clean, card-based, minimal) and it produced the CSS variables, layout, and hover interactions.
3. **Low level function implementation** — The most challenging portion of the task was the number to word conversion. I let AI do the heavy lifting to brainstorm several approaches to the algorithm and then iterated a few times to polish it until the agent and I agreed that we had the best approach overall.
4. **Rubber-ducking the scaling document** — Used chat to think through the considerations of scaling and different data modeling and caching approaches.

---

## Where AI Fell Short

1. **Project structure decisions** — AI suggestions for file organization were over simplified and agents repeatedly tried to put all the backend logic in one Program.cs file. I had to manually establish the `/Models/DomainModel`, `/RequestDtos`, `/ResponseDtos` separation and the monorepo layout. Once I got the backend mostly established and the scaffolding for the frontend with the basic Home page layed out, I was able to switch over to Kiro and generate some steering docs which helped increase iteration speed.
2. **Integration wiring** — CORS configuration, frontend-to-backend connection, and the JSON serialization approach for Int64 values all required manual attention. 
3. **Edge case awareness** — AI didn't proactively flag the `long.MinValue` overflow or add tests for this edge case initially. These were discovered through manual testing of boundary values.
4. **Scaling scenario — database design** — AI defaulted to a naive schema: `saved_lists` with a raw `long[]` column and `audit_log` with another `log[]`. No deduplication, no precomputation, no awareness of read vs. write access patterns. I had to drive the design through two iterations of refinement:
   - **Round 1**: I pointed out that `saved_lists` is read-heavy (store converted results to avoid reconverting on every login), while `audit_log` is write-heavy and rarely read (store only raw numbers, reconvert on demand). The original proposed approach would've caused excessive, unnecessary traffic on the API everytime a users lists were loaded and reconverted which would cause major performance impact at scale. AI's response was to add a `JSONB converted_items` column — better, but still duplicating converted text across every user's list resulting in high memory usage (db cost) and inefficienct storage patterns.
   - **Round 2**: I identified that conversion is a pure function (same input → same output, always), and that there is a relatively fixed domain (input set). So there's no reason to store the same word string thousands of times for different lists or different users. I proposed a shared `number_words` lookup table keyed by the number itself, with saved lists referencing rows via FK. AI then fleshed out the low level details such as the junction table and query patterns — but the core memoization insight came from me. I also recommended an in-memory caching layer for fast lookups instead of db reads for already converted numbers.
   
   This was one of the strongest examples of how I interact with AI. It expands effectively once direction is set, but gravitates toward naive solutions. The highest-value decision was the memoization of converted values and I don't think AI would have come up with that solution. Similarly, the asymmetric access patterns and unnecessary strain on the API with AIs proposed solution would've caused major performance issues if naiively followed.

---

## Concrete Override: Int64.MinValue Overflow

While testing boundary values, I sent `−9223372036854775808` (`Int64.MinValue`) through the app and the server crashed. Working with AI, we identified a two-layer bug:

1. **Backend**: `Math.Abs(long.MinValue)` overflows because `|long.MinValue|` exceeds `long.MaxValue`. The AI's initial suggestion was to add a simple `if` check and return a hardcoded string. I directed it toward the correct fix: cast to `ulong` before processing so the existing chunk-based algorithm handles it naturally.

2. **Frontend serialization**: The original `values.map(Number)` silently lost precision for 64-bit integers (JS doubles only have 53 bits of integer precision). AI could not figure out what was going wrong, and once I traced the issue back to that type, AIs recommendation was to use BigInt instead. That solution led to server errors as the backend only supports Long type values. Ultimately, I chose to keep values as validated strings and build the JSON body as a raw string literal (`{"numbers":[${values.join(",")}]}`), so numeric literals hit the wire without JavaScript float conversion.

Both fixes demonstrate a pattern: AI provides a surface level diagnosis quickly, but its first-pass solutions are either too narrow (hardcoded edge case) or too broad (rewrite with BigInt). Even with clear steering docs and prompts that reference specific context, in the second case AI still produced a breaking change which seemingly disregarded API limitations. Engineering judgment was needed to find the correct fix. As the system grows in complexity, the importance of overseeing influential decisions made in the code base also grows.

---

## Concrete Override: Duplicate Zero Rendering

With input like `-0, 7, 0, 5, 000`, the frontend was rendering duplicate "Zero" entries in the wrong order — some items were missing and others appeared twice. I asked AI to fix it several times and it kept proposing changes to the sorting logic or the conversion service, none of which resolved the issue.

I used the browser Network tab and compared the request/response bodies. The backend was returning the correct data — three distinct "Zero" items sorted properly. I traced the problem to the `ResultsList.tsx` component. The `key` prop on each list item was derived from `item.originalNumber`-`item.word`, and since `-0`, `0`, and `000` all convert to the same number (`0`) and the same word (`"Zero"`), React was seeing duplicate keys and deduplicating/reordering the rendered elements.

Once I identified the root cause and explained it to AI — "the key needs to be unique per list position, not per value" — it immediately produced the correct fix: using the array index as part of the key (`${index}-${item.originalNumber}-${item.word}`). The AI couldn't find this on its own because it kept looking at the data layer rather than the rendering layer. Knowing *where* to look and how to trace the request flow was the human contribution; generating the fix once pointed in the right direction was trivial for AI.
