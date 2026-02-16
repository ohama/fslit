# FsLit

![Version](https://img.shields.io/badge/version-0.3.0-blue.svg)
![F#](https://img.shields.io/badge/F%23-.NET%2010-purple.svg)
![License](https://img.shields.io/badge/license-MIT-green.svg)

[한국어](README.ko.md)

An F# test runner inspired by LLVM lit. Ideal for compiler/interpreter testing.

## Quick Start

```bash
# Using Makefile (recommended)
make build    # Build
make test     # Run tests

# Or use dotnet directly
dotnet build FsLit/FsLit.fsproj
dotnet run --project FsLit -- tests/
```

## CLI Options

```bash
fslit [options] <test-file-or-directory>
```

| Option | Description |
|--------|-------------|
| `-h, --help` | Show help message |
| `-v, --verbose` | Show actual vs expected output on failure |
| `-f, --filter <pattern>` | Run only tests matching glob pattern (e.g., `'echo*'`) |

## Makefile Commands

| Command | Description |
|---------|-------------|
| `make build` | Build debug version |
| `make release` | Build release version |
| `make dist` | Create standalone binary (Linux x64) |
| `make test` | Run tests |
| `make clean` | Remove build artifacts |
| `make help` | Show help |

## Test File Examples

### Example 1: Simple Command Test

`echo.flt`:
```
// --- Command: echo "hello world"
// --- Output:
hello world
```

### Example 2: Using Input File

`input.flt`:
```
// --- Command: cat %input
// --- Input:
line1
line2
line3
// --- Output:
line1
line2
line3
```

### Example 3: Exit Code and Stderr

`error.flt`:
```
// --- Command: sh -c 'echo "error output" >&2; exit 42'
// --- ExitCode: 42
// --- Stderr:
error output
```

### Example 4: Comprehensive Test

`full.flt`:
```
// --- Command: sh -c 'cat %input; echo "warning" >&2; exit 1'
// --- Input:
hello
// --- Output:
hello
// --- ExitCode: 1
// --- Stderr:
warning
// --- Timeout: 5
```

Running:
```bash
$ fslit tests/
PASS: tests/echo.flt
PASS: tests/input.flt

Results: 2/2 passed

$ fslit --verbose --filter 'echo*' tests/
PASS: tests/echo.flt

Results: 1/1 passed
```

## Test File Format

```
// --- Command: <command to execute>
// --- Input:
<source code>
// --- Output:
<expected output>
// --- ExitCode: N          (optional, default: not checked)
// --- Stderr:              (optional, contains-match)
<expected stderr lines>
// --- Timeout: N           (optional, seconds)
```

### Directives

| Directive | Required | Description |
|-----------|----------|-------------|
| `// --- Command:` | Yes | Shell command to execute |
| `// --- Input:` | No | Source code saved to temp file |
| `// --- Output:` | No | Expected stdout (line-by-line exact match) |
| `// --- ExitCode: N` | No | Expected exit code (not checked if absent) |
| `// --- Stderr:` | No | Expected stderr lines (contains-match) |
| `// --- Timeout: N` | No | Timeout in seconds (no timeout if absent) |

### Variables

| Variable | Description |
|----------|-------------|
| `%input` | Temporary file containing Input content |
| `%output` | Temporary file for Output |
| `%s` | Test file path |
| `%S` | Test file directory |

## Documentation

- [Installation Guide](guide/install.md)
- [Build Guide](guide/build.md)
- [Usage Guide](guide/usage.md)
- [Design Document](guide/design.md)
- [Tutorial](howto/README.md) - Step-by-step guide

## License

MIT
