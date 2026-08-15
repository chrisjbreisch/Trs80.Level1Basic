# TRS-80 Level II BASIC Feature Matrix

This document is the working implementation map for the `level2/type-declarations` branch. It tracks language capability, compatibility confidence, and the next small implementation slice.

The project already has a mature Level I interpreter. Level II work is being added incrementally on top of that baseline. A feature is not considered complete merely because a method or token exists: the behavior needs a focused interpreter test and must preserve the Level I suite.

## Status Legend

| Status | Meaning |
| --- | --- |
| Implemented | Runtime support exists and focused tests cover the main behavior. Boundary or hardware differences may still be documented separately. |
| Partial | The common path works, but one or more syntax forms, type rules, boundaries, or compatibility policies remain open. |
| Not audited | The code may contain related support, but it has not been compared systematically with the Level II manual. |
| Not started | No implementation or compatibility test exists yet. |
| Intentionally limited | The feature is outside the emulator scope. The supported approximation or rejection policy must be explicit. |

## Current Branch Snapshot

| Item | Current state |
| --- | --- |
| Branch | `level2/type-declarations` |
| Baseline | Level I interpreter with existing command, expression, file, input, and control-flow suites |
| Recent focus | Type declarations, arrays, numeric precision, Level II built-ins, `CLEAR`, `MID$` assignment, user-defined functions, and string-function compatibility |
| Latest implementation commit | `99b741f Support forward DEF FN references` |
| Latest documentation checkpoint | This update: complete the `DEF FN` lifecycle slice |
| Validation habit | Run the narrowest affected test first, then the affected test class, then the full project for shared changes |

## Language Core

| Area | Status | Implemented now | Remaining work and evidence |
| --- | --- | --- | --- |
| `DEFINT`, `DEFSNG`, `DEFDBL`, `DEFSTR` | Implemented | Declarations, ranges, aliases, and assignment casting are handled in `Environment` and `Parser`. | Add overflow, redeclaration, and mixed suffix cases. Existing anchors: `ExpressionTest` declaration tests. |
| Typed scalar assignment | Partial | Integer, single, double, and string targets cast assigned values. Double values retain precision through division and conversion functions. | Define promotion and invalid-conversion rules from the Level II manual. Add a dedicated type-conversion test class when the rules are settled. |
| `DIM` arrays | Implemented | One- and two-dimensional arrays are supported. Declared upper bounds and negative-subscript checks are enforced. | Verify higher dimensions, redeclaration, implicit-array rules, and exact error behavior. Existing anchors: `ExpressionTest` array tests. |
| Typed arrays | Partial | Numeric and string element casting exists. Untouched string elements default to empty strings; numeric elements default to zero. | Verify redeclaration, array type changes, mixed suffixes, and bounds after `CLEAR`. |
| Arithmetic operators | Implemented | Existing Level I operators plus `MOD`; division preserves `double` values when either operand is double. | Confirm all promotion, overflow, and divide/modulo edge cases against Level II behavior. |
| Logical operators | Implemented | `AND`, `OR`, `NOT`, `XOR`, `EQV`, and `IMP` are implemented with numeric truthiness for `int`, `float`, and `double`. | Verify precedence and whether Level II uses numeric bitwise or boolean semantics in every operand context. Existing anchor: `LogicalTest`. |
| Equality and comparisons | Implemented | Numeric equality compares `int`, `float`, and `double` by value rather than runtime type. | Add mixed numeric ordering and string comparison compatibility cases. |
| Control flow | Implemented | Existing `FOR/NEXT`, `GOSUB/RETURN`, `IF`, `ON`, and branch forms are available. | Audit Level II-specific syntax and error behavior. Existing anchors: `FlowControlTest`, `LogicalTest`. |
| User-defined functions | Implemented | One-parameter `DEF FN` declarations support forward references, caller-value restoration, exact arity rejection, and registry reset after `NEW` or a fresh `RUN`. Existing anchor: `NativeFunctionTest`. | Verify additional manual syntax and type-conversion rules if required by the Level II manual. |

## Built-in Functions

| Area | Status | Implemented now | Remaining work and evidence |
| --- | --- | --- | --- |
| String inspection and slicing | Partial | `CHR$`, `ASC`, `LEN`, `LEFT$`, `RIGHT$`, `MID$`, `INSTR`, `STR$`, `VAL`, and case/trim helpers exist. Fractional numeric arguments truncate consistently, including `STRING$` character codes; `INSTR` supports its optional start position; `MID$` supports both `(source$, start)` and `(source$, start, length)`; empty-string behavior is covered; and `VAL` consumes a leading numeric prefix. | Audit remaining conversion and exact error behavior. Existing anchor: `NativeFunctionTest`. |
| `MID$` assignment | Implemented | Supports `MID$(A$, start, length) = value$` and optional length. Replacement is fixed-length, does not expand the target, ignores out-of-range starts and nonpositive arguments, and requires a string target. | Keep as a regression anchor while the broader string-function row is completed. |
| Numeric conversion | Implemented | `CINT`, `CSNG`, `CDBL`, `FIX`, and `INT` exist. `CINT`, `FIX`, and `INT` avoid unnecessary `float` narrowing for double inputs. | Verify overflow, exact midpoint rounding, and conversion from strings. Existing anchor: `NativeFunctionTest`. |
| Math functions | Implemented | `SQR`, `SIN`, `COS`, `TAN`, `ATN`, `LOG`, and `EXP` are registered and callable. | Confirm domain errors, precision, and Level II output expectations. |
| Random numbers | Partial | One- and zero-argument `RND` exist; numeric controls are truncated to integer controls. | Define negative-control seeding/repeat behavior, zero behavior, and repeatability. |
| Keyboard/input functions | Partial | `INPUT$` and `INKEY$` exist. | Verify blocking behavior, end-of-input, character case, and interaction with `INPUT`. |
| Memory/display functions | Partial | `PEEK`, `POKE`, `MEM`, `FRE`, `POS`, `CSRLIN`, `POINT`, `SPC`, and `TAB` exist. | Document host-machine emulation, address wrapping, cursor behavior, and unsupported hardware assumptions. |
| Remaining Level II functions | Partial | The native-function registry is centralized in `NativeFunctions`; `CVI`, `CVS`, and `CVD` are now registered and covered for valid two-, four-, and eight-byte little-endian input. | Build an authoritative manual checklist and add one focused test for every remaining missing function. |

## Statements and Commands

| Area | Status | Implemented now | Remaining work and evidence |
| --- | --- | --- | --- |
| Program editing commands | Partial | `LIST` with closed, open-ended, or reversed line ranges, `LOAD`, `SAVE`, `MERGE`, `CLEAR`, and explicit single-line, closed-range, or open-ended `DELETE` exist; `DEL.`, `LO.`, `ME.`, and `SA.` are accepted as DELETE, LOAD, MERGE, and SAVE abbreviations. `CLEAR` resets variables and arrays while preserving the program. `CLOAD` and `CSAVE` are intentionally unavailable; disk-based `LOAD` and `SAVE` are the supported equivalents. | Verify remaining Level II command abbreviations. Existing anchor: `CommandTest`, `FileTest`. |
| Data statements | Implemented | `DATA`, `READ`, and `RESTORE` exist. | Add mixed-type, exhaustion, and `CLEAR` interaction cases. Existing anchor: `DataTest`. |
| Console input/output | Implemented | `INPUT`, `PRINT`, `TAB`, `SPC`, and cursor-related behavior exist. | Audit formatting, commas/semicolons, input errors, and Level II line-width behavior. Existing anchors: `InputTest`, `PrintTest`, `NativeFunctionTest`. |
| Hardware statements | Partial | `SET`, `RESET`, `POKE`, and related APIs exist as host-machine abstractions. | Specify whether each operation is emulated, approximated, rejected, or intentionally unavailable. |
| Remaining Level II commands | Not audited | The parser statement dispatch is centralized in `Parser.Statement()`. | Compare the scanner keyword table and parser dispatch with the manual, then implement one command family per slice. |

## Runtime and Compatibility

| Area | Status | Implemented now | Remaining work and evidence |
| --- | --- | --- | --- |
| Level I regression suite | Implemented | Existing Level I behavior remains the compatibility baseline. | Run the full interpreter project after changes to shared parser, environment, or interpreter code. |
| Level II focused tests | Partial | Type declarations, arrays, built-ins, `CLEAR`, and `MID$` assignment have focused coverage distributed across `ExpressionTest`, `NativeFunctionTest`, `CommandTest`, and `ErrorTest`. | Consolidate new type and Level II cases into clearer files as the feature surface grows. |
| Level II compatibility programs | Not started | No dedicated manual-derived program corpus exists yet. | Add short programs grouped by feature; each should have a stated expected output and manual reference. |
| Error compatibility | Partial | Existing errors preserve BASIC-style `WHAT?`, `HOW?`, and `SORRY` output with detailed diagnostics. | Map Level II error cases and expected source-position behavior explicitly. |
| Line editor | Not started | The application accepts commands and program lines but has no dedicated editor subsystem. | Implement separately after language compatibility is stable: cursor movement, insert/delete, recall, and line replacement. |
| ROM/RAM emulation | Intentionally limited | Host-machine APIs provide bounded approximations for memory and graphics operations. | Keep hardware policy outside the parser; document unsupported ROM, cassette, and machine-specific behavior. |

## Completed Slice Log

Recent Level II slices, in order:

1. Type declarations and range support.
2. `DIM`, typed arrays, and multidimensional arrays.
3. String helpers, `INPUT$`, and `INKEY$`.
4. `MOD`, `XOR`, `EQV`, and `IMP`.
5. Math, memory, and zero-argument `RND` built-ins.
6. Double-aware numeric operations and conversions.
7. Declared array bounds, negative-subscript checks, and string-array defaults.
8. `SPC` and `CLEAR`.
9. `MID$` assignment with optional length and boundary validation.
10. Fractional argument coverage for `LEFT$`, `RIGHT$`, and `MID$`.
11. One-parameter `DEF FN` functions, forward references, and registration lifecycle.
12. Optional start-position support for `INSTR`.
13. Optional length support for `MID$` function calls.
14. Fractional character-code truncation for `STRING$`.
15. Empty-string behavior across string inspection and slicing functions.
16. Leading numeric-prefix parsing for `VAL`.
17. Explicit `DELETE line` scanner/parser support.
18. Inclusive `DELETE start-end` range support.
19. Inclusive `LIST start-end` range support.
20. Open-ended `LIST -end` and `LIST start-` range support.
21. Open-ended `DELETE -end` and `DELETE start-` range support.
22. `DEL.` abbreviation for the `DELETE` command.
23. `LO.` abbreviation for the `LOAD` command.
24. `SA.` abbreviation for the `SAVE` command.
25. `ME.` abbreviation for the `MERGE` command.
26. Reversed `LIST` range normalization.
27. Documented intentional limitation for cassette `CLOAD` and `CSAVE` commands.
28. Added the `CVI` pure built-in for two-byte string-to-integer conversion.
29. Added the `CVS` pure built-in for four-byte string-to-single conversion.
30. Added the `CVD` pure built-in for eight-byte string-to-double conversion.

## Next Slice Queue

Work in this order unless manual research changes the dependency:

1. Build the complete Level II scanner/parser keyword inventory.
2. Add missing pure built-in functions one at a time.
3. Complete statement and command families one at a time.
4. Define and test hardware-dependent behavior.
5. Add short Level II compatibility programs.
6. Implement the line editor as a separate subsystem.

## Slice Completion Checklist

For every slice:

1. Name one behavior and one owning abstraction.
2. Add the smallest focused test that can fail before the fix.
3. Make the smallest implementation or regression-guard change.
4. Run the focused test, then the affected class or classes.
5. Update this matrix if capability or status changed.
6. Run the full project when shared behavior changed.
7. Commit and push the slice separately.

## Matrix Maintenance Rules

- Use **Implemented** only when runtime behavior and focused tests both exist.
- Keep hardware approximations separate from language support.
- Link the relevant source/test files when a row becomes difficult to locate.
- Add a new row before implementing a feature that does not fit an existing area.
- Record intentional deviations from Level II instead of silently treating them as complete.
