# FsLit - F# 테스트 러너

## 개요

FsLit은 LLVM lit test에서 영감을 받은 컴파일러/인터프리터용 테스트 러너입니다.
테스트 파일 하나에 입력, 명령어, 기대 출력을 모두 포함하여 관리합니다.

## 설치

### 빌드

```bash
dotnet build FsLit/FsLit.fsproj
```

### 독립 실행 파일 생성

```bash
# 일반 (~73MB)
dotnet publish FsLit/FsLit.fsproj -c Release -r linux-x64 \
  --self-contained true -p:PublishSingleFile=true -o FsLit/publish

# Trimmed (~16MB)
dotnet publish FsLit/FsLit.fsproj -c Release -r linux-x64 \
  --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true \
  -o FsLit/publish-trimmed
```

## CLI 사용법

```bash
# 도움말
fslit --help
fslit -h

# 단일 파일 실행
fslit test.flt

# 디렉토리 내 모든 테스트 실행
fslit tests/

# 실패 시 상세 출력
fslit --verbose tests/

# 패턴으로 필터링
fslit --filter 'echo*' tests/
```

### CLI 옵션

| 옵션 | 설명 |
|------|------|
| `-h, --help` | 도움말 표시 |
| `-v, --verbose` | 실패 시 actual vs expected 출력 |
| `-f, --filter <pattern>` | 글로브 패턴으로 테스트 필터링 |

### 종료 코드

| 코드 | 의미 |
|------|------|
| 0 | 모든 테스트 통과 (또는 --help) |
| 1 | 하나 이상의 테스트 실패 |
| 2 | 파일을 찾을 수 없음 |

## 테스트 파일 형식

확장자: `.flt`

```
// Test: <테스트 목적 설명>
// --- Command: <실행할 명령어>
// --- Input:
<컴파일러/인터프리터에 전달할 소스 코드>
// --- Output:
<기대하는 출력>
// --- ExitCode: N          (선택, 미지정 시 검사 안 함)
// --- Stderr:              (선택, contains-match)
<기대 stderr 라인>
// --- Timeout: N           (선택, 초 단위)
```

### 디렉티브

| 디렉티브 | 필수 | 설명 |
|----------|------|------|
| `// Test:` | X | 테스트 목적 설명 (첫 줄 주석) |
| `// --- Command:` | O | 실행할 명령어 |
| `// --- Input:` | X | 소스 코드 (임시 파일로 저장됨) |
| `// --- Output:` | X | 기대 stdout (줄 단위 정확 일치) |
| `// --- ExitCode: N` | X | 기대 종료 코드 |
| `// --- Stderr:` | X | 기대 stderr (contains-match) |
| `// --- Timeout: N` | X | 타임아웃 초 |

### 변수 치환

| 변수 | 설명 |
|------|------|
| `%input` | Input 섹션 내용이 저장된 임시 파일 경로 |
| `%output` | Output 섹션 내용이 저장된 임시 파일 경로 |
| `%s` | 테스트 파일 경로 |
| `%S` | 테스트 파일 디렉토리 |

## 예제

### 기본 예제

```
// Test: 기본 echo 명령이 기대한 stdout을 출력하는지 검증
// --- Command: echo "hello"
// --- Output:
hello
```

### Python 테스트

```
// Test: Python 스크립트를 %input으로 실행하여 출력 검증
// --- Command: python3 %input
// --- Input:
print(1 + 2)
// --- Output:
3
```

### 종료 코드 + Stderr 테스트

```
// Test: ExitCode와 Stderr 디렉티브로 에러 처리 검증
// --- Command: sh -c 'echo "error output" >&2; exit 42'
// --- ExitCode: 42
// --- Stderr:
error output
```

### 종합 테스트

```
// Test: 모든 디렉티브 (Command, Input, Output, ExitCode, Stderr, Timeout) 종합 검증
// --- Command: sh -c 'cat %input; echo "warning" >&2; exit 1'
// --- Input:
hello
// --- Output:
hello
// --- ExitCode: 1
// --- Stderr:
warning
// --- Timeout: 5
```

## 출력 비교 규칙

**stdout**: Output 섹션의 모든 줄은 순서대로 실제 출력과 비교됩니다.

- 각 줄은 정확히 일치해야 함 (CHECK-NEXT 방식)
- 빈 줄도 비교 대상
- 실제 출력이 더 길면: PASS (나머지 무시)
- 실제 출력이 더 짧으면: FAIL

**stderr**: Contains-match 방식으로 비교됩니다.

- 각 기대 라인이 실제 stderr 어딘가에 포함되면 통과
- 추가 라인 허용 (노이즈 내성)
- 순서 무관

## 동작 흐름

```
┌─────────────────┐
│  테스트 파일     │
│  (example.flt)  │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│  Parser         │
│  - Command 추출  │
│  - Input 추출   │
│  - Output 추출  │
│  - ExitCode 추출│
│  - Stderr 추출  │
│  - Timeout 추출 │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│  Substitution   │
│  - %input 생성  │
│  - %output 생성 │
│  - 변수 치환    │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│  Runner         │
│  - 명령어 실행  │
│  - stdout 캡처  │
│  - stderr 캡처  │
│  - exit code    │
│  - timeout 적용 │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│  Checker        │
│  - stdout 비교  │
│  - stderr 비교  │
│  - exit code    │
│  - 결과 보고    │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│  PASS / FAIL    │
└─────────────────┘
```

## 프로젝트 구조

```
FsLit/
├── FsLit.fsproj
├── src/
│   ├── Types.fs         # 핵심 타입 정의 (TestCase, CheckResult, TestResult)
│   ├── Checker.fs       # 출력 비교 (stdout, stderr, exit code)
│   ├── Substitution.fs  # 변수 치환, 임시 파일 관리
│   ├── Runner.fs        # 명령어 실행, timeout 적용
│   ├── Parser.fs        # 테스트 파일 파싱 (6개 디렉티브)
│   └── Program.fs       # CLI 진입점, --verbose, --filter
├── publish/             # 독립 실행 파일
└── publish-trimmed/     # Trimmed 실행 파일
```
