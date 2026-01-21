# FsLit

![Version](https://img.shields.io/badge/version-0.2.0-blue.svg)
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
dotnet run --project FsLit/FsLit.fsproj -- tests/
```

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
// --- Command: echo "hello world"
// --- Output:
hello world
```

### 예시 2: 입력 파일 사용

`input.flt`:
```
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

### 예시 3: Python 스크립트 테스트

`hello.flt`:
```
// --- Command: python3 %input
// --- Input:
print("Hello, World!")
// --- Output:
Hello, World!
```

### 예시 4: 컴파일러 테스트

`compile.flt`:
```
// --- Command: gcc -o %output %input && %output
// --- Input:
#include <stdio.h>
int main() {
    printf("Hello from C!\n");
    return 0;
}
// --- Output:
Hello from C!
```

실행:
```bash
$ fslit tests/
PASS: echo.flt
PASS: input.flt

Results: 2/2 passed
```

## 테스트 파일 형식

```
// --- Command: <실행할 명령어>
// --- Input:
<소스 코드>
// --- Output:
<기대 출력>
```

### 변수

| 변수 | 설명 |
|------|------|
| `%input` | Input 내용이 저장된 임시 파일 |
| `%output` | Output 내용이 저장된 임시 파일 |
| `%s` | 테스트 파일 경로 |
| `%S` | 테스트 파일 디렉토리 |

## 문서

- [설치 가이드](docs/install.md)
- [빌드 가이드](docs/build.md)
- [사용 가이드](docs/usage.md)
- [설계 문서](docs/design.md)
- [튜토리얼](docs/howto/README.md) - 처음부터 만들어보기

## 라이선스

MIT
