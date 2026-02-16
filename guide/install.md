# FsLit 설치 가이드

## 방법 1: 소스에서 빌드

### 요구 사항

- .NET SDK 10.0 이상

### 설치

```bash
# 저장소 클론
git clone https://github.com/ohama/fslit
cd fslit

# 빌드
dotnet build FsLit/FsLit.fsproj -c Release

# 실행 테스트
dotnet run --project FsLit -- --help
```

## 방법 2: 독립 실행 파일

### 다운로드

릴리스 페이지에서 플랫폼에 맞는 실행 파일 다운로드:

| 플랫폼 | 파일명 |
|--------|--------|
| Linux (x64) | `fslit-linux-x64` |
| macOS (x64) | `fslit-osx-x64` |
| macOS (ARM64) | `fslit-osx-arm64` |
| Windows (x64) | `fslit-win-x64.exe` |

### 직접 빌드

```bash
# Linux
dotnet publish FsLit/FsLit.fsproj -c Release -r linux-x64 \
  --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true \
  -o dist

# macOS (Intel)
dotnet publish FsLit/FsLit.fsproj -c Release -r osx-x64 \
  --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true \
  -o dist

# macOS (Apple Silicon)
dotnet publish FsLit/FsLit.fsproj -c Release -r osx-arm64 \
  --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true \
  -o dist

# Windows
dotnet publish FsLit/FsLit.fsproj -c Release -r win-x64 \
  --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true \
  -o dist
```

## PATH에 추가

### Linux / macOS

```bash
# 사용자 디렉토리에 설치
mkdir -p ~/.local/bin
cp dist/FsLit ~/.local/bin/fslit
chmod +x ~/.local/bin/fslit

# PATH에 추가 (~/.bashrc 또는 ~/.zshrc)
export PATH="$HOME/.local/bin:$PATH"
```

또는 시스템 전역 설치:

```bash
sudo cp dist/FsLit /usr/local/bin/fslit
sudo chmod +x /usr/local/bin/fslit
```

### Windows

1. 실행 파일을 원하는 위치에 복사 (예: `C:\Tools\fslit.exe`)
2. 시스템 환경 변수에서 PATH에 해당 디렉토리 추가

또는 PowerShell:

```powershell
# 사용자 디렉토리에 설치
$installDir = "$env:USERPROFILE\.local\bin"
New-Item -ItemType Directory -Force -Path $installDir
Copy-Item dist\FsLit.exe "$installDir\fslit.exe"

# PATH에 추가 (현재 세션)
$env:PATH += ";$installDir"

# PATH에 영구 추가
[Environment]::SetEnvironmentVariable("PATH", $env:PATH + ";$installDir", "User")
```

## 설치 확인

```bash
fslit --help
```

출력:
```
FsLit - F# Lit Test Runner

Usage: fslit [options] <test-file-or-directory>

Options:
  -h, --help               Show this help message
  -v, --verbose            Show actual vs expected output on test failure
  -f, --filter <pattern>   Run only tests matching glob pattern (e.g., 'echo*')

Arguments:
  <path>           Test file (.flt) or directory containing test files

Test File Format:
  // Test: <test objective>   (optional, first line comment)
  // --- Command: <command>
  // --- Input:
  <source code>
  // --- Output:
  <expected output>
  // --- ExitCode: N          (optional, default: not checked)
  // --- Stderr:              (optional, contains-match)
  <expected stderr lines>
  // --- Timeout: N          (optional, seconds)
...
```

## 제거

### Linux / macOS

```bash
rm ~/.local/bin/fslit
# 또는
sudo rm /usr/local/bin/fslit
```

### Windows

```powershell
Remove-Item "$env:USERPROFILE\.local\bin\fslit.exe"
```
