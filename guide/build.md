# FsLit 빌드 가이드

## 요구 사항

- .NET SDK 10.0 이상

## 빌드

### 개발 빌드

```bash
dotnet build FsLit/FsLit.fsproj
```

실행:
```bash
dotnet run --project FsLit/FsLit.fsproj -- <arguments>
```

### 릴리스 빌드

```bash
dotnet build FsLit/FsLit.fsproj -c Release
```

## 독립 실행 파일 생성

### Linux (x64)

```bash
# 일반 (~73MB)
dotnet publish FsLit/FsLit.fsproj -c Release -r linux-x64 \
  --self-contained true \
  -p:PublishSingleFile=true \
  -o FsLit/publish

# Trimmed (~16MB)
dotnet publish FsLit/FsLit.fsproj -c Release -r linux-x64 \
  --self-contained true \
  -p:PublishSingleFile=true \
  -p:PublishTrimmed=true \
  -o FsLit/publish-trimmed
```

### macOS (x64)

```bash
dotnet publish FsLit/FsLit.fsproj -c Release -r osx-x64 \
  --self-contained true \
  -p:PublishSingleFile=true \
  -p:PublishTrimmed=true \
  -o FsLit/publish-osx
```

### macOS (ARM64 / Apple Silicon)

```bash
dotnet publish FsLit/FsLit.fsproj -c Release -r osx-arm64 \
  --self-contained true \
  -p:PublishSingleFile=true \
  -p:PublishTrimmed=true \
  -o FsLit/publish-osx-arm64
```

### Windows (x64)

```bash
dotnet publish FsLit/FsLit.fsproj -c Release -r win-x64 \
  --self-contained true \
  -p:PublishSingleFile=true \
  -p:PublishTrimmed=true \
  -o FsLit/publish-win
```

## 설치

빌드된 실행 파일을 PATH에 포함된 디렉토리로 복사:

```bash
# Linux/macOS
cp FsLit/publish-trimmed/FsLit ~/.local/bin/fslit
chmod +x ~/.local/bin/fslit

# 또는 시스템 전역
sudo cp FsLit/publish-trimmed/FsLit /usr/local/bin/fslit
```

## 출력 디렉토리

| 디렉토리 | 설명 |
|----------|------|
| `bin/Debug/net10.0/` | 개발 빌드 |
| `bin/Release/net10.0/` | 릴리스 빌드 |
| `publish/` | 독립 실행 파일 (일반) |
| `publish-trimmed/` | 독립 실행 파일 (최적화) |
