# FsLit

## What This Is

FsLit is an F# test runner inspired by LLVM lit, designed for compiler and interpreter testing. It reads `.flt` test files with embedded command, input, and expected output sections, then executes and verifies them. Currently at v0.2.0 with basic command execution, input substitution, and exact output matching.

## Core Value

Reliable, declarative test verification — a test file fully describes what to run, what to feed it, and what to expect, with no external configuration needed.

## Requirements

### Validated

- ✓ Parse `.flt` files with `// --- Command:`, `// --- Input:`, `// --- Output:` sections — existing
- ✓ Execute commands via shell with variable substitution (`%input`, `%output`, `%s`, `%S`) — existing
- ✓ Exact line-by-line output comparison with mismatch reporting — existing
- ✓ Recursive test file discovery in directories — existing
- ✓ Cross-platform support (Windows cmd.exe / Unix sh) — existing
- ✓ Temp file management for input/output — existing
- ✓ `--help` flag — existing

### Active

- [ ] Exit code checking via `// --- ExitCode: N` directive (ignore if unspecified)
- [ ] Stderr checking via `// --- Stderr:` section (contains-match: expected lines must appear, extra OK)
- [ ] Timeout support via `// --- Timeout: N` directive (kill command after N seconds)
- [ ] `--verbose` flag to show actual vs expected output on failure
- [ ] `--filter` flag with glob pattern to run subset of tests

### Out of Scope

- Multiple test cases per file — adds parser complexity, not needed now
- Regex/pattern matching (`CHECK:` / `CHECK-RE:`) — significant feature, defer to future
- `CHECK-NOT` — depends on pattern matching infrastructure
- Parallel test execution — optimization, not a correctness feature
- F# idiomatic refactoring (fold-based parser, railway-oriented error handling) — code quality, not user-facing

## Context

- F# on .NET 10, built with `dotnet build`
- 6 source modules: Types, Parser, Runner, Checker, Substitution, Program
- Runner already captures ExitCode and Stderr but they're unused
- Parser uses mutable state (imperative style) — functional refactor deferred
- 2 existing test files (echo.flt, input.flt)

## Constraints

- **Tech stack**: F# / .NET 10 — existing project, no change
- **Compatibility**: New directives must be optional and backward-compatible with existing `.flt` files
- **Naming**: Follow existing `// --- Directive:` convention for new directives

## Key Decisions

| Decision | Rationale | Outcome |
|----------|-----------|---------|
| Exit code ignored when unspecified | Backward compatibility — existing tests don't specify exit codes | — Pending |
| Stderr uses contains-match | Stderr often has extra noise (warnings, debug info); exact match too brittle | — Pending |
| Filter uses glob patterns | More flexible than substring, familiar to CLI users | — Pending |

---
*Last updated: 2026-02-16 after initialization*
