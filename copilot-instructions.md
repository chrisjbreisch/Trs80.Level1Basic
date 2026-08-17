# Repository Instructions

## Project

- This repository is a C#/.NET 10 TRS-80 BASIC interpreter.
- Use the SDK pinned by `global.json`.
- The application and Windows-targeted test projects require Windows.
- Preserve the existing architecture and prefer the nearest owning abstraction for behavior changes.

## Build And Test

- From the repository root, restore with `dotnet restore .\Trs80.Level1Basic.sln` when dependencies are needed.
- Build with `dotnet build .\Trs80.Level1Basic.sln --no-restore`.
- Run the narrowest relevant test first, then the affected test class or project.
- Run the full interpreter project when shared parser, environment, interpreter, host, or runtime behavior changes.
- Report unrelated pre-existing test failures separately; do not fix unrelated defects as part of a focused change.

## Level II Slices

- Use `LEVEL-II-FEATURE-MATRIX.md` as the implementation map and update it for every completed Level II slice.
- Each slice should have one clearly named behavior, a focused regression or compatibility test, and the smallest implementation change needed.
- Keep hardware approximations and intentional compatibility deviations explicit in the matrix.
- After each completed step: validate it, update the matrix, commit the step separately, and push the active branch. Then continue with the next slice.

## Editing

- Keep changes focused and preserve unrelated user modifications in the worktree.
- Use `apply_patch` for edits to existing files and ASCII text by default.
- Do not add unnecessary comments, refactor unrelated code, create branches, or commit unless the task or repository workflow requires it.
- Do not use destructive Git commands such as `git reset --hard` or `git checkout --`.
