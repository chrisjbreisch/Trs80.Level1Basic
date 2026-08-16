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
| Recent focus | Type declarations, arrays, numeric precision, Level II built-ins, `CLEAR`, `MID$` assignment, user-defined functions, string-function compatibility, and scanner/parser keyword coverage |
| Latest implementation commit | `cfc8b9c Add POKE keyword support` |
| Latest documentation checkpoint | This update: expand the Level II compatibility-program corpus with numeric base formatting |
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
| Remaining Level II functions | Partial | The native-function registry is centralized in `NativeFunctions`; `CVI`, `CVS`, `CVD`, `MKI$`, `MKS$`, and `MKD$` are now registered and covered for valid little-endian binary conversion. | Build an authoritative manual checklist and add one focused test for every remaining missing function. |

## Statements and Commands

| Area | Status | Implemented now | Remaining work and evidence |
| --- | --- | --- | --- |
| Program editing commands | Partial | `LIST` with closed, open-ended, or reversed line ranges, `LOAD`, `SAVE`, `MERGE`, `CLEAR`, and explicit single-line, closed-range, or open-ended `DELETE` exist; `DEL.`, `LO.`, `ME.`, and `SA.` are accepted as DELETE, LOAD, MERGE, and SAVE abbreviations. `CLEAR` resets variables and arrays while preserving the program. `CLOAD` and `CSAVE` are intentionally unavailable; disk-based `LOAD` and `SAVE` are the supported equivalents. `AUTO` and `EDIT` remain deferred to the separate line-editor subsystem. | Verify remaining command abbreviations after the line editor is implemented. Existing anchor: `CommandTest`, `FileTest`. |
| Data statements | Implemented | `DATA`, `READ`, and `RESTORE` exist. | Add mixed-type, exhaustion, and `CLEAR` interaction cases. Existing anchor: `DataTest`. |
| Console input/output | Implemented | `INPUT`, `PRINT`, `TAB`, `SPC`, and cursor-related behavior exist. | Audit formatting, commas/semicolons, input errors, and Level II line-width behavior. Existing anchors: `InputTest`, `PrintTest`, `NativeFunctionTest`. |
| Hardware statements | Partial | `SET`, `RESET`, `POKE`, and related APIs exist as host-machine abstractions; explicit `SET x, y`, `RESET x, y`, and `POKE address, value` scanner/parser dispatch now routes to the existing graphics and memory APIs. `Level2CompatibilityTest` locks the combined graphics/memory behavior. | Specify whether each operation is emulated, approximated, rejected, or intentionally unavailable; add equivalent contracts for intentionally limited audio, port, and printer behavior. |
| Remaining Level II commands | Partial | All VM-backed Level II command keywords are now inventoried with focused scanner/parser coverage. `BEEP`, `OUT`, `WAIT`, `LPRINT`, and `LLIST` remain limited by host audio, port, and printer policies; `SYSTEM`, `SET`, `RESET`, and `POKE` dispatch to existing lifecycle, graphics, and memory APIs. `AUTO` and `EDIT` are deferred to the line editor; `CLOAD` and `CSAVE` remain intentionally unavailable. | Define hardware policies, then implement the line editor before revisiting `AUTO` and `EDIT`. The parser statement dispatch is centralized in `Parser.Statement()`. |

## Runtime and Compatibility

| Area | Status | Implemented now | Remaining work and evidence |
| --- | --- | --- | --- |
| Level I regression suite | Implemented | Existing Level I behavior remains the compatibility baseline. | Run the full interpreter project after changes to shared parser, environment, or interpreter code. |
| Level II focused tests | Partial | Type declarations, arrays, built-ins, `CLEAR`, and `MID$` assignment have focused coverage distributed across `ExpressionTest`, `NativeFunctionTest`, `CommandTest`, and `ErrorTest`. | Consolidate new type and Level II cases into clearer files as the feature surface grows. |
| Level II compatibility programs | Partial | `Level2CompatibilityTest` now contains executable hardware and typed-array/string-assignment programs with stated expected output. | Add short programs grouped by the remaining feature families and attach manual references as the corpus grows. |
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
31. Added the `MKI$` pure built-in for two-byte integer-to-string conversion.
32. Added the `MKS$` pure built-in for four-byte single-to-string conversion.
33. Added the `MKD$` pure built-in for eight-byte double-to-string conversion.
34. Added scanner, parser, and statement dispatch support for the `BEEP` keyword with focused scanner coverage; runtime audio remains intentionally silent pending a host-audio policy.
35. Added scanner, parser, and statement dispatch support for the `OUT port, value` keyword form with focused scanner and parser coverage; runtime port output remains intentionally silent pending a hardware policy.
36. Added scanner, parser, and statement dispatch support for the `WAIT port, mask[, invert]` keyword form with focused scanner and parser coverage; runtime port polling remains intentionally silent pending a hardware policy.
37. Added scanner, parser, and statement dispatch support for the `LPRINT` keyword form with focused scanner and parser coverage; runtime printer output remains intentionally silent pending printer support.
38. Added scanner, parser, and statement dispatch support for the `LLIST` keyword with LIST-compatible ranges and focused scanner/parser coverage; runtime printer output remains intentionally silent pending printer support.
39. Added scanner, parser, and statement dispatch support for the no-argument `SYSTEM` keyword; execution ends the current BASIC run through the machine halt lifecycle.
40. Added explicit scanner, parser, and statement dispatch support for `RESET x, y`, routing execution to the existing graphics reset API with focused scanner/parser coverage.
41. Added explicit scanner, parser, and statement dispatch support for `SET x, y`, routing execution to the existing graphics set API while preserving the parenthesized native-call form.
42. Added explicit scanner, parser, and statement dispatch support for `POKE address, value`, routing execution to the existing memory API while preserving the parenthesized native-call form.
43. Closed the scanner/parser keyword inventory audit: VM-backed commands have focused coverage, while `AUTO`/`EDIT` and cassette commands have explicit deferred or unavailable policies.
44. Added the first executable Level II hardware compatibility program covering explicit `SET`, `RESET`, `POKE`, `POINT`, and `PEEK` behavior.
45. Started the Level II compatibility-program corpus with a typed-array and `MID$` assignment program.
46. Expanded the compatibility corpus with direct `CVI` and `MKI$` binary-conversion programs.
47. Expanded the compatibility corpus with `CVS`/`MKS$` and `CVD`/`MKD$` floating binary-conversion programs.
48. Expanded the compatibility corpus with a one-parameter `DEF FN` and `FOR`/`NEXT` loop program.
49. Expanded the compatibility corpus with an `ON ... GOSUB` computed-subroutine program.
50. Expanded the compatibility corpus with a `DATA`/`READ`/`RESTORE` data-stream program.
51. Expanded the compatibility corpus with `LEFT$`, `RIGHT$`, `INSTR`, and `VAL` string-inspection and numeric-prefix behavior.
52. Expanded the compatibility corpus with deterministic `SQR`, `ABS`, `SGN`, `UCASE$`, and `STRING$` behavior.
53. Expanded the compatibility corpus with `MID$`, `CHR$`, `ASC`, and `LEN` character and substring behavior.
54. Expanded the compatibility corpus with numeric truth behavior for `AND`, `OR`, `NOT`, `XOR`, `EQV`, and `IMP`.
55. Expanded the compatibility corpus with `CINT`, `FIX`, and `INT` positive and negative conversion behavior.
56. Expanded the compatibility corpus with optional `INSTR` start and `MID$` length arguments.
57. Expanded the compatibility corpus with deterministic `INPUT` and `PRINT` console interaction.
58. Expanded the compatibility corpus with fixed-length `DATE$` and `TIME$` shape behavior.
59. Expanded the compatibility corpus with combined `SPC` and `TAB` console formatting behavior.
60. Expanded the compatibility corpus with `CLEAR` variable/array reset while preserving the program.
61. Expanded the compatibility corpus with an `IF`/`THEN` conditional and inline `GOTO` branch.
62. Expanded the compatibility corpus with descending `FOR`/`NEXT` behavior using `STEP -1`.
63. Expanded the compatibility corpus with a two-dimensional array declaration, assignment, and readback program.
64. Expanded the compatibility corpus with typed string-array assignment and empty-element defaults.
65. Expanded the compatibility corpus with declared `DEFDBL` precision during division.
66. Expanded the compatibility corpus with `DEFINT` fractional assignment truncation.
67. Expanded the compatibility corpus with `DEFSNG` single-precision declaration and assignment behavior.
68. Expanded the compatibility corpus with `DEFSTR` string declaration and assignment behavior.
69. Expanded the compatibility corpus with ranged `DEFINT A-C` declaration and assignment behavior.
70. Expanded the compatibility corpus with an `ON ... GOTO` computed-branch program.
71. Expanded the compatibility corpus with a direct `GOSUB`/`RETURN` subroutine program.
72. Expanded the compatibility corpus with `STOP` break output and `CONT` resumption behavior.
73. Expanded the compatibility corpus with `RUN` starting from an explicit line number.
74. Expanded the compatibility corpus with `NEW` clearing the stored program before `RUN`.
75. Expanded the compatibility corpus with `SAVE`/`LOAD` program round-trip behavior across `NEW`.
76. Expanded the compatibility corpus with `MERGE` combining an existing program and a file fixture.
77. Expanded the compatibility corpus with closed-range `DELETE` program editing.
78. Expanded the compatibility corpus with closed-range `LIST` program listing.
79. Expanded the compatibility corpus with open-ended `LIST -end` program listing.
80. Expanded the compatibility corpus with open-ended `LIST start-` program listing.
81. Expanded the compatibility corpus with open-ended `DELETE start-` program editing.
82. Expanded the compatibility corpus with open-ended `DELETE -end` program editing.
83. Expanded the compatibility corpus with reversed `LIST` range normalization.
84. Expanded the compatibility corpus with reversed `DELETE` range normalization.
85. Expanded the compatibility corpus with the `DEL.` `DELETE` command abbreviation.
86. Expanded the compatibility corpus with the `LO.` `LOAD` command abbreviation.
87. Expanded the compatibility corpus with the `SA.` `SAVE` command abbreviation.
88. Expanded the compatibility corpus with the `ME.` `MERGE` command abbreviation.
89. Expanded the compatibility corpus with the `R.` `RUN` command abbreviation.
90. Expanded the compatibility corpus with the `ST.` `STOP` command abbreviation.
91. Expanded the compatibility corpus with `RESTORE` targeting an explicit DATA line.
92. Expanded the compatibility corpus with `CDBL` conversion from a declared single value.
93. Expanded the compatibility corpus with `CSNG` conversion from a declared double value and single-precision formatting.
94. Expanded the compatibility corpus with positive and negative `CINT` midpoint rounding.
95. Expanded the compatibility corpus with `VAL` returning zero for an invalid numeric prefix.
96. Expanded the compatibility corpus with a deterministic predicate for zero-argument `RND` range behavior.
97. Expanded the compatibility corpus with deterministic positive-control `RND(1)` behavior.
98. Expanded the compatibility corpus with deterministic `HEX$` and `OCT$` formatting behavior.

## Next Slice Queue

Work in this order unless manual research changes the dependency:

1. Define and test hardware-dependent behavior.
2. Add short Level II compatibility programs.
3. Implement the line editor as a separate subsystem, then revisit `AUTO` and `EDIT`.
4. Complete statement and command families one at a time as manual gaps are found.

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
