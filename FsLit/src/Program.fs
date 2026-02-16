module FsLit.Program

open System
open System.IO
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
                Fail [timeoutError]
            else
                let outputErrors = check testCase.ExpectedOutput runResult.Stdout
                let stderrErrors = checkStderr testCase.ExpectedStderr runResult.Stderr
                let exitCodeErrors = checkExitCode testCase.ExpectedExitCode runResult.ExitCode
                let allErrors = outputErrors @ stderrErrors @ exitCodeErrors
                if allErrors.IsEmpty then
                    Pass
                else
                    Fail allErrors
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

let private printReport (report: TestReport) =
    match report.Result with
    | Pass ->
        printfn "PASS: %s" report.File
    | Fail errors ->
        printfn "FAIL: %s" report.File
        errors |> List.iter (fun e -> printfn "%s" (formatResult e))
    | Error msg ->
        printfn "ERROR: %s" report.File
        printfn "  %s" msg

let private findTestFiles (path: string) : string list =
    if File.Exists(path) then
        [ path ]
    elif Directory.Exists(path) then
        Directory.GetFiles(path, "*.flt", SearchOption.AllDirectories)
        |> Array.toList
    else
        []

let private printHelp () =
    printfn "FsLit - F# Lit Test Runner"
    printfn ""
    printfn "Usage: fslit [options] <test-file-or-directory>"
    printfn ""
    printfn "Options:"
    printfn "  -h, --help    Show this help message"
    printfn ""
    printfn "Arguments:"
    printfn "  <path>        Test file (.flt) or directory containing test files"
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
        let path = args.[0]
        let files = findTestFiles path

        if files.IsEmpty then
            printfn "No test files found: %s" path
            2
        else
            let reports = files |> List.map runTestFile
            reports |> List.iter printReport

            let passed = reports |> List.filter (fun r -> r.Result = Pass) |> List.length
            let total = reports.Length

            printfn ""
            printfn "Results: %d/%d passed" passed total

            if passed = total then 0 else 1
