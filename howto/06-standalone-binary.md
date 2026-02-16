# 독립 실행 파일 빌드: 초보자를 위한 가이드

## 1. 개요

.NET 런타임 없이 실행 가능한 단일 바이너리를 만드는 방법을 배웁니다.

## 2. 사전 준비

- [05-output-checking.md](05-output-checking.md) 완료
- 프로젝트 빌드 성공

## 3. 단계별 구현

### 3.1 기본 publish

```bash
dotnet publish -c Release
```

출력: `bin/Release/net10.0/publish/`

아직 .NET 런타임 필요.

### 3.2 Self-contained 빌드

```bash
dotnet publish -c Release -r linux-x64 --self-contained true
```

- `-r linux-x64`: 런타임 식별자 (RID)
- `--self-contained true`: 런타임 포함

### 3.3 단일 파일로 합치기

```bash
dotnet publish -c Release -r linux-x64 \
  --self-contained true \
  -p:PublishSingleFile=true
```

결과: 하나의 실행 파일 (~70MB)

### 3.4 Trimming으로 크기 줄이기

```bash
dotnet publish -c Release -r linux-x64 \
  --self-contained true \
  -p:PublishSingleFile=true \
  -p:PublishTrimmed=true
```

결과: ~16MB (사용하지 않는 코드 제거)

### 3.5 출력 디렉토리 지정

```bash
dotnet publish -c Release -r linux-x64 \
  --self-contained true \
  -p:PublishSingleFile=true \
  -p:PublishTrimmed=true \
  -o dist
```

### 3.6 플랫폼별 빌드

| 플랫폼 | RID |
|--------|-----|
| Linux x64 | `linux-x64` |
| Linux ARM64 | `linux-arm64` |
| macOS x64 | `osx-x64` |
| macOS ARM64 | `osx-arm64` |
| Windows x64 | `win-x64` |

```bash
# macOS Apple Silicon
dotnet publish -c Release -r osx-arm64 \
  --self-contained true \
  -p:PublishSingleFile=true \
  -p:PublishTrimmed=true \
  -o dist-mac

# Windows
dotnet publish -c Release -r win-x64 \
  --self-contained true \
  -p:PublishSingleFile=true \
  -p:PublishTrimmed=true \
  -o dist-win
```

### 3.7 Makefile 활용

```makefile
.PHONY: dist

PROJECT = FsLit/FsLit.fsproj

dist:
	dotnet publish $(PROJECT) -c Release -r linux-x64 \
		--self-contained true \
		-p:PublishSingleFile=true \
		-p:PublishTrimmed=true \
		-o dist
	@echo "Built: dist/FsLit"
```

사용:
```bash
make dist
```

## 4. 테스트

```bash
# 빌드
make dist

# 실행 권한 확인
ls -la dist/FsLit

# 테스트
./dist/FsLit --help
./dist/FsLit tests/
```

## 5. 문제 해결

### Trimming 경고

```
warning IL2026: Using member '...' which has 'RequiresUnreferencedCodeAttribute'
```

대부분 무시 가능. 리플렉션 사용 시 주의.

### 파일 크기가 여전히 큼

추가 옵션:
```bash
-p:PublishTrimmed=true \
-p:TrimMode=link \
-p:EnableCompressionInSingleFile=true
```

### 실행 권한

```bash
chmod +x dist/FsLit
```

### macOS 보안 경고

```bash
xattr -d com.apple.quarantine dist/FsLit
```

## 6. 다음 단계

- PATH에 추가하여 어디서든 실행
- CI/CD에서 자동 빌드
- GitHub Releases에 배포
