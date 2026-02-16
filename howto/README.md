# FsLit 튜토리얼

F#으로 테스트 러너를 만드는 단계별 가이드입니다.

## 목차

1. [F# 콘솔 프로젝트 만들기](01-project-setup.md)
2. [CLI 인수 처리](02-cli-arguments.md)
3. [파일 파싱](03-file-parsing.md)
4. [명령어 실행](04-command-execution.md)
5. [출력 비교](05-output-checking.md)
6. [독립 실행 파일 빌드](06-standalone-binary.md)

## 대상

- F# 초보자
- CLI 도구 개발에 관심 있는 개발자
- 테스트 자동화 도구를 만들고 싶은 개발자

## 사전 지식

- 기본 프로그래밍 개념
- 터미널/명령줄 사용법
- (선택) 다른 언어 경험

## 완성 결과

이 튜토리얼을 완료하면 다음과 같은 테스트 파일을 실행할 수 있는 도구를 만들게 됩니다:

```
// Test: Python 스크립트를 %input으로 실행하여 출력 검증
// --- Command: python3 %input
// --- Input:
print("Hello, World!")
// --- Output:
Hello, World!
```

v0.3.0에서는 종료 코드, stderr, 타임아웃 검증과 --verbose, --filter 플래그도 지원합니다:

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

```bash
$ fslit test.flt
PASS: test.flt

Results: 1/1 passed

$ fslit --verbose --filter 'echo*' tests/
PASS: tests/echo.flt

Results: 1/1 passed
```
