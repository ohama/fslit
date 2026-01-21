module FsLit.Runner

open System
open System.Diagnostics

type RunResult = {
    ExitCode: int
    Stdout: string
    Stderr: string
}

let run (command: string) : Result<RunResult, string> =
    try
        let isWindows = OperatingSystem.IsWindows()
        let shell, shellArg =
            if isWindows then "cmd.exe", "/c"
            else "/bin/sh", "-c"

        let psi = ProcessStartInfo()
        psi.FileName <- shell
        psi.Arguments <- sprintf "%s \"%s\"" shellArg (command.Replace("\"", "\\\""))
        psi.RedirectStandardOutput <- true
        psi.RedirectStandardError <- true
        psi.UseShellExecute <- false
        psi.CreateNoWindow <- true

        use proc = new Process()
        proc.StartInfo <- psi

        let stdout = System.Text.StringBuilder()
        let stderr = System.Text.StringBuilder()

        proc.OutputDataReceived.Add(fun e ->
            if not (isNull e.Data) then
                stdout.AppendLine(e.Data) |> ignore)
        proc.ErrorDataReceived.Add(fun e ->
            if not (isNull e.Data) then
                stderr.AppendLine(e.Data) |> ignore)

        proc.Start() |> ignore
        proc.BeginOutputReadLine()
        proc.BeginErrorReadLine()
        proc.WaitForExit()

        Ok {
            ExitCode = proc.ExitCode
            Stdout = stdout.ToString().TrimEnd('\r', '\n')
            Stderr = stderr.ToString().TrimEnd('\r', '\n')
        }
    with
    | ex -> Result.Error (sprintf "Failed to run command: %s" ex.Message)
