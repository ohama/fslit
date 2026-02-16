# F# 콘솔 프로젝트 만들기: 초보자를 위한 가이드

## 1. 개요

F#으로 CLI 도구를 만드는 방법을 배웁니다. FsLit 프로젝트를 예시로 사용합니다.

## 2. 사전 준비

- .NET SDK 설치 (8.0 이상)
- 터미널 기본 사용법

```bash
# .NET 버전 확인
dotnet --version
```

## 3. 단계별 구현

### 3.1 프로젝트 생성

```bash
# F# 콘솔 프로젝트 생성
dotnet new console -lang F# -o FsLit

# 디렉토리 이동
cd FsLit
```

생성된 파일:
```
FsLit/
├── FsLit.fsproj    # 프로젝트 파일
├── Program.fs      # 메인 소스 파일
└── obj/            # 빌드 캐시
```

### 3.2 프로젝트 파일 구조

`FsLit.fsproj`:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <Compile Include="Program.fs" />
  </ItemGroup>
</Project>
```

### 3.3 여러 소스 파일 추가

F#은 파일 순서가 중요합니다. 의존하는 파일이 먼저 나와야 합니다.

```bash
# src 디렉토리 생성
mkdir src
```

`FsLit.fsproj` 수정:
```xml
<ItemGroup>
  <Compile Include="src/Types.fs" />
  <Compile Include="src/Parser.fs" />
  <Compile Include="src/Program.fs" />
</ItemGroup>
```

### 3.4 기본 모듈 작성

`src/Types.fs`:
```fsharp
module FsLit.Types

type Result =
    | Success
    | Failure of string
```

`src/Program.fs`:
```fsharp
module FsLit.Program

open FsLit.Types

[<EntryPoint>]
let main args =
    printfn "Hello, FsLit!"
    0
```

### 3.5 빌드 및 실행

```bash
# 빌드
dotnet build

# 실행
dotnet run

# 인수 전달
dotnet run -- arg1 arg2
```

## 4. 테스트

```bash
# 빌드 확인
dotnet build

# 실행 확인
dotnet run -- --help
```

## 5. 문제 해결

### 파일 순서 오류

```
error FS0039: The namespace or module 'FsLit.Types' is not defined
```

**해결**: `FsLit.fsproj`에서 `Types.fs`가 `Program.fs`보다 먼저 나오는지 확인

### 모듈 이름 충돌

```
error FS0248: Two modules named 'Program' occur in two parts of this assembly
```

**해결**: 각 파일에 고유한 모듈 이름 사용 (`module FsLit.Program`)

## 6. 다음 단계

- [CLI 인수 처리](02-cli-arguments.md)
- [파일 파싱](03-file-parsing.md)
