# FsLit 사용 가이드

## 기본 사용법

```bash
# 도움말
fslit --help
fslit -h

# 단일 테스트 파일 실행
fslit test.flt

# 디렉토리 내 모든 .flt 파일 실행
fslit tests/

# 실패 시 상세 출력
fslit --verbose tests/

# 특정 패턴 테스트만 실행
fslit --filter 'echo*' tests/

# 플래그 조합
fslit --verbose --filter 'exitcode-*' tests/
```

## CLI 옵션

| 옵션 | 설명 |
|------|------|
| `-h, --help` | 도움말 표시 |
| `-v, --verbose` | 실패 시 actual vs expected 출력 |
| `-f, --filter <pattern>` | 글로브 패턴으로 테스트 필터링 |

## 테스트 파일 작성

확장자: `.flt`

### 기본 구조

```
// Test: <테스트 목적 설명>
// --- Command: <실행할 명령어>
// --- Input:
<입력 내용>
// --- Output:
<기대 출력>
// --- ExitCode: N
// --- Stderr:
<기대 stderr 라인>
// --- Timeout: N
```

### 디렉티브

| 디렉티브 | 필수 | 설명 |
|----------|------|------|
| `// Test:` | X | 테스트 목적 설명 (첫 줄 주석) |
| `// --- Command:` | O | 실행할 셸 명령어 |
| `// --- Input:` | X | 임시 파일로 저장될 입력 내용 |
| `// --- Output:` | X | 기대하는 stdout (줄 단위 정확 일치) |
| `// --- ExitCode: N` | X | 기대 종료 코드 (미지정 시 검사 안 함) |
| `// --- Stderr:` | X | 기대 stderr (contains-match: 각 라인이 실제 stderr에 포함되면 통과) |
| `// --- Timeout: N` | X | 타임아웃 초 단위 (미지정 시 제한 없음) |

## 변수

| 변수 | 설명 |
|------|------|
| `%input` | Input 섹션 내용이 저장된 임시 파일 경로 |
| `%output` | Output 섹션 내용이 저장된 임시 파일 경로 |
| `%s` | 테스트 파일의 절대 경로 |
| `%S` | 테스트 파일이 있는 디렉토리 |

## 예제

### 1. 단순 명령어 테스트

```
// Test: 기본 echo 명령이 기대한 stdout을 출력하는지 검증
// --- Command: echo "hello world"
// --- Output:
hello world
```

### 2. 파일 입력 테스트

```
// Test: %input 변수가 Input 섹션을 임시 파일로 전달하는지 검증
// --- Command: cat %input
// --- Input:
line 1
line 2
line 3
// --- Output:
line 1
line 2
line 3
```

### 3. Python 스크립트 테스트

```
// Test: Python 스크립트를 %input으로 실행하여 출력 검증
// --- Command: python3 %input
// --- Input:
for i in range(3):
    print(f"count: {i}")
// --- Output:
count: 0
count: 1
count: 2
```

### 4. 종료 코드 테스트

```
// Test: ExitCode 디렉티브가 비정상 종료 코드를 올바르게 검증
// --- Command: sh -c 'exit 42'
// --- ExitCode: 42
```

### 5. Stderr 테스트

```
// Test: Stderr 디렉티브가 에러 출력을 contains-match로 검증
// --- Command: sh -c 'echo "warning: deprecated" >&2'
// --- Stderr:
warning: deprecated
```

Stderr는 contains-match 방식으로, 기대하는 각 라인이 실제 stderr에 포함되면 통과합니다.
실제 stderr에 추가 라인이 있어도 무시됩니다.

### 6. 타임아웃 테스트

```
// Test: Timeout 내 완료되는 명령이 정상 통과하는지 검증
// --- Command: sh -c 'sleep 1; echo done'
// --- Output:
done
// --- Timeout: 5
```

타임아웃 초과 시 프로세스가 종료되고 실패로 보고됩니다.

### 7. 종합 테스트 (모든 디렉티브 사용)

```
// Test: 모든 디렉티브 (Command, Input, Output, ExitCode, Stderr, Timeout) 종합 검증
// --- Command: sh -c 'cat %input; echo "error output" >&2; exit 42'
// --- Input:
Hello from integration test
// --- Output:
Hello from integration test
// --- ExitCode: 42
// --- Stderr:
error output
// --- Timeout: 5
```

### 8. 컴파일러 테스트

```
// Test: C 소스를 컴파일하고 실행하여 출력 검증
// --- Command: gcc %input -o /tmp/a.out && /tmp/a.out
// --- Input:
#include <stdio.h>
int main() {
    printf("Hello, C!\n");
    return 0;
}
// --- Output:
Hello, C!
```

## 출력 비교 규칙

### stdout (Output 섹션)

1. **줄 단위 비교**: Output의 각 줄이 실제 출력의 해당 줄과 정확히 일치해야 함
2. **순서 보장**: CHECK-NEXT 방식으로 순서대로 비교
3. **추가 출력 허용**: 실제 출력이 더 길면 통과 (나머지 무시)
4. **부족한 출력 실패**: 실제 출력이 기대보다 짧으면 실패

### stderr (Stderr 섹션)

1. **Contains-match**: 각 기대 라인이 실제 stderr 어딘가에 포함되면 통과
2. **추가 라인 허용**: 실제 stderr에 추가 라인이 있어도 무시
3. **순서 무관**: 각 기대 라인의 존재 여부만 확인

## 종료 코드

| 코드 | 의미 |
|------|------|
| 0 | 모든 테스트 통과 |
| 1 | 하나 이상의 테스트 실패 |
| 2 | 파일을 찾을 수 없음 |

## 출력 예시

### 성공

```
$ fslit tests/
PASS: tests/echo.flt
PASS: tests/input.flt

Results: 2/2 passed
```

### 실패

```
$ fslit tests/fail.flt
FAIL: tests/fail.flt
  Line 1: expected "expected", got "actual"

Results: 0/1 passed
```

### Verbose 모드

```
$ fslit --verbose tests/fail.flt
FAIL: tests/fail.flt
  Line 1: expected "expected", got "actual"

Actual stdout:
actual

Actual stderr:
(empty)

Actual exit code: 0

Results: 0/1 passed
```

### Filter 사용

```
$ fslit --filter 'echo*' tests/
PASS: tests/echo.flt

Results: 1/1 passed
```

## 팁

### 디버깅

명령어 결과를 직접 확인:

```bash
# Input 내용을 임시 파일로 만들어 테스트
echo 'print(1+1)' > /tmp/test.py
python3 /tmp/test.py
```

`--verbose` 플래그로 실패 시 actual 출력 확인:

```bash
fslit --verbose tests/failing-test.flt
```

### 테스트 구성

```
tests/
├── basic/
│   ├── echo.flt
│   └── cat.flt
├── python/
│   ├── hello.flt
│   └── math.flt
└── compiler/
    ├── valid.flt
    └── error.flt
```

디렉토리 단위로 실행:

```bash
fslit tests/python/
```

패턴으로 필터링:

```bash
fslit --filter 'hello*' tests/
```
