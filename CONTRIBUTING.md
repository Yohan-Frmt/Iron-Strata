# Contributing to Iron Strata

Thanks for your interest in contributing to Iron Strata!

## How to set up the project locally

1. Clone the repository: `git clone https://github.com/Yohan-Frmt/iron-strata.git`
2. Open the project in Godot Engine (v4.3 or later).
3. Ensure you have the .NET SDK installed (v8.0 or later).
4. Restore NuGet packages: `dotnet restore`

## Branch naming conventions

- `feature/...` for new features
- `fix/...` for bug fixes
- `docs/...` for documentation changes
- `refactor/...` for code refactoring

## Commit message style

We follow [Conventional Commits](https://www.conventionalcommits.org/):
- `feat: ...`
- `fix: ...`
- `docs: ...`
- `style: ...`
- `refactor: ...`
- `test: ...`
- `chore: ...`

## How to submit issues and PRs

1. Check if an issue/PR already exists.
2. If not, create a new issue using the appropriate template.
3. Fork the repository and create your branch from `main`.
4. Make your changes and ensure tests pass.
5. Submit a PR with a clear description and link the relevant issue.

## Coding style and linting

- Follow [C# Coding Conventions](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions).
- Use 4 spaces for indentation (standard Godot/C# convention).
- Keep methods concise and use meaningful names.

## Testing requirements

- Add unit tests for new logic if possible.
- Ensure the project builds without errors.
- Verify changes in the Godot editor.
