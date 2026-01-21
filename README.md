# FsLit

![Version](https://img.shields.io/badge/version-0.2.0-blue.svg)
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
dotnet run --project FsLit/FsLit.fsproj -- tests/
```

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

### Example 3: Python Script Test

`hello.flt`:
```
// --- Command: python3 %input
// --- Input:
print("Hello, World!")
// --- Output:
Hello, World!
```

### Example 4: Compiler Test

`compile.flt`:
```
// --- Command: gcc -o %output %input && %output
// --- Input:
#include <stdio.h>
int main() {
    printf("Hello from C!\n");
    return 0;
}
// --- Output:
Hello from C!
```

Running:
```bash
$ fslit tests/
PASS: echo.flt
PASS: input.flt

Results: 2/2 passed
```

## Test File Format

```
// --- Command: <command to execute>
// --- Input:
<source code>
// --- Output:
<expected output>
```

### Variables

| Variable | Description |
|----------|-------------|
| `%input` | Temporary file containing Input content |
| `%output` | Temporary file for Output |
| `%s` | Test file path |
| `%S` | Test file directory |

## Documentation

- [Installation Guide](docs/install.md)
- [Build Guide](docs/build.md)
- [Usage Guide](docs/usage.md)
- [Design Document](docs/design.md)
- [Tutorial](docs/howto/README.md) - Step-by-step guide

## License

MIT
