.PHONY: build release dist clean test help

PROJECT = FsLit/FsLit.fsproj
DIST_DIR = dist
BIN_NAME = fslit

help:
	@echo "Usage: make [target]"
	@echo ""
	@echo "Targets:"
	@echo "  build    Build debug version"
	@echo "  release  Build release version"
	@echo "  dist     Build standalone Linux binary"
	@echo "  test     Run tests"
	@echo "  clean    Remove build artifacts"
	@echo "  help     Show this help"

build:
	dotnet build $(PROJECT)

release:
	dotnet build $(PROJECT) -c Release

dist:
	dotnet publish $(PROJECT) -c Release -r linux-x64 \
		--self-contained true \
		-p:PublishSingleFile=true \
		-p:PublishTrimmed=true \
		-o $(DIST_DIR)
	mv $(DIST_DIR)/FsLit $(DIST_DIR)/$(BIN_NAME)
	@echo ""
	@echo "Built: $(DIST_DIR)/$(BIN_NAME)"
	@ls -lh $(DIST_DIR)/$(BIN_NAME)

test:
	dotnet run --project $(PROJECT) -- tests/

clean:
	rm -rf FsLit/bin FsLit/obj $(DIST_DIR)
	@echo "Cleaned build artifacts"
