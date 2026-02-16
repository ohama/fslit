# FsLit

![Version](https://img.shields.io/badge/version-0.3.0-blue.svg)
![F#](https://img.shields.io/badge/F%23-.NET%2010-purple.svg)
![License](https://img.shields.io/badge/license-MIT-green.svg)

[English](README.md)

LLVM lit test에서 영감을 받은 F# 테스트 러너. 컴파일러/인터프리터 테스트에 적합합니다.

## 빠른 시작

```bash
# Makefile 사용 (권장)
make build    # 빌드
make test     # 테스트 실행

# 또는 dotnet 직접 사용
dotnet build FsLit/FsLit.fsproj
dotnet run --project FsLit -- tests/
```

## CLI 옵션

```bash
fslit [options] <test-file-or-directory>
```

| 옵션 | 설명 |
|------|------|
| `-h, --help` | 도움말 표시 |
| `-v, --verbose` | 실패 시 실제 출력 vs 기대 출력 표시 |
| `-f, --filter <pattern>` | 글로브 패턴으로 테스트 필터링 (예: `'echo*'`) |

## Makefile 명령어

| 명령어 | 설명 |
|--------|------|
| `make build` | Debug 버전 빌드 |
| `make release` | Release 버전 빌드 |
| `make dist` | 단일 실행 파일 생성 (Linux x64) |
| `make test` | 테스트 실행 |
| `make clean` | 빌드 결과물 삭제 |
| `make help` | 도움말 표시 |

## 테스트 파일 예시

### 예시 1: 간단한 명령어 테스트

`echo.flt`:
```
// Test: 기본 echo 명령이 기대한 stdout을 출력하는지 검증
// --- Command: echo "hello world"
// --- Output:
hello world
```

### 예시 2: 입력 파일 사용

`input.flt`:
```
// Test: %input 변수가 Input 섹션을 임시 파일로 전달하는지 검증
// --- Command: cat %input
// --- Input:
line1
line2
line3
// --- Output:
line1
line2
line3
```

### 예시 3: 종료 코드와 Stderr

`error.flt`:
```
// Test: ExitCode와 Stderr 디렉티브로 에러 처리 검증
// --- Command: sh -c 'echo "error output" >&2; exit 42'
// --- ExitCode: 42
// --- Stderr:
error output
```

### 예시 4: 종합 테스트

`full.flt`:
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

실행:
```bash
$ fslit tests/
PASS: tests/echo.flt
PASS: tests/input.flt

Results: 2/2 passed

$ fslit --verbose --filter 'echo*' tests/
PASS: tests/echo.flt

Results: 1/1 passed
```

## 테스트 파일 형식

```
// Test: <테스트 목적 설명>
// --- Command: <실행할 명령어>
// --- Input:
<소스 코드>
// --- Output:
<기대 출력>
// --- ExitCode: N          (선택, 미지정 시 검사 안 함)
// --- Stderr:              (선택, contains-match)
<기대 stderr 라인>
// --- Timeout: N           (선택, 초 단위)
```

### 디렉티브

| 디렉티브 | 필수 | 설명 |
|----------|------|------|
| `// Test:` | X | 테스트 목적 설명 (첫 줄 주석) |
| `// --- Command:` | O | 실행할 셸 명령어 |
| `// --- Input:` | X | 임시 파일로 저장될 소스 코드 |
| `// --- Output:` | X | 기대 stdout (줄 단위 정확 일치) |
| `// --- ExitCode: N` | X | 기대 종료 코드 (미지정 시 검사 안 함) |
| `// --- Stderr:` | X | 기대 stderr (contains-match) |
| `// --- Timeout: N` | X | 타임아웃 초 (미지정 시 제한 없음) |

### 변수

| 변수 | 설명 |
|------|------|
| `%input` | Input 내용이 저장된 임시 파일 |
| `%output` | Output 내용이 저장된 임시 파일 |
| `%s` | 테스트 파일 경로 |
| `%S` | 테스트 파일 디렉토리 |

## 문서

- [설치 가이드](guide/install.md)
- [빌드 가이드](guide/build.md)
- [사용 가이드](guide/usage.md)
- [설계 문서](guide/design.md)
- [튜토리얼](howto/README.md) - 처음부터 만들어보기

## 라이선스

MIT
