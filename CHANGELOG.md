# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.3.0] - 2026-02-16

### Added
- `// --- ExitCode: N` directive for exit code verification
- `// --- Stderr:` directive with contains-match semantics
- `// --- Timeout: N` directive for per-test timeout enforcement
- `--verbose` (`-v`) flag to show actual vs expected output on failure
- `--filter` (`-f`) flag to run tests matching glob pattern
- Comprehensive integration test using all directives
- `/ftest` slash command with subcommands (run, new, help)
- GSD workflow commands
- Tutorial (howto/) with v0.3.0 examples

### Changed
- TestResult.Fail extended to carry actual stdout, stderr, and exit code
- Documentation reorganized: `docs/` split into `guide/` and `howto/`
- READMEs updated with full directive reference and CLI options
- Help text updated to document all directives and flags

## [0.2.0] - 2025-01-21

### Added
- `/release` command for version management and changelog updates
- VERSION file for version tracking
- CHANGELOG.md for release notes

## [0.1.0] - 2025-01-21

### Added
- Initial release
- Core test runner implementation
- Test file format with `// --- Command:`, `// --- Input:`, `// --- Output:` sections
- Variable substitution: `%input`, `%output`, `%s`, `%S`
- CLI with `--help` option
- Documentation (README, build guide, usage guide, design doc)
