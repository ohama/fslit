# FsLit Improvement Ideas

## A. More Features (functionality)

1. **Multiple test cases per file** — Currently hardcoded to 1 case per `.flt` file. Supporting `// --- END` or repeated `// --- Command:` blocks would allow grouping related tests.

2. **Exit code checking** — `// --- ExitCode: 1` to verify the command's exit code, useful for error-case tests.

3. **Regex/pattern matching in output** — `// --- CHECK:` for substring match or `// --- CHECK-RE:` for regex, like LLVM lit's `CHECK` directives. Currently only exact line match is supported.

4. **`CHECK-NOT`** — Verify a line does *not* appear in output.

5. **Stderr checking** — `// --- Stderr:` section, instead of relying on `2>&1` redirection.

6. **Timeout support** — `// --- Timeout: 5` to kill long-running commands.

7. **Parallel test execution** — Use `Async.Parallel` for running multiple `.flt` files concurrently.

8. **`--verbose` / `--filter` flags** — Show actual output on failure, or run only tests matching a pattern.

## B. More Functional Programming Style (F# idiomatic)

### Parser refactor

The biggest win is the **Parser**. It currently uses 4 mutable variables + `ResizeArray`:

```fsharp
// current (imperative)
let mutable command = None
let mutable inputLines = ResizeArray<string>()
...
for line in lines do ...
```

Could become a clean `List.fold`:

```fsharp
// functional alternative
type ParseState = { Section: Section; Command: string option; Input: string list; Output: string list }

let parse lines =
    lines
    |> List.fold (fun state line ->
        match detectSection line with
        | CommandSection, value -> { state with Section = CommandSection; Command = value }
        | InputSection, _      -> { state with Section = InputSection }
        | OutputSection, _     -> { state with Section = OutputSection }
        | NoSection, _ ->
            match state.Section with
            | InputSection  -> { state with Input = line :: state.Input }
            | OutputSection -> { state with Output = line :: state.Output }
            | _ -> state
    ) { Section = NoSection; Command = None; Input = []; Output = [] }
```

### Checker refactor

Use `List.zip` or `Seq.map2` instead of manual recursion.

### Railway-oriented error handling

Chain `Result.bind` / `Result.map` through the pipeline instead of nested `match`.

### Computation expressions

A `result { }` CE to flatten the error handling in `Program.fs`.
