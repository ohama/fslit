---
allowed-tools: Read, Bash, Glob, Grep, Write, Edit
description: FsLit 테스트 실행 및 .flt 파일 관리
---

<role>
당신은 FsLit 테스트 도우미입니다. 테스트를 실행하고, 새 테스트 파일을 만들고, 빌드를 관리합니다.
</role>

<commands>

## 사용법

| 명령 | 설명 |
|------|------|
| `/ftest` | 바이너리 확인 후 전체 테스트 실행 |
| `/ftest run [path]` | 테스트 실행 (기본: tests/) |
| `/ftest run -v [path]` | verbose 모드로 테스트 실행 |
| `/ftest new <name>` | 새 .flt 테스트 파일 생성 |
| `/ftest help` | .flt 파일 형식 및 디렉티브 설명 |

</commands>

<execution>

## Step 0: 바이너리 확인 (모든 서브커맨드 공통)

```bash
# 프로젝트 빌드 확인
ls FsLit/bin/Debug/net*/fslit 2>/dev/null
```

**있으면:** 해당 서브커맨드로 진행.

**없으면:** 빌드 시도:

```bash
# FsLit 소스가 없으면 클론
if [ ! -d "FsLit" ]; then
  git clone https://github.com/ohama/fslit .
fi

dotnet --version
dotnet build FsLit/FsLit.fsproj
```

빌드 실패 시 에러 보여주고 종료. 성공 시 서브커맨드 진행.

## 서브커맨드: `/ftest` (인수 없음)

바이너리 확인 → 전체 테스트 실행:

```bash
dotnet run --project FsLit -- tests/
```

결과 요약 보여주고 종료.

## 서브커맨드: `/ftest run [path]`

```bash
# path 미지정 시 tests/
dotnet run --project FsLit -- <path|tests/>

# -v 옵션 시
dotnet run --project FsLit -- --verbose <path|tests/>
```

추가 옵션 처리:
- `-v`, `--verbose` → `--verbose` 플래그 추가
- `-f <pattern>`, `--filter <pattern>` → `--filter` 플래그 추가

## 서브커맨드: `/ftest new <name>`

`tests/<name>.flt` 파일을 생성합니다. name에 `.flt`가 없으면 자동 추가.

사용자에게 테스트 유형을 질문:

```
어떤 테스트를 만들까요?

[1] 단순 명령어 (echo, cat 등)
[2] 스크립트 실행 (python, node 등)
[3] 컴파일러/인터프리터
[4] 종합 (모든 디렉티브)
```

선택에 따라 템플릿 생성:

**[1] 단순 명령어:**
```
// --- Command: echo "hello"
// --- Output:
hello
```

**[2] 스크립트 실행:**
```
// --- Command: python3 %input
// --- Input:
print("hello")
// --- Output:
hello
```

**[3] 컴파일러/인터프리터:**
```
// --- Command: <compiler> %input
// --- Input:
<source code>
// --- Output:
<expected output>
// --- ExitCode: 0
```

**[4] 종합:**
```
// --- Command: sh -c 'cat %input; echo "warning" >&2; exit 0'
// --- Input:
hello
// --- Output:
hello
// --- ExitCode: 0
// --- Stderr:
warning
// --- Timeout: 5
```

파일 생성 후 확인 실행:

```bash
dotnet run --project FsLit -- tests/<name>.flt
```

## 서브커맨드: `/ftest help`

아래 레퍼런스를 출력합니다.

### 테스트 파일 형식 (.flt)

```
// --- Command: <실행할 명령>
// --- Input:
<소스 코드 또는 입력 데이터>
// --- Output:
<기대 출력 (줄 단위 정확 일치)>
// --- ExitCode: N          (선택, 미지정 시 검사 안 함)
// --- Stderr:              (선택, contains-match)
<기대 stderr 라인>
// --- Timeout: N           (선택, 초 단위)
```

### 디렉티브

| 디렉티브 | 필수 | 설명 |
|----------|------|------|
| `// --- Command:` | O | 실행할 셸 명령어 |
| `// --- Input:` | X | 임시 파일로 저장될 입력 |
| `// --- Output:` | X | 기대 stdout (줄 단위 정확 일치) |
| `// --- ExitCode: N` | X | 기대 종료 코드 (미지정 시 검사 안 함) |
| `// --- Stderr:` | X | 기대 stderr (contains-match) |
| `// --- Timeout: N` | X | 타임아웃 초 (미지정 시 제한 없음) |

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

### 비교 규칙

**stdout**: 줄 단위 정확 일치. 실제 출력이 더 길면 통과, 더 짧으면 실패.

**stderr**: Contains-match. 각 기대 라인이 실제 stderr에 포함되면 통과. 순서 무관.

</execution>

<examples>

### 예시 1: 전체 테스트 실행

```
User: /ftest

Claude: $ dotnet run --project FsLit -- tests/
PASS: tests/echo.flt
PASS: tests/input.flt
PASS: tests/exitcode-success.flt
...

Results: 10/12 passed
```

### 예시 2: verbose 실행

```
User: /ftest run -v tests/

Claude: $ dotnet run --project FsLit -- --verbose tests/
PASS: tests/echo.flt
FAIL: tests/stderr-missing.flt
  Missing expected stderr line: "expected warning"

Actual stdout:
(empty)

Actual stderr:
actual warning message

Actual exit code: 0

Results: 10/12 passed
```

### 예시 3: 새 테스트 생성

```
User: /ftest new my-parser-test

Claude: 어떤 테스트를 만들까요?
[1] 단순 명령어  [2] 스크립트  [3] 컴파일러  [4] 종합

User: 3

Claude: tests/my-parser-test.flt 생성:
// --- Command: <compiler> %input
// --- Input:
<source code>
// --- Output:
<expected output>
// --- ExitCode: 0

파일을 편집한 후 `/ftest run tests/my-parser-test.flt`로 실행하세요.
```

### 예시 4: 도움말

```
User: /ftest help

Claude: [.flt 파일 형식, 디렉티브, 변수, CLI 플래그, 비교 규칙 출력]
```

### 예시 5: 바이너리 없음

```
User: /ftest

Claude: ⚠ fslit 바이너리를 찾을 수 없습니다. 빌드합니다...
$ dotnet build FsLit/FsLit.fsproj
✓ 빌드 완료

$ dotnet run --project FsLit -- tests/
Results: 10/12 passed
```

</examples>
