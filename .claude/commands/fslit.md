---
allowed-tools: Read, Bash, Glob, Grep, Write, Edit
description: FsLit 테스트 러너 설명 및 바이너리 설치
---

<role>
당신은 FsLit 테스트 가이드입니다. FsLit의 파일 기반 테스트 방식을 설명하고, 바이너리가 없으면 빌드를 도와줍니다.
</role>

<commands>

## 사용법

| 명령 | 설명 |
|------|------|
| `/fslit` | FsLit 소개 및 바이너리 확인 |
| `/fslit run <path>` | 테스트 실행 |

</commands>

<execution>

## Step 1: FsLit 소개

FsLit은 LLVM lit에서 영감을 받은 F# 테스트 러너입니다.

**핵심 가치:** 테스트 파일 하나가 실행할 명령, 입력, 기대 출력을 모두 선언합니다. 외부 설정 파일 불필요.

### 테스트 파일 형식 (.flt)

```
// --- Command: <실행할 명령>
// --- Input:
<소스 코드 또는 입력 데이터>
// --- Output:
<기대 출력 (정확히 일치)>
// --- ExitCode: N          (선택, 미지정 시 검사 안 함)
// --- Stderr:              (선택, contains-match)
<기대 stderr 라인>
// --- Timeout: N           (선택, 초 단위)
```

### 변수

| 변수 | 설명 |
|------|------|
| `%input` | Input 섹션이 담긴 임시 파일 경로 |
| `%output` | Output 섹션이 담긴 임시 파일 경로 |
| `%s` | 테스트 파일 경로 |
| `%S` | 테스트 파일 디렉토리 |

### CLI 플래그

| 플래그 | 설명 |
|--------|------|
| `-h, --help` | 도움말 |
| `-v, --verbose` | 실패 시 actual vs expected 출력 |
| `-f, --filter <pattern>` | 글로브 패턴으로 테스트 필터링 (예: `'echo*'`) |

### 예시: echo 테스트

```
// --- Command: echo "Hello, World!"
// --- Output:
Hello, World!
```

### 예시: Python 스크립트 테스트

```
// --- Command: python3 %input
// --- Input:
print(1 + 2)
// --- Output:
3
```

### 예시: 종합 테스트 (모든 디렉티브 사용)

```
// --- Command: sh -c 'cat %input; echo "warning" >&2; exit 42'
// --- Input:
hello
// --- Output:
hello
// --- ExitCode: 42
// --- Stderr:
warning
// --- Timeout: 5
```

## Step 2: 바이너리 확인

프로젝트 디렉토리에서 fslit 바이너리를 확인합니다.

```bash
# 1. 글로벌 명령어 확인
which fslit 2>/dev/null

# 2. 빌드 산출물 확인
ls FsLit/bin/Debug/net*/fslit 2>/dev/null
```

**바이너리가 있으면:**

```
✓ fslit 바이너리 발견: {경로}

실행 방법:
  dotnet run --project FsLit -- <test-file-or-dir>
  또는
  FsLit/bin/Debug/net10.0/fslit <test-file-or-dir>
```

테스트 실행 예시를 보여주고 종료.

**바이너리가 없으면:** Step 3으로 진행.

## Step 3: Git Clone & Build

```bash
# FsLit 소스가 없으면 클론
if [ ! -d "FsLit" ]; then
  git clone https://github.com/ohama/fslit .
fi

# .NET SDK 확인
dotnet --version

# 빌드
dotnet build FsLit/FsLit.fsproj

# 확인
dotnet run --project FsLit -- --help
```

빌드 성공 시:

```
✓ fslit 빌드 완료

실행 방법:
  dotnet run --project FsLit -- tests/
  dotnet run --project FsLit -- --verbose tests/echo.flt
  dotnet run --project FsLit -- --filter 'exitcode-*' tests/
```

## Step 4: 인수가 있으면 실행

`/fslit run <path>` 형태로 호출된 경우:

```bash
dotnet run --project FsLit -- <path>
```

결과를 보여주고 종료.

</execution>

<examples>

### 예시 1: 소개만

```
User: /fslit

Claude: [FsLit 소개 출력]
✓ fslit 바이너리 발견: FsLit/bin/Debug/net10.0/fslit

테스트 실행: dotnet run --project FsLit -- tests/
Results: 10/12 passed
```

### 예시 2: 바이너리 없음

```
User: /fslit

Claude: [FsLit 소개 출력]
⚠ fslit 바이너리를 찾을 수 없습니다.

빌드를 시작합니다...
$ git clone https://github.com/ohama/fslit .
$ dotnet build FsLit/FsLit.fsproj

✓ 빌드 완료
```

### 예시 3: 테스트 실행

```
User: /fslit run tests/echo.flt

Claude: $ dotnet run --project FsLit -- tests/echo.flt
PASS: tests/echo.flt
Results: 1/1 passed
```

</examples>
