# FsLit

LLVM lit test에서 영감을 받은 F# 테스트 러너. 컴파일러/인터프리터 테스트에 적합합니다.

## 빠른 시작

```bash
# 빌드
dotnet build FsLit/FsLit.fsproj

# 테스트 실행
dotnet run --project FsLit/FsLit.fsproj -- tests/
```

## 테스트 파일 예시

`hello.flt`:
```
// --- Command: python3 %input
// --- Input:
print("Hello, World!")
// --- Output:
Hello, World!
```

실행:
```bash
$ fslit hello.flt
PASS: hello.flt

Results: 1/1 passed
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
