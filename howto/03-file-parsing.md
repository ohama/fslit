# 파일 파싱: 초보자를 위한 가이드

## 1. 개요

테스트 파일을 읽고 섹션별로 파싱하는 방법을 배웁니다.

## 2. 사전 준비

- [02-cli-arguments.md](02-cli-arguments.md) 완료
- 파일 형식 이해:

```
// Test: <test objective description>
// --- Command: echo "hello"
// --- Input:
source code here
// --- Output:
expected output
```

## 3. 단계별 구현

### 3.1 타입 정의

`src/Types.fs`:
```fsharp
module FsLit.Types

type TestCase = {
    Command: string
    Input: string option
    ExpectedOutput: string list
}
```

### 3.2 섹션 구분

```fsharp
type Section =
    | CommandSection
    | InputSection
    | OutputSection
    | NoSection
```

### 3.3 섹션 감지 함수

`src/Parser.fs`:
```fsharp
module FsLit.Parser

open System
open FsLit.Types

let private commandPrefix = "// --- Command:"
let private inputPrefix = "// --- Input:"
let private outputPrefix = "// --- Output:"

let private detectSection (line: string) =
    let trimmed = line.Trim()
    if trimmed.StartsWith(commandPrefix) then
        (CommandSection, Some (trimmed.Substring(commandPrefix.Length).Trim()))
    elif trimmed.StartsWith(inputPrefix) then
        (InputSection, None)
    elif trimmed.StartsWith(outputPrefix) then
        (OutputSection, None)
    else
        (NoSection, None)
```

### 3.4 파싱 함수

```fsharp
let parseContent (content: string) : Result<TestCase, string> =
    let lines = content.Split([| '\n' |], StringSplitOptions.None)

    let mutable command: string option = None
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
    | None -> Error "Missing '// --- Command:' section"
    | Some cmd ->
        Ok {
            Command = cmd
            Input = if inputLines.Count > 0 then Some (String.Join("\n", inputLines)) else None
            ExpectedOutput = outputLines |> Seq.toList
        }
```

### 3.5 파일 읽기

```fsharp
let parseFile (path: string) =
    try
        let content = System.IO.File.ReadAllText(path)
        parseContent content
    with
    | ex -> Error ex.Message
```

## 4. 테스트

테스트 파일 (`test.flt`):
```
// Test: echo 명령이 기대한 출력을 생성하는지 검증
// --- Command: echo "hello"
// --- Output:
hello
```

```bash
dotnet run -- test.flt
```

## 5. 문제 해결

### 빈 줄 처리

Output 섹션 끝의 빈 줄 제거:

```fsharp
let output =
    outputLines
    |> Seq.toList
    |> List.rev
    |> List.skipWhile String.IsNullOrEmpty
    |> List.rev
```

### 인코딩 문제

```fsharp
let content = System.IO.File.ReadAllText(path, System.Text.Encoding.UTF8)
```

## 6. 다음 단계

- [명령어 실행](04-command-execution.md)
- [출력 비교](05-output-checking.md)
