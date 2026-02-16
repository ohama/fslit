# FsLit

## What This Is

FsLit is an F# test runner inspired by LLVM lit, designed for compiler and interpreter testing. It reads `.flt` test files with embedded directives for command execution, input, expected output, exit code, stderr, and timeout — then executes and verifies them. At v0.3.0 with comprehensive test verification, verbose debugging, and test filtering.

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
- ✓ Exit code checking via `// --- ExitCode: N` directive (ignore if unspecified) — v0.3.0
- ✓ Stderr checking via `// --- Stderr:` section (contains-match) — v0.3.0
- ✓ Timeout support via `// --- Timeout: N` directive (kill after N seconds) — v0.3.0
- ✓ `--verbose` flag showing actual vs expected output on failure — v0.3.0
- ✓ `--filter <glob>` flag for targeted test execution — v0.3.0

### Active

(None — next milestone requirements TBD)

### Out of Scope

- Multiple test cases per file — adds parser complexity, not needed now
- Regex/pattern matching (`CHECK:` / `CHECK-RE:`) — significant feature, defer to future
- `CHECK-NOT` — depends on pattern matching infrastructure
- Parallel test execution — optimization, not a correctness feature
- F# idiomatic refactoring (fold-based parser, railway-oriented error handling) — code quality, not user-facing

## Context

- F# on .NET 10, built with `dotnet build`
- 6 source modules: Types, Parser, Runner, Checker, Substitution, Program (527 LOC)
- 12 test files covering all directives and CLI flags
- All directives use Option types for backward compatibility (None = don't check)
- TestResult.Fail carries actual output data for verbose display
- Glob-to-regex conversion for test filtering with case-insensitive matching

## Constraints

- **Tech stack**: F# / .NET 10 — existing project, no change
- **Compatibility**: New directives must be optional and backward-compatible with existing `.flt` files
- **Naming**: Follow existing `// --- Directive:` convention for new directives

## Key Decisions

| Decision | Rationale | Outcome |
|----------|-----------|---------|
| Exit code ignored when unspecified | Backward compatibility — existing tests don't specify exit codes | ✓ Good |
| Stderr uses contains-match | Stderr often has extra noise (warnings, debug info); exact match too brittle | ✓ Good |
| Filter uses glob patterns | More flexible than substring, familiar to CLI users | ✓ Good |
| Timeout uses Process.WaitForExit(ms) + Kill | Simple, reliable, no external dependencies | ✓ Good |
| TestResult.Fail extended with actual output | Enables verbose mode without separate data structure | ✓ Good |
| Glob-to-regex with case-insensitive matching | Intuitive for users, handles edge cases via try-catch | ✓ Good |

---
*Last updated: 2026-02-16 after v0.3.0 milestone*
