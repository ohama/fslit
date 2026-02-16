---
allowed-tools: Read, Bash, Glob, Grep, Write, Edit
description: 앱의 미테스트 기능을 찾아 .flt 테스트를 자동 생성
---

<role>
당신은 테스트 커버리지 분석가이자 테스트 생성기입니다.
프로젝트 소스를 분석하여 테스트되지 않은 기능과 함수를 찾고,
가능한 많은 미테스트 기능에 대해 .flt 테스트 케이스를 자동 생성합니다.
</role>

<commands>

## 사용법

| 명령 | 설명 |
|------|------|
| `/ftest` | 분석 → 미테스트 기능 찾기 → 테스트 생성 → 실행 |
| `/ftest scan` | 미테스트 기능 분석 및 리포트만 (생성 안 함) |
| `/ftest run [path]` | 기존 테스트 실행 |
| `/ftest help` | .flt 파일 형식 레퍼런스 |

</commands>

<execution>

## Step 0: 프로젝트 감지

프로젝트의 테스트 가능한 인터페이스를 파악합니다.

```
1. 빌드/실행 방법 감지:
   - Makefile → make 명령
   - package.json → npm/node 명령
   - *.fsproj → dotnet run 명령
   - *.csproj → dotnet run 명령
   - main.py / setup.py → python 명령
   - go.mod → go run 명령
   - Cargo.toml → cargo run 명령

2. 앱의 실행 명령 결정:
   - CLI 앱이면: 실행 명령 + 플래그 조합
   - 라이브러리면: 래퍼 스크립트로 함수 호출
   - 웹 앱이면: curl 명령으로 엔드포인트 테스트

3. 기존 tests/ 디렉토리 확인:
   - .flt 파일 존재 여부
   - 테스트 디렉토리 구조
```

## Step 1: 소스 코드 분석 (Feature Map)

프로젝트 소스를 읽고 테스트 가능한 기능을 나열합니다.

```
분석 대상:
├── CLI 플래그/옵션 (--help, --verbose 등)
├── 서브커맨드 (run, build, test 등)
├── 입력 처리 (파일 입력, stdin, 인수)
├── 출력 형식 (stdout, stderr, exit code)
├── 에러 처리 (잘못된 입력, 파일 없음 등)
├── 엣지 케이스 (빈 입력, 경계값, 특수 문자)
└── 기능 조합 (플래그 조합, 파이프라인)
```

**각 기능에 대해 기록:**
- 기능 이름
- 어떤 소스 파일에 구현되어 있는지
- 어떻게 .flt로 테스트 가능한지 (Command, Input, Output 예상)

## Step 2: 기존 테스트 분석 (Coverage Map)

기존 .flt 파일들을 읽어서 무엇이 테스트되고 있는지 파악합니다.

```bash
# .flt 파일 찾기
find . -name "*.flt" -type f 2>/dev/null
```

**각 .flt 파일에서 추출:**
- `// --- Command:` → 어떤 기능을 테스트하는지
- `// --- ExitCode:` → 에러 케이스를 테스트하는지
- `// --- Stderr:` → stderr 출력을 테스트하는지
- 파일명 → 테스트 의도 추론

## Step 3: 갭 분석 (Gap Analysis)

Feature Map과 Coverage Map을 비교하여 미테스트 기능을 식별합니다.

**출력 형식:**

```markdown
## 테스트 커버리지 분석

### 테스트됨 (N개)
- ✓ [기능명] — [테스트 파일]
- ✓ [기능명] — [테스트 파일]

### 미테스트 (M개)
- ✗ [기능명] — [소스 위치] — [테스트 가능 방법]
- ✗ [기능명] — [소스 위치] — [테스트 가능 방법]

### 커버리지: N / (N+M) = XX%
```

**`/ftest scan` 이면 여기서 종료.**

## Step 4: 테스트 생성

미테스트 기능 각각에 대해 .flt 파일을 생성합니다.

**생성 규칙:**

1. **파일명**: `tests/{feature-name}.flt` (kebab-case)
2. **Command**: 실제 실행 가능한 명령 (Step 0에서 감지한 실행 방법 사용)
3. **Input**: 기능을 트리거하는 최소한의 입력
4. **Output**: 실제로 실행하여 확인한 정확한 기대 출력
5. **ExitCode**: 에러 케이스면 0이 아닌 코드
6. **Stderr**: 에러 메시지가 예상되면 추가
7. **Timeout**: 장시간 실행 가능하면 추가

**생성 전 확인:**

각 테스트를 생성하기 전에 실제로 명령을 실행하여 출력을 확인합니다.
예측이 아닌 실제 결과를 기반으로 테스트를 작성합니다.

```bash
# 1. 명령 실행하여 실제 출력 캡처
actual_output=$(command 2>stderr.tmp; echo "EXIT:$?")

# 2. 실제 출력을 기반으로 .flt 파일 생성
# 3. 생성된 .flt로 테스트 실행하여 PASS 확인
```

**생성하지 않는 경우:**
- 이미 테스트 파일이 존재하는 기능
- 실행 불가능한 기능 (외부 의존성, 네트워크 필요 등)
- 비결정적 출력 (타임스탬프, 랜덤 등)

## Step 5: 테스트 실행

모든 테스트 (기존 + 새로 생성된)를 실행합니다.

```bash
# fslit이 있으면
dotnet run --project FsLit -- tests/

# 없으면 fslit 빌드 또는 설치 안내
```

**결과 요약:**

```markdown
## 테스트 결과

### 새로 생성된 테스트
- PASS: tests/new-test-1.flt
- PASS: tests/new-test-2.flt
- FAIL: tests/new-test-3.flt  ← 수정 필요

### 전체 결과
Results: X/Y passed

### 커버리지 변화
이전: N/(N+M) = XX%
이후: (N+K)/(N+M) = YY%  (+K 테스트 추가)
```

## 서브커맨드: `/ftest run [path]`

기존 테스트를 실행합니다.

```bash
# path 미지정 시 tests/
dotnet run --project FsLit -- <path|tests/>

# -v 옵션: dotnet run --project FsLit -- --verbose <path>
# -f 옵션: dotnet run --project FsLit -- --filter <pattern> <path>
```

## 서브커맨드: `/ftest help`

.flt 파일 형식 레퍼런스를 출력합니다.

### 테스트 파일 형식 (.flt)

```
// --- Command: <실행할 명령>
// --- Input:
<입력 데이터>
// --- Output:
<기대 출력 (줄 단위 정확 일치)>
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

</execution>

<examples>

### 예시 1: 전체 실행 (`/ftest`)

```
User: /ftest

Claude:
## 프로젝트 감지

F# CLI 앱 (FsLit/FsLit.fsproj)
실행: dotnet run --project FsLit --
테스트 디렉토리: tests/ (12 .flt files)

## 소스 분석

6개 모듈에서 23개 기능 식별:
- Program.fs: CLI 파싱, 플래그 처리, 파일 탐색
- Parser.fs: 디렉티브 파싱, 멀티케이스
- Runner.fs: 프로세스 실행, 타임아웃
- Checker.fs: 출력 비교, stderr 비교, exit code
- Substitution.fs: 변수 치환
- Types.fs: 타입 정의

## 테스트 커버리지 분석

### 테스트됨 (12개)
- ✓ echo 출력 — echo.flt
- ✓ 파일 입력 — input.flt
- ✓ exit code 성공 — exitcode-success.flt
- ✓ exit code 실패 — exitcode-fail.flt
- ✓ exit code 미지정 — exitcode-absent.flt
- ✓ stderr 매칭 — stderr-match.flt
- ✓ stderr 미포함 — stderr-missing.flt
- ✓ stderr 미지정 — stderr-absent.flt
- ✓ 타임아웃 이내 — timeout-under.flt
- ✓ 타임아웃 초과 — timeout-exceeded.flt
- ✓ 타임아웃 미지정 — timeout-absent.flt
- ✓ 종합 테스트 — integration-all.flt

### 미테스트 (8개)
- ✗ --help 출력 — Program.fs:printHelp
- ✗ 인수 없이 실행 — Program.fs:main (args.Length=0)
- ✗ 존재하지 않는 파일 — Program.fs:findTestFiles
- ✗ 빈 디렉토리 — Program.fs:findTestFiles
- ✗ --filter 매칭 없음 — Program.fs:matchesPattern
- ✗ 잘못된 .flt (Command 없음) — Parser.fs:parseFile
- ✗ %s, %S 변수 치환 — Substitution.fs
- ✗ 빈 Output 섹션 — Checker.fs:check

### 커버리지: 12/20 = 60%

## 테스트 생성

8개 .flt 파일 생성 중...

✓ tests/help-output.flt — --help 플래그 테스트
✓ tests/no-args.flt — 인수 없이 실행 시 help + exit 0
✓ tests/missing-file.flt — 존재하지 않는 파일 경로
✓ tests/empty-dir.flt — .flt 없는 디렉토리
✓ tests/filter-no-match.flt — --filter 매칭 없음
✓ tests/invalid-flt.flt — Command 없는 .flt 파일
✓ tests/var-s-substitution.flt — %s 변수 치환
✓ tests/empty-output.flt — 빈 Output 섹션

## 테스트 실행

$ dotnet run --project FsLit -- tests/
PASS: tests/echo.flt
PASS: tests/help-output.flt
PASS: tests/no-args.flt
...

Results: 18/20 passed

## 커버리지 변화
이전: 12/20 = 60%
이후: 18/20 = 90%  (+6 테스트 추가, 2개 수정 필요)
```

### 예시 2: 스캔만 (`/ftest scan`)

```
User: /ftest scan

Claude:
## 테스트 커버리지 분석

### 테스트됨 (12개)
- ✓ echo 출력 — echo.flt
...

### 미테스트 (8개)
- ✗ --help 출력 — Program.fs:printHelp
- ✗ 인수 없이 실행 — Program.fs:main
...

### 커버리지: 12/20 = 60%

테스트를 생성하려면 `/ftest`를 실행하세요.
```

### 예시 3: Node.js 프로젝트

```
User: /ftest

Claude:
## 프로젝트 감지

Node.js CLI 앱 (package.json → bin: "mycli")
실행: node src/index.js
테스트 디렉토리: tests/ (0 .flt files)

## 소스 분석

src/index.js에서 8개 기능 식별:
- --version 플래그
- --output <file> 옵션
- JSON 입력 파싱
- CSV 출력 포맷
...

### 미테스트 (8개)
...

## 테스트 생성

8개 .flt 파일 생성 중...
✓ tests/version.flt
✓ tests/json-input.flt
✓ tests/csv-output.flt
...

Results: 7/8 passed
```

### 예시 4: 테스트 실행

```
User: /ftest run -v tests/

Claude: $ dotnet run --project FsLit -- --verbose tests/
PASS: tests/echo.flt
FAIL: tests/help-output.flt
  Line 3: expected "Options:", got "Usage:"
...

Results: 17/20 passed
```

</examples>

<critical_rules>

1. **실제 실행 기반**: 테스트 생성 시 반드시 실제 명령을 실행하여 출력을 확인한 후 .flt 파일 작성
2. **비결정적 출력 회피**: 타임스탬프, PID, 메모리 주소 등 매번 달라지는 출력은 테스트하지 않음
3. **최소 입력 원칙**: 각 테스트는 하나의 기능만 검증하는 최소한의 입력 사용
4. **기존 테스트 유지**: 기존 .flt 파일은 절대 수정하지 않음, 새 파일만 추가
5. **PASS 확인**: 생성한 테스트가 PASS하는지 확인 후 최종 결과 보고
6. **실패 테스트 처리**: 생성한 테스트가 FAIL하면 원인 분석 후 수정 시도, 수정 불가하면 삭제

</critical_rules>
