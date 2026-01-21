module FsLit.Parser

open System
open System.IO
open FsLit.Types

let private commandPrefix = "// --- Command:"
let private inputPrefix = "// --- Input:"
let private outputPrefix = "// --- Output:"

type private Section =
    | CommandSection
    | InputSection
    | OutputSection
    | NoSection

let private detectSection (line: string) : Section * string option =
    let trimmed = line.Trim()
    if trimmed.StartsWith(commandPrefix) then
        (CommandSection, Some (trimmed.Substring(commandPrefix.Length).Trim()))
    elif trimmed.StartsWith(inputPrefix) then
        (InputSection, Option.None)
    elif trimmed.StartsWith(outputPrefix) then
        (OutputSection, Option.None)
    else
        (NoSection, Option.None)

let parseContent (content: string) : Result<TestCase, string> =
    let lines = content.Split([| '\n' |], StringSplitOptions.None)

    let mutable command: string option = Option.None
    let mutable inputLines = ResizeArray<string>()
    let mutable outputLines = ResizeArray<string>()
    let mutable currentSection = NoSection

    for line in lines do
        let section, value = detectSection line
        match section with
        | CommandSection ->
            command <- value
            currentSection <- CommandSection
        | InputSection ->
            currentSection <- InputSection
        | OutputSection ->
            currentSection <- OutputSection
        | NoSection ->
            match currentSection with
            | InputSection -> inputLines.Add(line)
            | OutputSection -> outputLines.Add(line)
            | _ -> ()

    match command with
    | Option.None -> Result.Error "Missing '// --- Command:' section"
    | Some cmd ->
        let input =
            if inputLines.Count > 0 then
                Some (String.Join("\n", inputLines))
            else
                Option.None

        let output =
            outputLines
            |> Seq.toList
            |> List.rev
            |> List.skipWhile String.IsNullOrEmpty
            |> List.rev

        Ok {
            Command = cmd
            Input = input
            ExpectedOutput = output
        }

let parseFile (path: string) : Result<TestFile, string> =
    try
        let content = File.ReadAllText(path)
        match parseContent content with
        | Ok testCase ->
            Ok { Path = path; Cases = [ testCase ] }
        | Result.Error msg ->
            Result.Error (sprintf "%s: %s" path msg)
    with
    | ex -> Result.Error (sprintf "%s: %s" path ex.Message)
