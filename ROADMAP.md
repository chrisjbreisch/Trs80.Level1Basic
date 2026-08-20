# TRS-80 BASIC Completion Roadmap

This roadmap covers two connected goals:

1. Complete the Level II BASIC compatibility surface tracked by [LEVEL-II-FEATURE-MATRIX.md](LEVEL-II-FEATURE-MATRIX.md).
2. Add a runtime language-profile toggle so the application can run in strict Level I or Level II mode.

The current interpreter is a unified Level II-aware engine. Level I programs generally run as a subset of that behavior, but there is no explicit runtime mode today. The roadmap preserves the existing Level II behavior as the default while introducing an injected profile boundary instead of duplicating the scanner, parser, and interpreter.

## Decisions

- Level I is a strict profile: Level II-only syntax is rejected, and Level I typing and error semantics are enforced where the manual defines them.
- Level II remains the default to preserve the current branch behavior and existing Level II work.
- Configuration is the primary toggle surface.
- Environment variables override JSON through the existing configuration provider, using `AppSettings__BasicLevel`.
- CLI mode selection is out of scope unless it is requested later.
- Hardware behavior remains explicit and host-bound; unsupported TRS-80 devices are not silently emulated.
- Existing unrelated test failures are not fixed as part of a mode slice unless a focused regression proves the mode work caused them.

## Current Baseline

The implementation map and completed slice history are maintained in [LEVEL-II-FEATURE-MATRIX.md](LEVEL-II-FEATURE-MATRIX.md). Current areas marked `Partial` or requiring further audit include:

- Typed scalar and array edge cases.
- Numeric promotion, precision, conversion, and formatting boundaries.
- String-function conversion and boundary behavior.
- Random-number and keyboard-function compatibility edges.
- Memory/display and hardware policy boundaries.
- Remaining manual coverage for built-in functions and statements.
- Richer `EDIT` and `AUTO` forms.
- Error diagnostics and source-position behavior.
- A strict Level I versus Level II behavior boundary.

Before implementation changes, establish a baseline using the SDK pinned by [global.json](global.json):

```powershell
dotnet restore .\Trs80.Level1Basic.sln
dotnet build .\Trs80.Level1Basic.sln --no-restore
dotnet test .\Trs80.Level1Basic.Interpreter.Test\Trs80.Level1Basic.Interpreter.Test.csproj --no-restore
```

Record pre-existing failures separately. The repository's slice workflow remains: one behavior, one focused test, one minimal change, affected validation, matrix update, separate commit, and push.

## Phase 0: Roadmap And Baseline

- [x] Materialize this roadmap at the repository root.
- [x] Capture the solution build and interpreter-test baseline.
- [x] Record known unrelated failures without folding them into mode work.
- [x] Convert remaining `Partial` and `Not audited` matrix rows into an ordered manual checklist.
- [x] Keep hardware approximations and intentional compatibility deviations explicit in the matrix.

**Exit criteria:** the baseline is reproducible, known failures are identified, and the next Level II slice is unambiguous.

Phase 0 is complete. The solution build succeeded, the interpreter baseline and unrelated failures are recorded above, and the ordered checklist plus explicit hardware policies are now maintained in [LEVEL-II-FEATURE-MATRIX.md](LEVEL-II-FEATURE-MATRIX.md).

## Phase 1: Define The Language-Mode Contract

**Owning areas:** `Trs80.Level1Basic.Common`, `IAppSettings`, `AppSettings`, and application startup.

- [x] Add a shared `BasicLanguageLevel` contract with `Level1` and `Level2` values.
- [x] Define `Level2` as the default to preserve current behavior.
- [x] Add the configured level to `IAppSettings` and `AppSettings`.
- [x] Add an explicit `AppSettings:BasicLevel` default to the application JSON.
- [x] Preserve environment-variable override through `AppSettings__BasicLevel`.
- [x] Reject unknown values during startup with a clear configuration error.
- [x] Register the resolved profile as a singleton in `Bootstrapper`.
- [x] Allow test construction to inject an explicit profile without process-global state.
- [x] Add focused tests for default, explicit Level I, explicit Level II, environment override, and invalid values.

**Exit criteria:** startup and tests can select either profile deterministically, with Level II behavior unchanged when no setting is supplied.

Phase 1 is complete. Configuration selects Level1 or Level2, the resolved profile is available through dependency injection, explicit test overrides avoid process-global state, and the current syntax/runtime behavior remains unchanged pending Phase 2 gates.

## Phase 2: Gate Level II Syntax At The Owning Boundaries

**Owning areas:** `Scanner`, `Parser`, and parser-focused tests.

- [x] Confirm the Level I manual boundary for each Level II-only syntax form before implementation.
- [x] Identify and classify type declarations (`DEFINT`, `DEFSNG`, `DEFDBL`, `DEFSTR`) and numeric suffixes (`%`, `!`, `#`); preserve Level I-compatible `$` string suffixes.
- [x] Identify and classify double-exponent literals and exponentiation.
- [x] Identify and classify multidimensional `DIM` forms.
- [x] Identify and classify `MID$` assignment and other Level II-only statement forms.
- [x] Identify and classify Level II-only built-ins and commands.
- [x] Keep tokenization in `Scanner`; make parser acceptance profile-aware.
- [x] Reject type declaration statements in Level I with the existing syntax diagnostic.
- [x] Reject type declarations and numeric suffixes in Level I with the existing syntax diagnostic.
- [x] Preserve current Level II parsing behavior for the gated syntax families.
- [x] Add data-driven tests for numeric suffix cases in Level1.
- [x] Verify Level I-compatible string suffixes remain accepted.

**Exit criteria:** the parser has a documented and tested acceptance policy for Level I and Level II without duplicating the grammar.

**Phase 2 boundary policy:** tokenization remains shared, while the parser rejects the following forms in Level I with `?SN ERROR` and accepts them in Level II: type declarations; numeric `%`/`!`/`#` suffixes; `D`/`d` exponent literals; exponentiation; multidimensional `DIM`; `MID$` assignment; `BEEP`, `OUT`, `WAIT`, `LPRINT`, and `LLIST`; `PRINT AT`; and binary-conversion functions `CVI`, `CVS`, `CVD`, `MKI$`, `MKS$`, and `MKD$`. One-dimensional `DIM`, `$` string suffixes, and the shared Level I statement families remain accepted. The policy is covered by `Level1SyntaxGatingTest` data-driven tests and the existing Level II parser/interpreter suite.

Phase 2 is complete. The scanner still produces the shared token stream, the parser owns profile acceptance, Level I rejection and Level II acceptance are tested for every gated syntax family, and the full interpreter project remains green.

## Phase 3: Apply Level-Specific Runtime Semantics

**Owning areas:** `Environment`, `Interpreter`, `ExceptionHandler`, `NativeFunctions`, and `Trs80Api`.

- [ ] Make `Environment` profile-aware for default numeric types.
- [ ] Define profile-specific declaration, suffix-casting, array-typing, and string-capacity rules.
- [ ] Define integer, single, and double overflow behavior for each profile.
- [ ] Make `Interpreter` profile-aware for promotion and conversion behavior.
- [ ] Make `ExceptionHandler` profile-aware for error vocabulary, source positions, and diagnostics.
- [ ] Preserve current Level II `?SN`, `?OV`, and `?TM` behavior in Level II mode.
- [ ] Audit native-function registration and dispatch for Level I versus Level II availability.
- [ ] Decide per function whether Level I rejects it or intentionally shares behavior.
- [ ] Audit `AUTO`, `EDIT`, `BEEP`, `OUT`, `WAIT`, `LPRINT`, `LLIST`, `CLOAD`, `CSAVE`, and `PRINT AT` against the selected profile.
- [ ] Preserve explicit host policies for unsupported hardware and printer/cassette behavior.

**Exit criteria:** representative programs demonstrate distinct, documented Level I and Level II semantics, while existing Level II tests remain green.

## Phase 4: Complete The Remaining Level II Surface

**Current position:** Phase 4 is active. Phases 0, 1, and 2 are complete; Phase 3 remains open for broader profile-specific runtime semantics. The branch has 336 logged Level II slices. The current validation checkpoint is 727 passing tests in the full solution and 651 in the interpreter project. The remaining work is an ordered audit queue; the next implementation slice is typed scalar promotion and invalid conversions.

**What is already closed:** declarations and suffixes, arrays and `CLEAR`, string slicing and assignment, numeric/logical truth values, DATA lifecycle, direct/computed flow control, `ON ERROR`/`RESUME`/`ERL`, host hardware policies, and the line-editor foundation. See the matrix dashboard for the detailed evidence and commit history.

Work through [LEVEL-II-FEATURE-MATRIX.md](LEVEL-II-FEATURE-MATRIX.md) in dependency order:

1. **Typed scalar promotion and invalid conversions.** Inspect `Environment`, `Interpreter.CheckOperands`, and native conversion dispatch. Add mixed `%`/`!`/`#` assignments, string-to-number conversions, type mismatches, and overflow cases.
2. **Remaining typed-array rules.** Inspect `Environment.ValidateArrayIndex` and declaration/cast paths. Cover higher dimensions, suffix-driven type changes, redeclaration combinations, and bounds after `CLEAR`.
3. **String conversion and error boundaries.** Audit `CHR$`, `ASC`, `VAL`, `STR$`, `CVI`/`CVS`/`CVD`, and `MK*` for empty, malformed, invalid-length, and overflow inputs.
4. **Numeric promotion and formatting.** Audit mixed arithmetic, exact midpoint rounding, exponent thresholds, and conversions across `int`, `float`, and `double` in `Interpreter` and `Trs80Api`.
5. **DATA and error lifecycle.** Complete `RESTORE` target errors, `READ` exhaustion after `CLEAR`, `ON ERROR` state across `MERGE`/`CONT`, and remaining `ERL`/source-position cases.
6. **Random and keyboard compatibility.** Define remaining `RND` controls and the TRS-80 extended-key policy for `INKEY$`; cover end-of-input interaction with `INPUT$`.
7. **Hardware/display closure.** Finish explicit policies and tests for `PRINT AT`, cursor functions, graphics bounds, printer behavior, cassette aliases, `OUT`, and `WAIT`.
8. **Editor and compatibility corpus.** Add richer `EDIT`/`AUTO` forms and one deterministic executable compatibility program for each completed family.

For every slice:

- [ ] Name one behavior and one owning abstraction.
- [ ] Add the smallest failing regression or compatibility test.
- [ ] Make the smallest implementation change.
- [ ] Run the focused test, then the affected class or project.
- [ ] Update the feature matrix.
- [ ] Run the full interpreter project after shared parser, environment, interpreter, host, or runtime changes.
- [ ] Commit and push the slice separately.

**Phase 4 working rule:** do not mark a row `Implemented` from code presence alone. Close a slice only when its policy is stated, its focused regression passes, the affected project passes, the matrix and completion log are updated, and the commit is pushed.

**Exit criteria:** every matrix row is either `Implemented` with focused evidence or explicitly `Intentionally limited` with a documented policy and test.

## Phase 5: Startup, Documentation, And Migration

**Owning areas:** `Bootstrapper`, `Program`, `ConsoleApp`, configuration, workflow startup, and documentation.

- [ ] Resolve the configured profile before registering profile-aware services.
- [ ] Keep the `Interpreter.json` workflow graph unchanged unless profile data must be passed explicitly.
- [ ] Set an explicit default profile in `appSettings.json`.
- [ ] Document valid values and the `AppSettings__BasicLevel` override.
- [ ] Document strict Level I limitations and Level II-only forms.
- [ ] Add startup/configuration tests.
- [ ] Smoke-test application startup in Level I and Level II modes.
- [ ] Verify Windows Terminal/conhost relaunch preserves the selected configuration.
- [ ] Update matrix statuses and roadmap completion notes only after runtime behavior and focused tests exist.
- [ ] Link this roadmap from `README.md` if that matches the repository documentation convention.

**Exit criteria:** users can select a documented mode at startup, observe the expected syntax and runtime behavior, and switch modes without code changes.

## Dependencies And Parallel Work

- Phase 0 can proceed independently and establishes the baseline for all later work.
- Phase 1 blocks mode-aware implementation and should land before syntax or runtime gating.
- Configuration tests can proceed alongside the profile contract once its public shape is fixed.
- Phase 2 can proceed after the profile contract and may be split by syntax family.
- Phase 3 depends on the profile contract and parser acceptance policy; shared `Environment` or `Interpreter` changes require full interpreter-project validation.
- Phase 4 is the continuing Level II slice queue and can proceed family by family once the mode boundary is stable.
- Phase 5 depends on the profile contract and completed behavior; startup wiring, documentation, and smoke tests can proceed in parallel after service registration is finalized.

## Relevant Files

- `ROADMAP.md` — this roadmap and completion record.
- `LEVEL-II-FEATURE-MATRIX.md` — Level II capability status, completed slices, and manual gaps.
- `copilot-instructions.md` — repository workflow and validation rules.
- `Trs80.Level1Basic.Common/IAppSettings.cs` and `AppSettings.cs` — configuration contract.
- `Trs80.Level1Basic.Application/Bootstrapper.cs` — configuration binding and dependency injection.
- `Trs80.Level1Basic/appSettings.json` — default application settings.
- `Trs80.Level1Basic/Program.cs` and `Trs80.Level1Basic.Application/ConsoleApp.cs` — startup and conhost relaunch flow.
- `Trs80.Level1Basic/Interpreter.json` — workflow graph.
- `Trs80.Level1Basic.VirtualMachine/Scanner/Scanner.cs` and `Parser/Parser.cs` — tokenization and syntax acceptance.
- `Trs80.Level1Basic.VirtualMachine/Interpreter/Environment.cs` — variables, declarations, arrays, casting, and numeric limits.
- `Trs80.Level1Basic.VirtualMachine/Interpreter/Interpreter.cs` and `Exceptions/ExceptionHandler.cs` — runtime semantics and diagnostics.
- `Trs80.Level1Basic.VirtualMachine/Machine/NativeFunctions.cs` and `Trs80Api.cs` — built-in function policy.
- `Trs80.Level1Basic.Command/Commands/InputCommand.cs` and `Trs80.Level1Basic.Common/AutoLineNumbering.cs` — editor and AUTO behavior.
- `Trs80.Level1Basic.Interpreter.Test/` — focused mode, syntax, runtime, and compatibility tests.
- `Trs80.Level1Basic.Environment.Test/` and `Trs80.Level1Basic.Common.Test/` — isolated configuration and type-policy tests.

## Verification Commands

```powershell
# Restore only when dependencies are needed
dotnet restore .\Trs80.Level1Basic.sln

# Build after shared contract or startup changes
dotnet build .\Trs80.Level1Basic.sln --no-restore

# Run the narrowest new mode/configuration test first
dotnet test .\Trs80.Level1Basic.Interpreter.Test\Trs80.Level1Basic.Interpreter.Test.csproj --no-restore --filter "FullyQualifiedName~LanguageLevel"

# Run the affected test class or project
dotnet test .\Trs80.Level1Basic.Interpreter.Test\Trs80.Level1Basic.Interpreter.Test.csproj --no-restore

# Full solution validation before declaring completion
dotnet test .\Trs80.Level1Basic.sln --no-restore
```

Manual smoke checks should cover:

- `AppSettings:BasicLevel=Level1`.
- `AppSettings:BasicLevel=Level2`.
- `AppSettings__BasicLevel=Level1` overriding JSON.
- Level I rejection of a representative Level II-only construct.
- Level II acceptance of that same construct.
- Preservation of the selected mode through Windows Terminal/conhost relaunch.

## Completion Record

| Date | Milestone | Evidence |
| --- | --- | --- |
| 2026-08-18 | Roadmap created; strict Level I / Level II configuration approach approved | Plan recorded in session memory and materialized here |
| 2026-08-18 | Added the language-level contract and Level2 default | `BasicLanguageLevelTest`: 3 passed; solution build succeeded |
| 2026-08-18 | Verified BasicLevel startup binding and environment override | `BasicLevelConfigurationTest`: 3 passed |
| 2026-08-18 | Registered the resolved BasicLanguageLevel for dependency injection | `BasicLevelConfigurationTest`: 3 passed; solution build succeeded |
| 2026-08-18 | Completed Phase 0 baseline and implementation checklist | Solution build succeeded; interpreter baseline recorded; matrix checklist and hardware policies documented |
| 2026-08-18 | Completed Phase 1 language-mode contract | `BasicLevelConfigurationTest`: 5 passed; solution build succeeded |
| 2026-08-18 | Gated DEFINT/DEFSNG/DEFDBL/DEFSTR in Level1 mode | `Level1SyntaxGatingTest`: 2 passed; Level2 declaration acceptance preserved |
| 2026-08-18 | Gated numeric `%`/`!`/`#` suffixes while preserving `$` strings | `Level1SyntaxGatingTest`: 6 passed; solution build succeeded |

The full `Trs80.Level1Basic.Environment.Test` project currently has an unrelated pre-existing failure in `Environment_Can_Delete_Line` (expected `null`, received an empty string); the focused configuration tests remain green.

The 2026-08-18 Phase 0 baseline also found:

- Solution build: succeeded.
- `ErrorTest.Interpreter_Handles_Array_Max_Size`: expected `" 0 "`, received `"SORRY"`.
- `InputTest.Interpreter_Handles_Invalid_Input_Type`: expected output ending in `" 3.14159 "`, but received no matching output.

These failures are recorded as baseline defects and are excluded from language-profile work unless a focused regression shows that a mode change causes them.

The roadmap is complete when all phases have exit criteria evidence, the feature matrix has no unexplained partial rows, and both runtime profiles have focused and end-to-end compatibility coverage.
