module FsLit.Substitution

open System
open System.IO

type TempFiles = {
    InputFile: string option
    OutputFile: string option
}

let createTempFile (content: string option) (suffix: string) : string option =
    match content with
    | None -> None
    | Some text ->
        let tempPath = Path.Combine(Path.GetTempPath(), sprintf "fslit_%s_%s" (Guid.NewGuid().ToString("N").[..7]) suffix)
        File.WriteAllText(tempPath, text)
        Some tempPath

let createTempFiles (input: string option) (expectedOutput: string list) : TempFiles =
    let inputFile = createTempFile input "input"
    let outputContent =
        if expectedOutput.IsEmpty then None
        else Some (String.Join("\n", expectedOutput))
    let outputFile = createTempFile outputContent "output"
    { InputFile = inputFile; OutputFile = outputFile }

let cleanupTempFiles (files: TempFiles) =
    files.InputFile |> Option.iter (fun f -> if File.Exists(f) then File.Delete(f))
    files.OutputFile |> Option.iter (fun f -> if File.Exists(f) then File.Delete(f))

let substitute (command: string) (testFilePath: string) (tempFiles: TempFiles) : string =
    let testFileDir = Path.GetDirectoryName(Path.GetFullPath(testFilePath))
    let testFileFullPath = Path.GetFullPath(testFilePath)

    command
    |> fun s ->
        match tempFiles.InputFile with
        | Some f -> s.Replace("%input", f)
        | None -> s
    |> fun s ->
        match tempFiles.OutputFile with
        | Some f -> s.Replace("%output", f)
        | None -> s
    |> fun s -> s.Replace("%s", testFileFullPath)
    |> fun s -> s.Replace("%S", testFileDir)
