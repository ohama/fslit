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
```

## 테스트 파일 작성

확장자: `.flt`

### 기본 구조

```
// --- Command: <실행할 명령어>
// --- Input:
<입력 내용>
// --- Output:
<기대 출력>
```

### 섹션 설명

| 섹션 | 필수 | 설명 |
|------|------|------|
| `// --- Command:` | O | 실행할 셸 명령어 |
| `// --- Input:` | X | 임시 파일로 저장될 입력 내용 |
| `// --- Output:` | X | 기대하는 출력 (줄 단위 비교) |

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
// --- Command: echo "hello world"
// --- Output:
hello world
```

### 2. 파일 입력 테스트

```
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
// --- Command: python3 %input
// --- Input:
for i in range(3):
    print(f"count: {i}")
// --- Output:
count: 0
count: 1
count: 2
```

### 4. 컴파일러 테스트

```
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

### 5. 에러 출력 테스트

stderr를 stdout으로 리다이렉트:

```
// --- Command: python3 %input 2>&1
// --- Input:
raise ValueError("test error")
// --- Output:
Traceback (most recent call last):
  File "%input", line 1, in <module>
    raise ValueError("test error")
ValueError: test error
```

### 6. 종료 코드 테스트

```
// --- Command: exit 0 && echo "success"
// --- Output:
success
```

## 출력 비교 규칙

1. **줄 단위 비교**: Output의 각 줄이 실제 출력의 해당 줄과 정확히 일치해야 함
2. **순서 보장**: CHECK-NEXT 방식으로 순서대로 비교
3. **추가 출력 허용**: 실제 출력이 더 길면 통과 (나머지 무시)
4. **부족한 출력 실패**: 실제 출력이 기대보다 짧으면 실패

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

## 팁

### 디버깅

명령어 결과를 직접 확인:

```bash
# Input 내용을 임시 파일로 만들어 테스트
echo 'print(1+1)' > /tmp/test.py
python3 /tmp/test.py
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
