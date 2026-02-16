module FsLit.Checker

open System
open FsLit.Types

let private splitLines (text: string) : string list =
    if String.IsNullOrEmpty(text) then []
    else text.Split([| '\n' |], StringSplitOptions.None) |> Array.toList

let check (expected: string list) (actual: string) : CheckResult list =
    let actualLines = splitLines actual

    let rec loop lineNum expectedLines actualLines results =
        match expectedLines, actualLines with
        | [], [] ->
            results
        | [], _ :: _ ->
            results
        | exp :: restExp, [] ->
            let result = MissingLine(lineNum, exp)
            loop (lineNum + 1) restExp [] (result :: results)
        | exp :: restExp, act :: restAct ->
            if exp = act then
                let result = Match
                loop (lineNum + 1) restExp restAct results
            else
                let result = Mismatch(lineNum, exp, act)
                loop (lineNum + 1) restExp restAct (result :: results)

    let results = loop 1 expected actualLines []
    results |> List.rev |> List.filter (fun r -> r <> Match)

let checkExitCode (expectedExitCode: int option) (actualExitCode: int) : CheckResult list =
    match expectedExitCode with
    | None -> []
    | Some expected ->
        if expected = actualExitCode then
            []
        else
            [ExitCodeMismatch(expected, actualExitCode)]

let formatResult (result: CheckResult) : string =
    match result with
    | Match -> ""
    | Mismatch(lineNum, expected, actual) ->
        sprintf "  Line %d: expected \"%s\", got \"%s\"" lineNum expected actual
    | MissingLine(lineNum, expected) ->
        sprintf "  Line %d: expected \"%s\", but no more output" lineNum expected
    | ExtraOutput(lineNum, actual) ->
        sprintf "  Line %d: unexpected output \"%s\"" lineNum actual
    | ExitCodeMismatch(expected, actual) ->
        sprintf "  Exit code: expected %d, got %d" expected actual
