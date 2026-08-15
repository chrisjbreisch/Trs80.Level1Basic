# .NET 10 Upgrade Matrix

This document tracks the migration of the solution from .NET 6 to .NET 10. It is separate from `LEVEL-II-FEATURE-MATRIX.md`, which tracks TRS-80 BASIC language compatibility.

## Status Legend

| Status | Meaning |
| --- | --- |
| Complete | Implemented and validated by the stated check. |
| In progress | The migration slice is underway and has partial validation. |
| Pending | Identified work not yet implemented. |
| Blocked | Requires a compatibility decision, external dependency, or environment change. |
| Intentional | Deliberately retained as-is or outside the upgrade scope. |

## Current Snapshot

| Item | Status | Evidence / next action |
| --- | --- | --- |
| SDK selection | Complete | `global.json` pins SDK `10.0.302`; `dotnet --version` resolves to `10.0.302`. |
| Project target frameworks | Complete | All 15 projects target `net10.0` or `net10.0-windows`. |
| Solution restore | Complete | Restore succeeded under .NET 10. |
| Solution build | Complete | `dotnet build Trs80.Level1Basic.sln` succeeds. |
| Runtime/package alignment | In progress | Framework-provided package cleanup is complete; older application/test packages and vulnerability warnings remain. |
| Test infrastructure | In progress | All five test projects use the aligned current package set; four projects pass, while the full interpreter suite still runs beyond the bounded window. |
| Application smoke test | Complete | Root-directory launch reaches the interactive application under .NET 10 after deployment files were switched to `AppContext.BaseDirectory`; the bounded smoke process was stopped after startup. |
| Full test suite under .NET 10 | Pending | Run after test infrastructure is upgraded; distinguish test-host failures from product failures. |
| Documentation | In progress | This matrix records the migration; update it after every upgrade slice. |

## Project Target Frameworks

| Project group | Projects | Target |
| --- | --- | --- |
| Cross-platform libraries/tools | `Trs80.Level1Basic.Common`, `Trs80.Level1Basic.GenerateAst` | `net10.0` |
| Windows application/runtime | `Trs80.Level1Basic`, `Trs80.Level1Basic.Application`, `Trs80.Level1Basic.Command`, `Trs80.Level1Basic.CommandModels`, `Trs80.Level1Basic.HostMachine`, `Trs80.Level1Basic.VirtualMachine`, `Trs80.Level1Basic.Workflow`, `Trs80.Level1Basic.TestUtilities` | `net10.0-windows` |
| Windows tests | `Trs80.Level1Basic.Common.Test`, `Trs80.Level1Basic.Environment.Test`, `Trs80.Level1Basic.Interpreter.Test`, `Trs80.Level1Basic.TestUtilities.Test`, `Trs80.Level1Basic.Trs80.Test` | `net10.0-windows` |

## SDK and Build Files

| File / concern | Status | Detail |
| --- | --- | --- |
| `global.json` | Complete | Pins `10.0.302`, `latestPatch` roll-forward, and prereleases disabled. |
| `Directory.build.props` | Complete | Existing version metadata remains unchanged; no central package management is present. |
| `.gitignore` | Complete | Local `.vscode/` workspace settings are ignored. |
| Solution format | Complete | Existing Visual Studio solution format remains usable with the .NET 10 SDK. |

## Runtime and Framework Dependencies

| Area | Current state | Status | Required action |
| --- | --- | --- | --- |
| Windows Forms | `HostMachine` uses `net10.0-windows` and `UseWindowsForms=true`. | Complete | Keep Windows targeting; run host/application smoke tests after package changes. |
| `System.Drawing.Common` | Removed from `HostMachine` and `VirtualMachine`; no source usage was found. | Complete | Both affected projects and the full solution build successfully without the package. |
| `Microsoft.CSharp` | Removed from `VirtualMachine`. | Complete | VirtualMachine builds and all 24 `ExpressionTest` tests pass under .NET 10. |
| `Microsoft.Extensions.*` | Explicit references in Application, Command, Common, Workflow, and the root app are aligned to `10.0.11`. | Complete | Restore/build succeeds and root-directory application startup reaches the interactive process. |
| `WorkflowCore` | Version `3.18.0` in Application and Workflow. | Complete | Restore/build succeeds; the root application loads the configured workflow, runs it synchronously, stops the workflow host, and reaches the interactive process without errors. |
| `NLog.Extensions.Logging` | Version `6.1.4`; NLog 6 configuration API is used. | Complete | Application build and bounded root-directory startup pass without logging API errors. |
| `Scrutor` | Version `7.0.0`. | Complete | Application and executable builds pass; root-directory startup reaches the interactive process without service-registration errors. |
| `Newtonsoft.Json` | Version `13.0.1` in Common. | Pending | Keep behavior stable; update only if restore/audit requires it. |

## Test Dependencies

| Package family | Current versions | Status | Required action |
| --- | --- | --- | --- |
| `Microsoft.NET.Test.Sdk` | `18.9.0` across all test projects | Complete | Restore and test execution work under .NET 10. |
| `MSTest.TestAdapter` | `4.3.3` across all test projects | Complete | Four project suites pass; interpreter suite builds but remains long-running. |
| `MSTest.TestFramework` | `4.3.3` across all test projects | Complete | Restore/build succeeds across the test projects. |
| `FluentAssertions` | `8.10.0` across all test projects | Complete | Existing assertions compile and pass in the completed suites. |
| `coverlet.collector` | `10.0.1` across all test projects | Complete | Package alignment restores successfully; coverage collection remains to be exercised separately. |

## Restore Warnings Baseline

The first .NET 10 restore/build succeeded but reported warnings that belong to the next dependency slices:

- `NU1510`: explicit `System.Drawing.Common` package is not pruned in `HostMachine`.
- `NU1510`: explicit `Microsoft.CSharp` package is not pruned in `VirtualMachine`.
- `NU1902`: `OpenTelemetry.Api` 1.1.0 has a known moderate vulnerability.
- `NU1903`: `System.Linq.Dynamic.Core` 1.2.13 has a known high-severity vulnerability.
- `NU1904`: `System.Linq.Dynamic.Core` 1.2.13 has a known critical-severity vulnerability.

These warnings must not be silently accepted as part of the final .NET 10 state. The package upgrade slice should remove or resolve them and record the resulting versions here.

## Upgrade Slices

| Slice | Scope | Status | Validation |
| --- | --- | --- | --- |
| 1 | Pin SDK and retarget all projects to .NET 10 | Complete | `dotnet --version`; full solution restore/build. Commit `acad1e6`. |
| 2 | Remove or align framework-provided package references | Complete | Removed `Microsoft.CSharp` and `System.Drawing.Common`; targeted builds, 24 expression tests, and full solution build pass. Commit `bd8daa7` contains the Microsoft.CSharp removal; this slice completes the remaining cleanup. |
| 3 | Upgrade Microsoft.Extensions and application dependencies | Complete | Microsoft.Extensions references are `10.0.11`; deployment files resolve from `AppContext.BaseDirectory`; application project and executable builds pass, and root-directory startup reaches the interactive process. |
| 4 | Validate or upgrade WorkflowCore, NLog, and Scrutor | Complete | WorkflowCore and WorkflowCore.DSL are `3.18.0`; NLog.Extensions.Logging is `6.1.4`; Scrutor is `7.0.0`; application build and root workflow/startup smoke validation pass. |
| 5 | Align MSTest, test SDK, adapter, framework, and coverlet | In progress | Package alignment is complete; Common (35), Environment (7), TestUtilities (16), and TRS-80 host (1) suites pass. The interpreter suite builds but hangs beyond 150 seconds and needs separate test-host investigation. |
| 6 | Resolve security warnings and review remaining packages | Next | Investigate the interpreter test hang, then address remaining package vulnerability warnings. |
| 7 | Run runtime and compatibility validation | Pending | Application smoke test, focused interpreter tests, full test suite, and representative BASIC programs. |
| 8 | Update README and close the migration | Pending | Document prerequisites, commands, final package versions, and residual limitations. |

## Recommended Next Slice

Microsoft.Extensions alignment and application startup validation are complete. The next slice is WorkflowCore, NLog, and Scrutor compatibility.

## Maintenance Rules

- Update this file after every .NET 10 migration slice.
- Record exact package versions and validation commands, not only a general status.
- Keep language compatibility changes in `LEVEL-II-FEATURE-MATRIX.md`.
- Do not combine package upgrades, test-runner changes, and behavioral refactors in one migration commit.
- Treat critical and high-severity restore warnings as unresolved until explicitly remediated or documented with a conscious exception.
- Preserve the existing Windows-only scope unless a separate portability effort is requested.
