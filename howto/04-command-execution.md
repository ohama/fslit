# 명령어 실행: 초보자를 위한 가이드

## 1. 개요

셸 명령어를 실행하고 출력을 캡처하는 방법을 배웁니다.

## 2. 사전 준비

- [03-file-parsing.md](03-file-parsing.md) 완료
- `System.Diagnostics.Process` 이해

## 3. 단계별 구현

### 3.1 결과 타입 정의

`src/Types.fs`에 추가:
```fsharp
type RunResult = {
    ExitCode: int
    Stdout: string
    Stderr: string
}
```

### 3.2 기본 실행 함수

`src/Runner.fs`:
```fsharp
module FsLit.Runner

open System
open System.Diagnostics

let run (command: string) : Result<RunResult, string> =
    try
        let psi = ProcessStartInfo()
        psi.FileName <- "/bin/sh"
        psi.Arguments <- sprintf "-c \"%s\"" (command.Replace("\"", "\\\""))
        psi.RedirectStandardOutput <- true
        psi.RedirectStandardError <- true
        psi.UseShellExecute <- false
        psi.CreateNoWindow <- true

        use proc = new Process()
        proc.StartInfo <- psi
        proc.Start() |> ignore

        let stdout = proc.StandardOutput.ReadToEnd()
        let stderr = proc.StandardError.ReadToEnd()
        proc.WaitForExit()

        Ok {
            ExitCode = proc.ExitCode
            Stdout = stdout.TrimEnd('\r', '\n')
            Stderr = stderr.TrimEnd('\r', '\n')
        }
    with
    | ex -> Error ex.Message
```

### 3.3 크로스 플랫폼 지원

```fsharp
let run (command: string) =
    let isWindows = OperatingSystem.IsWindows()
    let shell, shellArg =
        if isWindows then "cmd.exe", "/c"
        else "/bin/sh", "-c"

    let psi = ProcessStartInfo()
    psi.FileName <- shell
    psi.Arguments <- sprintf "%s \"%s\"" shellArg (command.Replace("\"", "\\\""))
    // ... 나머지 동일
```

### 3.4 비동기 출력 캡처

긴 출력을 처리할 때:

```fsharp
let run (command: string) =
    // ... ProcessStartInfo 설정 ...

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
```

### 3.5 변수 치환

`src/Substitution.fs`:
```fsharp
module FsLit.Substitution

open System.IO

let substitute (command: string) (testFilePath: string) (inputFile: string option) =
    command
    |> fun s -> s.Replace("%s", Path.GetFullPath(testFilePath))
    |> fun s -> s.Replace("%S", Path.GetDirectoryName(Path.GetFullPath(testFilePath)))
    |> fun s ->
        match inputFile with
        | Some f -> s.Replace("%input", f)
        | None -> s
```

## 4. 테스트

```fsharp
// 간단한 테스트
let result = run "echo hello"
match result with
| Ok r -> printfn "Output: %s" r.Stdout
| Error e -> printfn "Error: %s" e
```

```bash
dotnet run -- test.flt
```

## 5. 문제 해결

### 명령어를 찾을 수 없음

```
/bin/sh: command: not found
```

**해결**: 전체 경로 사용 (`/usr/bin/python3` 대신 `python3`)

### 타임아웃

```fsharp
proc.WaitForExit(timeout) |> ignore
if not proc.HasExited then
    proc.Kill()
    Error "Command timed out"
```

### 특수 문자 이스케이프

```fsharp
let escaped = command.Replace("\"", "\\\"").Replace("$", "\\$")
```

## 6. 다음 단계

- [출력 비교](05-output-checking.md)
- [독립 실행 파일 빌드](06-standalone-binary.md)
