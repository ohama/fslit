# 출력 비교: 초보자를 위한 가이드

## 1. 개요

실제 출력과 기대 출력을 비교하는 방법을 배웁니다.

## 2. 사전 준비

- [04-command-execution.md](04-command-execution.md) 완료

## 3. 단계별 구현

### 3.1 결과 타입 정의

`src/Types.fs`에 추가:
```fsharp
type CheckResult =
    | Match
    | Mismatch of lineNum: int * expected: string * actual: string
    | MissingLine of lineNum: int * expected: string
```

### 3.2 줄 분리 함수

`src/Checker.fs`:
```fsharp
module FsLit.Checker

open System
open FsLit.Types

let private splitLines (text: string) : string list =
    if String.IsNullOrEmpty(text) then []
    else text.Split([| '\n' |], StringSplitOptions.None) |> Array.toList
```

### 3.3 비교 함수

```fsharp
let check (expected: string list) (actual: string) : CheckResult list =
    let actualLines = splitLines actual

    let rec loop lineNum expectedLines actualLines results =
        match expectedLines, actualLines with
        | [], [] ->
            results
        | [], _ :: _ ->
            // 실제 출력이 더 길면 OK (나머지 무시)
            results
        | exp :: restExp, [] ->
            // 기대 출력이 남았는데 실제 출력 끝
            let result = MissingLine(lineNum, exp)
            loop (lineNum + 1) restExp [] (result :: results)
        | exp :: restExp, act :: restAct ->
            if exp = act then
                loop (lineNum + 1) restExp restAct results
            else
                let result = Mismatch(lineNum, exp, act)
                loop (lineNum + 1) restExp restAct (result :: results)

    loop 1 expected actualLines []
    |> List.rev
    |> List.filter (fun r -> r <> Match)
```

### 3.4 결과 포맷팅

```fsharp
let formatResult (result: CheckResult) : string =
    match result with
    | Match -> ""
    | Mismatch(lineNum, expected, actual) ->
        sprintf "  Line %d: expected \"%s\", got \"%s\"" lineNum expected actual
    | MissingLine(lineNum, expected) ->
        sprintf "  Line %d: expected \"%s\", but no more output" lineNum expected
```

### 3.5 전체 흐름 연결

`src/Program.fs`:
```fsharp
let runTest (testCase: TestCase) =
    // 1. 명령어 실행
    match Runner.run testCase.Command with
    | Error msg -> Error msg
    | Ok result ->
        // 2. 출력 비교
        let errors = Checker.check testCase.ExpectedOutput result.Stdout
        if errors.IsEmpty then
            Ok "PASS"
        else
            // 3. 오류 출력
            errors
            |> List.map Checker.formatResult
            |> String.concat "\n"
            |> Error
```

## 4. 테스트

성공 케이스:
```
// --- Command: echo "hello"
// --- Output:
hello
```

실패 케이스:
```
// --- Command: echo "actual"
// --- Output:
expected
```

출력:
```
FAIL: test.flt
  Line 1: expected "expected", got "actual"
```

## 5. 문제 해결

### 줄바꿈 차이

Windows (`\r\n`) vs Unix (`\n`):

```fsharp
let normalizeLineEndings (text: string) =
    text.Replace("\r\n", "\n")
```

### 공백 차이

앞뒤 공백 무시:

```fsharp
if exp.Trim() = act.Trim() then
    // 매칭
```

### 빈 줄 처리

기대 출력 끝의 빈 줄 제거 (Parser에서):

```fsharp
|> List.rev
|> List.skipWhile String.IsNullOrEmpty
|> List.rev
```

## 6. 다음 단계

- [독립 실행 파일 빌드](06-standalone-binary.md)
