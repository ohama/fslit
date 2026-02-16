module FsLit.Program

open System
open System.IO
open System.Text.RegularExpressions
open FsLit.Types
open FsLit.Parser
open FsLit.Substitution
open FsLit.Runner
open FsLit.Checker

let private runTestCase (testFilePath: string) (testCase: TestCase) : TestResult =
    let tempFiles = createTempFiles testCase.Input testCase.ExpectedOutput

    try
        let command = substitute testCase.Command testFilePath tempFiles

        match run command testCase.Timeout with
        | Result.Error msg ->
            Error msg
        | Ok runResult ->
            if runResult.TimedOut then
                let timeoutError = TimeoutExceeded(testCase.Timeout |> Option.defaultValue 0)
                Fail ([timeoutError], runResult.Stdout, runResult.Stderr, runResult.ExitCode)
            else
                let outputErrors = check testCase.ExpectedOutput runResult.Stdout
                let stderrErrors = checkStderr testCase.ExpectedStderr runResult.Stderr
                let exitCodeErrors = checkExitCode testCase.ExpectedExitCode runResult.ExitCode
                let allErrors = outputErrors @ stderrErrors @ exitCodeErrors
                if allErrors.IsEmpty then
                    Pass
                else
                    Fail (allErrors, runResult.Stdout, runResult.Stderr, runResult.ExitCode)
    finally
        cleanupTempFiles tempFiles

let private runTestFile (path: string) : TestReport =
    match parseFile path with
    | Result.Error msg ->
        { File = path; Result = Error msg }
    | Ok testFile ->
        let results =
            testFile.Cases
            |> List.map (runTestCase path)

        let finalResult =
            results
            |> List.tryFind (function Pass -> false | _ -> true)
            |> Option.defaultValue Pass

        { File = path; Result = finalResult }

let private printReport (verbose: bool) (report: TestReport) =
    match report.Result with
    | Pass ->
        printfn "PASS: %s" report.File
    | Fail (errors, actualStdout, actualStderr, actualExitCode) ->
        printfn "FAIL: %s" report.File
        errors |> List.iter (fun e -> printfn "%s" (formatResult verbose e))

        if verbose then
            printfn ""
            printfn "Actual stdout:"
            printfn "%s" (if actualStdout = "" then "(empty)" else actualStdout)
            printfn ""
            printfn "Actual stderr:"
            printfn "%s" (if actualStderr = "" then "(empty)" else actualStderr)
            printfn ""
            printfn "Actual exit code: %d" actualExitCode
    | Error msg ->
        printfn "ERROR: %s" report.File
        printfn "  %s" msg

let private matchesPattern (fileName: string) (pattern: string) : bool =
    try
        let regexPattern =
            pattern
                .Replace(".", "\\.")
                .Replace("*", ".*")
                .Replace("?", ".")
        let regex = Regex("^" + regexPattern + "$", RegexOptions.IgnoreCase)
        regex.IsMatch(fileName)
    with
    | :? System.ArgumentException -> false

let private findTestFiles (path: string) (filterPattern: string option) : string list =
    let allFiles =
        if File.Exists(path) then
            [ path ]
        elif Directory.Exists(path) then
            Directory.GetFiles(path, "*.flt", SearchOption.AllDirectories)
            |> Array.toList
        else
            []

    match filterPattern with
    | None -> allFiles
    | Some pattern ->
        allFiles |> List.filter (fun f -> matchesPattern (Path.GetFileName(f)) pattern)

let private printHelp () =
    printfn "FsLit - F# Lit Test Runner"
    printfn ""
    printfn "Usage: fslit [options] <test-file-or-directory>"
    printfn ""
    printfn "Options:"
    printfn "  -h, --help               Show this help message"
    printfn "  -v, --verbose            Show actual vs expected output on test failure"
    printfn "  -f, --filter <pattern>   Run only tests matching glob pattern (e.g., 'echo*')"
    printfn ""
    printfn "Arguments:"
    printfn "  <path>           Test file (.flt) or directory containing test files"
    printfn ""
    printfn "Test File Format:"
    printfn "  // --- Command: <command>"
    printfn "  // --- Input:"
    printfn "  <source code>"
    printfn "  // --- Output:"
    printfn "  <expected output>"
    printfn ""
    printfn "Variables:"
    printfn "  %%input     Path to temp file containing Input section"
    printfn "  %%output    Path to temp file containing Output section"
    printfn "  %%s         Path to test file"
    printfn "  %%S         Directory of test file"
    printfn ""
    printfn "Example:"
    printfn "  // --- Command: python3 %%input"
    printfn "  // --- Input:"
    printfn "  print(1 + 2)"
    printfn "  // --- Output:"
    printfn "  3"

[<EntryPoint>]
let main args =
    if args.Length = 0 then
        printHelp ()
        0
    elif args.[0] = "--help" || args.[0] = "-h" then
        printHelp ()
        0
    else
        let verbose = args |> Array.exists (fun a -> a = "--verbose" || a = "-v")

        let filterPattern =
            args
            |> Array.tryFindIndex (fun a -> a = "--filter" || a = "-f")
            |> Option.bind (fun idx ->
                if idx + 1 < args.Length then Some args.[idx + 1]
                else None)

        let filterIdx = args |> Array.tryFindIndex (fun a -> a = "--filter" || a = "-f")
        let pathArgs =
            args
            |> Array.mapi (fun i a -> (i, a))
            |> Array.filter (fun (i, a) ->
                not (a.StartsWith("--") || a.StartsWith("-")) &&
                (match filterIdx with
                 | Some idx when i = idx + 1 -> false
                 | _ -> true))
            |> Array.map snd

        let path = if pathArgs.Length > 0 then pathArgs.[0] else ""

        if path = "" then
            printfn "No test file or directory specified"
            printHelp ()
            2
        else
            let files = findTestFiles path filterPattern

            if files.IsEmpty then
                match filterPattern with
                | Some pattern ->
                    printfn "No test files found matching filter '%s' in: %s" pattern path
                | None ->
                    printfn "No test files found: %s" path
                2
            else
                let reports = files |> List.map runTestFile
                reports |> List.iter (printReport verbose)

                let passed = reports |> List.filter (fun r -> r.Result = Pass) |> List.length
                let total = reports.Length

                printfn ""
                printfn "Results: %d/%d passed" passed total

                if passed = total then 0 else 1
