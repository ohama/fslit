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
# 일반 (73MB)
dotnet publish FsLit/FsLit.fsproj -c Release -r linux-x64 \
  --self-contained true -p:PublishSingleFile=true -o FsLit/publish

# Trimmed (16MB)
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
```

### 종료 코드

| 코드 | 의미 |
|------|------|
| 0 | 모든 테스트 통과 (또는 --help) |
| 1 | 하나 이상의 테스트 실패 |
| 2 | 파일을 찾을 수 없음 |

## 테스트 파일 형식

확장자: `.flt`

```
// --- Command: <실행할 명령어>
// --- Input:
<컴파일러/인터프리터에 전달할 소스 코드>
// --- Output:
<기대하는 출력>
```

### 섹션 설명

| 섹션 | 필수 | 설명 |
|------|------|------|
| `// --- Command:` | O | 실행할 명령어 |
| `// --- Input:` | X | 소스 코드 (임시 파일로 저장됨) |
| `// --- Output:` | X | 기대 출력 (줄 단위 비교) |

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
// --- Command: echo "hello"
// --- Output:
hello
```

### Python 테스트

```
// --- Command: python3 %input
// --- Input:
print(1 + 2)
// --- Output:
3
```

### 컴파일러 테스트

```
// --- Command: mycompiler %input && ./a.out
// --- Input:
fn main() {
    print(42)
}
// --- Output:
42
```

### 에러 메시지 테스트

```
// --- Command: mycompiler %input 2>&1
// --- Input:
let x =
// --- Output:
error: unexpected end of input at line 1
```

## 출력 비교 규칙

Output 섹션의 모든 줄은 순서대로 실제 출력과 비교됩니다.

- 각 줄은 정확히 일치해야 함 (CHECK-NEXT 방식)
- 빈 줄도 비교 대상
- 실제 출력이 더 길면: PASS (나머지 무시)
- 실제 출력이 더 짧으면: FAIL

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
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│  Checker        │
│  - 줄 단위 비교 │
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
│   ├── Types.fs         # 핵심 타입 정의
│   ├── Parser.fs        # 테스트 파일 파싱
│   ├── Substitution.fs  # 변수 치환, 임시 파일 관리
│   ├── Runner.fs        # 명령어 실행
│   ├── Checker.fs       # 출력 비교
│   └── Program.fs       # CLI 진입점
├── publish/             # 독립 실행 파일
└── publish-trimmed/     # Trimmed 실행 파일
```
