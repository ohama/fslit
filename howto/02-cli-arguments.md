# CLI 인수 처리: 초보자를 위한 가이드

## 1. 개요

`--help` 옵션과 파일 경로 인수를 처리하는 방법을 배웁니다.

## 2. 사전 준비

- [01-project-setup.md](01-project-setup.md) 완료
- F# 기본 문법 (match, if/else)

## 3. 단계별 구현

### 3.1 기본 인수 처리

`src/Program.fs`:
```fsharp
[<EntryPoint>]
let main args =
    if args.Length = 0 then
        printfn "Usage: fslit <file>"
        0
    else
        let path = args.[0]
        printfn "File: %s" path
        0
```

### 3.2 --help 옵션 추가

```fsharp
let printHelp () =
    printfn "FsLit - Test Runner"
    printfn ""
    printfn "Usage: fslit [options] <file>"
    printfn ""
    printfn "Options:"
    printfn "  -h, --help    Show this help"

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
        printfn "Processing: %s" path
        0
```

### 3.3 종료 코드 사용

```fsharp
[<EntryPoint>]
let main args =
    if args.Length = 0 then
        printHelp ()
        0  // 성공
    elif args.[0] = "--help" || args.[0] = "-h" then
        printHelp ()
        0  // 성공
    else
        let path = args.[0]
        if System.IO.File.Exists(path) then
            printfn "Processing: %s" path
            0  // 성공
        else
            printfn "File not found: %s" path
            2  // 오류
```

### 3.4 패턴 매칭 사용

더 깔끔한 방식:

```fsharp
[<EntryPoint>]
let main args =
    match args |> Array.toList with
    | [] ->
        printHelp ()
        0
    | ["-h"] | ["--help"] ->
        printHelp ()
        0
    | [path] ->
        processFile path
    | _ ->
        printfn "Too many arguments"
        2
```

## 4. 테스트

```bash
# 도움말
dotnet run -- --help
dotnet run -- -h

# 파일 처리
dotnet run -- test.txt

# 인수 없음
dotnet run
```

## 5. 문제 해결

### 인수가 전달되지 않음

```bash
# 잘못된 방법
dotnet run --help

# 올바른 방법 (-- 사용)
dotnet run -- --help
```

`--` 뒤의 인수만 프로그램에 전달됩니다.

### 배열 인덱스 오류

```
System.IndexOutOfRangeException
```

**해결**: `args.Length` 확인 후 접근

## 6. 다음 단계

- [파일 파싱](03-file-parsing.md)
- [명령어 실행](04-command-execution.md)
