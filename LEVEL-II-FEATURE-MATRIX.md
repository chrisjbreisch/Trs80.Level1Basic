# TRS-80 Level II BASIC Feature Matrix

This is the working implementation checklist for the `level2/type-declarations` branch. A feature is marked **implemented** only when the runtime behavior and focused tests are present.

## Language Core

| Area | Status | Current evidence / next step |
| --- | --- | --- |
| `DEFINT`, `DEFSNG`, `DEFDBL`, `DEFSTR` | Implemented | Type declarations, ranges, and assignment casting are in `Environment` and `Parser`. Add boundary and conversion cases as the next type slice. |
| Typed scalar assignment | Partial | Core conversions exist; numeric promotion, invalid conversions, and exact rounding rules need an explicit compatibility contract. |
| `DIM` arrays | Implemented | One- and two-dimensional arrays are supported. Higher dimensions and exact bounds/error behavior remain to be verified. |
| Typed arrays | Partial | Numeric and string element casting exists; default values, redeclaration, and bounds behavior need compatibility tests. |
| Arithmetic operators | Implemented | Existing Level I operators plus `MOD`. Numeric promotion and division edge cases need a dedicated matrix of tests. |
| Logical operators | Implemented | `AND`, `OR`, `NOT`, `XOR`, `EQV`, and `IMP` are present. Verify precedence and operand coercion against Level II behavior. |
| Control flow | Implemented | Existing Level I control flow is present. Validate Level II-specific forms and error behavior. |

## Built-in Functions

| Area | Status | Current evidence / next step |
| --- | --- | --- |
| String functions | Partial | `CHR$`, `ASC`, `LEN`, `LEFT$`, `RIGHT$`, `MID$`, `INSTR`, `STR$`, `VAL`, and case/trim helpers exist. Audit argument forms and boundary behavior against Level II. |
| Numeric conversion | Implemented | `CINT`, `CSNG`, `CDBL`, `FIX`, and `INT` exist. Confirm rounding and overflow behavior. |
| Math functions | Implemented | `SQR`, `SIN`, `COS`, `TAN`, `ATN`, `LOG`, and `EXP` exist. Confirm domain errors and precision behavior. |
| Random numbers | Partial | One- and zero-argument `RND` exist. Negative, zero, and repeatability/seeding semantics need compatibility tests. |
| Keyboard/input functions | Partial | `INPUT$` and `INKEY$` exist. Verify blocking, end-of-input, and character semantics. |
| Memory/display functions | Partial | `PEEK`, `POKE`, `MEM`, `FRE`, `POS`, `CSRLIN`, `POINT`, and related functions exist. Their emulation policy needs to be documented and tested. |
| Remaining Level II functions | Not audited | Build the authoritative function list from the Level II manual and add one matrix row and test per missing function. |

## Statements and Commands

| Area | Status | Next step |
| --- | --- | --- |
| Program editing commands | Partial | `LIST`, `LOAD`, `SAVE`, and `MERGE` exist. Decide the intended support policy for `CLOAD` and `CSAVE`. |
| Data statements | Implemented | `DATA`, `READ`, and `RESTORE` exist. Add Level II compatibility cases for mixed types and exhaustion errors. |
| Console input/output | Implemented | `INPUT`, `PRINT`, `TAB`, and related cursor functions exist. Audit formatting and input error behavior. |
| Hardware statements | Partial | `SET`, `RESET`, `POKE`, and related APIs exist as host-machine abstractions. Define behavior for unsupported ROM/hardware operations. |
| Remaining Level II commands | Not audited | Compare the parser keyword table with the Level II manual and implement one command family at a time. |

## Runtime and Compatibility

| Area | Status | Next step |
| --- | --- | --- |
| Level I regression suite | Implemented | The interpreter test project currently passes 297 tests. Keep this suite green after every slice. |
| Level II compatibility programs | Not started | Add small programs grouped by feature instead of starting with a large corpus. |
| Error compatibility | Partial | Existing error handling is detailed and useful; map Level II error cases and expected messages explicitly. |
| Line editor | Not started | Keep this separate from language compatibility and implement after the runtime surface is stable. |
| ROM/RAM emulation | Intentionally limited | Document approximations and unsupported hardware behavior rather than expanding the interpreter core around it. |

## Slice Order

1. Type conversion, promotion, array defaults, and bounds tests.
2. Complete the scanner/parser inventory from the Level II manual.
3. Add missing pure built-in functions one at a time.
4. Complete statement and command families one at a time.
5. Define and test hardware-dependent behavior.
6. Add Level II compatibility programs and update this matrix as each feature closes.
7. Implement line editing as a separate subsystem.
