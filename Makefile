.PHONY: install build test clean restore

# Install dotnet tools
install:
	dotnet tool restore

# Restore dependencies
restore:
	dotnet restore

# Build the project
build: restore
	dotnet build

# Run tests
test: build
	dotnet test

# Clean build artifacts
clean:
	dotnet clean

# Build and test (common workflow)
all: install build test
