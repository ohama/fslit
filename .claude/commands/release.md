# Release Command

git push 전에 버전을 업그레이드하고 CHANGELOG.md를 업데이트합니다.

## 실행 단계

1. VERSION 파일에서 현재 버전을 읽습니다.
2. 사용자에게 버전 업그레이드 타입을 묻습니다 (major/minor/patch).
3. 새 버전 번호를 계산합니다.
4. 마지막 릴리스 이후의 git 커밋들을 분석합니다.
5. CHANGELOG.md에 새 버전 섹션을 추가합니다.
6. VERSION 파일을 업데이트합니다.
7. 변경사항을 git commit 합니다.

## 사용자 입력

$ARGUMENTS - 버전 타입 (major, minor, patch) 또는 비워두면 질문

## 실행

1. VERSION 파일을 읽어 현재 버전 확인
2. $ARGUMENTS가 없으면 AskUserQuestion으로 버전 타입 질문 (patch/minor/major)
3. 새 버전 계산 (예: 0.1.0 + patch = 0.1.1)
4. `git log --oneline <last-tag>..HEAD` 로 변경사항 수집 (태그 없으면 전체)
5. CHANGELOG.md 상단에 새 버전 섹션 추가:
   - Added, Changed, Fixed, Removed 카테고리로 분류
   - 커밋 메시지 기반으로 내용 작성
6. VERSION 파일 업데이트
7. git add VERSION CHANGELOG.md && git commit -m "chore: release v{새버전}"
8. 결과 출력
