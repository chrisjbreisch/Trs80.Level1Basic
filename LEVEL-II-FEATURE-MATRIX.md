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
| Latest implementation commit | `a3f1bdb Report scalar assignment type mismatches` |
| Latest documentation checkpoint | This update: add a compatibility program for scalar type-mismatch diagnostics |
| Validation habit | Run the narrowest affected test first, then the affected test class, then the full project for shared changes |

## Language Core

| Area | Status | Implemented now | Remaining work and evidence |
| --- | --- | --- | --- |
| `DEFINT`, `DEFSNG`, `DEFDBL`, `DEFSTR` | Implemented | Declarations and ranges apply to unsuffixed variables by initial letter; aliases and assignment casting are handled in `Environment` and `Parser`. Implicit `DEFINT` scalar assignments enforce the Level II 16-bit range. | Add redeclaration, array-overflow, and mixed suffix cases. Existing anchors: `ExpressionTest` declaration tests and `ErrorTest` overflow tests. |
| Typed scalar assignment | Partial | Integer, single, double, and string targets cast assigned values. Double values retain precision through division and conversion functions. | Define promotion and invalid-conversion rules from the Level II manual. Add a dedicated type-conversion test class when the rules are settled. |
| `DIM` arrays | Implemented | One- and two-dimensional arrays are supported. Declared upper bounds and negative-subscript checks are enforced. | Verify higher dimensions, redeclaration, implicit-array rules, and exact error behavior. Existing anchors: `ExpressionTest` array tests. |
| Typed arrays | Partial | Numeric and string element casting exists. Untouched string elements default to empty strings; numeric elements default to zero. Duplicate `DIM` declarations are rejected with `HOW?`. | Verify higher dimensions, implicit creation, array type changes, mixed suffixes, and bounds after `CLEAR`. |
| Arithmetic operators | Implemented | Existing Level I operators plus `MOD`; division preserves `double` values when either operand is double. | Confirm all promotion, overflow, and divide/modulo edge cases against Level II behavior. |
| Logical operators | Implemented | `AND`, `OR`, `NOT`, `XOR`, `EQV`, and `IMP` are implemented with numeric truthiness for `int`, `float`, and `double`; true results use `-1` and false results use `0`. | Verify precedence and whether Level II uses numeric bitwise or boolean semantics in every operand context. Existing anchor: `LogicalTest`. |
| Equality and comparisons | Implemented | Numeric and string equality and ordering are supported; numeric values compare by value across `int`, `float`, and `double`, while true relational results use the Level II numeric value `-1` and false results use `0`. | Keep mixed-type and string comparison cases as regression anchors. |
| Control flow | Implemented | Existing `FOR/NEXT`, `GOSUB/RETURN`, `IF`, `ON`, and branch forms are available, including spaced `GO TO`. Program `STOP` reports `BREAK IN line` and prints `READY` on the following line without an intervening blank line; `CONT` resumes at the next statement after `STOP`. `END`, `NEW`, and full program replacement through `LOAD` clear the continuation point, so `CONT` cannot resume beyond ended, erased, or replaced program text. | Audit remaining Level II-specific syntax and error behavior. Existing anchors: `FlowControlTest`, `LogicalTest`, `Level2CompatibilityTest`. |
| User-defined functions | Implemented | One-parameter `DEF FN` declarations support forward references, caller-value restoration, exact arity rejection, and registry reset after `NEW` or a fresh `RUN`. Existing anchor: `NativeFunctionTest`. | Verify additional manual syntax and type-conversion rules if required by the Level II manual. |

## Built-in Functions

| Area | Status | Implemented now | Remaining work and evidence |
| --- | --- | --- | --- |
| String inspection and slicing | Partial | `CHR$`, `ASC`, `LEN`, `LEFT$`, `RIGHT$`, `MID$`, `INSTR`, `STR$`, `VAL`, and case/trim helpers exist. `CHR$` and `STRING$` truncate fractional character codes and reject values outside the TRS-80 byte range; `INSTR` supports its optional start position; `MID$` supports both `(source$, start)` and `(source$, start, length)`; empty-string behavior is covered; and `VAL` consumes a leading numeric prefix. | Audit remaining conversion and exact error behavior. Existing anchor: `NativeFunctionTest`. |
| `MID$` assignment | Implemented | Supports `MID$(A$, start, length) = value$` and optional length. Replacement is fixed-length, does not expand the target, ignores out-of-range starts and nonpositive arguments, and requires a string target. | Keep as a regression anchor while the broader string-function row is completed. |
| Numeric conversion | Implemented | `CINT`, `CSNG`, `CDBL`, `FIX`, and `INT` exist. `CINT`, `FIX`, and `INT` avoid unnecessary `float` narrowing for double inputs. Unsuffixed numeric constants with more than seven significant mantissa digits are classified as double precision, while explicit `!` retains single precision. Ordinary large doubles use round-trip fixed formatting when an exponent is unnecessary; extreme magnitudes remain scientific. | Verify remaining overflow, exact midpoint rounding, and conversion from strings. Existing anchors: `NativeFunctionTest`, `ExpressionTest`, `PrintTest`. |
| Math functions | Implemented | `SQR`, `SIN`, `COS`, `TAN`, `ATN`, `LOG`, and `EXP` are registered and callable. | Confirm domain errors, precision, and Level II output expectations. |
| Random numbers | Partial | One- and zero-argument `RND` exist; numeric controls are truncated to integer controls; negative controls reseed the generator deterministically and repeat the seeded first value; zero control repeats the last generated value after initialization. | Verify the remaining manual boundary cases. |
| Keyboard/input functions | Partial | `INPUT$` remains length-bounded input and returns available characters without padding at end-of-input; `INKEY$` performs a non-blocking host key probe, normalizes printable input to uppercase, and returns available control characters. Extended keys with no character payload intentionally remain empty. | Define any TRS-80-specific extended-key encoding and verify interaction with `INPUT`. |
| Memory/display functions | Partial | `PEEK`, `POKE`, `MEM`, `FRE`, `POS`, `CSRLIN`, `POINT`, `SPC`, and `TAB` exist; memory addresses wrap symmetrically across the bounded 64K host memory for negative and positive out-of-range addresses. `PRINT AT` maps valid absolute display positions 0 through 1023 across the 64-by-16 character screen and wraps negative or overflowed positions back onto the display area. | Define the remaining hardware assumptions for unsupported `PRINT AT` edge cases and keep the explicit wrap policy documented. |
| Remaining Level II functions | Partial | The native-function registry is centralized in `NativeFunctions`; `CVI`, `CVS`, `CVD`, `MKI$`, `MKS$`, and `MKD$` are now registered and covered for valid little-endian binary conversion. | Build an authoritative manual checklist and add one focused test for every remaining missing function. |

## Statements and Commands

| Area | Status | Implemented now | Remaining work and evidence |
| --- | --- | --- | --- |
| Program editing commands | Partial | `LIST` with closed, open-ended, or reversed line ranges, `LOAD`, `SAVE`, `MERGE`, `CLEAR`, and explicit single-line, closed-range, or open-ended `DELETE` exist; `LIST` paginates only when another line remains after a 12-line page and prints `READY` immediately when listing completes. Bare `LOAD` and `SAVE` use the Windows open/save dialogs, while quoted paths remain available for scripts; cancelling either file dialog is a no-op, preserving the current program without a success message. `DEL.`, `LO.`, `ME.`, and `SA.` are accepted as DELETE, LOAD, MERGE, and SAVE abbreviations. `CLEAR` resets variables and arrays while preserving the program. Plain `AUTO` with optional start/increment and plain `EDIT line` are handled by the line editor. `CLOAD` and `CSAVE` are accepted as aliases for file-backed `LOAD` and `SAVE`; cassette transport is not emulated. The modern `EDIT` experience intentionally differs from the original TRS-80 keyboard workflow because the original keyboard had no arrow keys. | Verify remaining command abbreviations and richer editor forms. Existing anchor: `CommandTest`, `FileTest`. |
| Data statements | Implemented | `DATA`, `READ`, and `RESTORE` exist. READ source and declared scalar or array target types must both be string or both numeric; either mismatch direction reports `?TM ERROR`. Explicit `$` and implicit `DEFSTR` targets accept string DATA, and reading beyond the available DATA elements reports `?OD ERROR` at the unread target. | Keep exhaustion, explicit-target, and `CLEAR` lifecycle cases as regression anchors. Existing anchor: `DataTest`. |
| Console input/output | Implemented | `INPUT`, `PRINT`, `TAB`, `SPC`, and cursor-related behavior exist; comma-separated variable input such as `INPUT D,N` consumes one response line and assigns values in order, while semicolon-separated variables retain independent reads. | Audit remaining formatting, input errors, and Level II line-width behavior. Existing anchors: `InputTest`, `PrintTest`, `NativeFunctionTest`. |
| Hardware statements | Partial | `SET`, `RESET`, `POKE`, and related APIs exist as host-machine abstractions; explicit `SET x, y`, `RESET x, y`, and `POKE address, value` scanner/parser dispatch now routes to the existing graphics and memory APIs. Graphics coordinates wrap symmetrically across the 128-by-48 screen for negative and positive out-of-range values. `CLS` clears the complete graphics plane, including boundary pixels. `BEEP` routes through `IHost` to a short Windows console tone, while test hosts record the call without producing audio. `OUT` and `WAIT` are accepted as silent, non-blocking no-ops because no TRS-80 port devices are emulated. `LPRINT` and `LLIST` route through `IHost.Print`; `LLIST` honors closed and open-ended `LIST` line-range selection, the Windows host presents the standard print dialog, and test hosts capture documents. | Specify whether each remaining operation is emulated, approximated, rejected, or intentionally unavailable. |
| Remaining Level II commands | Partial | All VM-backed Level II command keywords are now inventoried with focused scanner/parser coverage. `BEEP` produces a short host tone; `OUT` and `WAIT` are intentionally silent, non-blocking no-ops because no TRS-80 port devices are emulated; `LPRINT` and `LLIST` invoke the host printer interface. `SYSTEM`, `SET`, `RESET`, and `POKE` dispatch to existing lifecycle, graphics, and memory APIs. `AUTO` and `EDIT` are handled by interactive input; `CLOAD` and `CSAVE` are file-backed aliases for `LOAD` and `SAVE`, not cassette emulation. | Add richer editor forms and revisit any remaining command abbreviations. The parser statement dispatch is centralized in `Parser.Statement()`. |

## Runtime and Compatibility

| Area | Status | Implemented now | Remaining work and evidence |
| --- | --- | --- | --- |
| Level I regression suite | Implemented | Existing Level I behavior remains the compatibility baseline. | Run the full interpreter project after changes to shared parser, environment, or interpreter code. |
| Level II focused tests | Partial | Type declarations, arrays, built-ins, `CLEAR`, and `MID$` assignment have focused coverage distributed across `ExpressionTest`, `NativeFunctionTest`, `CommandTest`, and `ErrorTest`. | Consolidate new type and Level II cases into clearer files as the feature surface grows. |
| Level II compatibility programs | Partial | `Level2CompatibilityTest` now contains executable hardware and typed-array/string-assignment programs with stated expected output. | Add short programs grouped by the remaining feature families and attach manual references as the corpus grows. |
| Error compatibility | Partial | Existing errors preserve BASIC-style `WHAT?`, `HOW?`, and `SORRY` output with detailed diagnostics. Syntax failures use `?SN ERROR`, undefined `GOTO`/`GO TO` targets use `?UL ERROR`, numeric overflow uses `?OV ERROR`, incompatible binary operands or scalar assignment types use `?TM ERROR`, and declared array-bound reads and assignments mark the failing expression in the source diagnostic. Missing-parenthesis `PRINT` syntax errors emit no partial expression output, while malformed quoted-string recovery preserves valid prefixes. | Map remaining Level II error cases and expected source-position behavior explicitly. |
| Console host and configured font | Implemented | The Windows entry point detects Windows Terminal and relaunches the application under `conhost.exe`, where `AppSettings:FontName` and `AppSettings:FontSize` can be applied through the legacy console API. Existing Command Prompt and `conhost.exe` launches are unchanged. | Keep the launch policy Windows-specific; Windows Terminal profile settings remain authoritative for programs intentionally hosted there. |
| Line editor | Partial | `LineEditorBuffer` owns cursor position and deterministic insertion, Home/End, left/right movement, backspace, delete, and clear behavior; `InputCommand` connects it to console key handling, including immediate Escape cancellation and Ctrl+U line clearing, redraws the edited line, preserves original and uppercase source forms, supports Up/Down recall with draft restoration through session-scoped `LineEditorHistory`, recognizes `AUTO`, `AUTO start`, and `AUTO start,increment` until a blank entry, and preloads existing lines for plain `EDIT line`. Numbered input replaces an existing program line through the interpreter's established `Replace` path. The modern cursor-based `EDIT` UI is an intentional compatibility deviation from the original arrow-less TRS-80 keyboard design. | Add richer `EDIT` command behavior and recall/rendering coverage as needed. |
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
27. Documented file-backed alias policy for cassette `CLOAD` and `CSAVE` commands without cassette transport emulation.
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
99. Expanded the compatibility corpus with `LCASE$`, `TRIM$`, `LTRIM$`, and `RTRIM$` normalization behavior.
100. Expanded the compatibility corpus with `STR$` numeric conversion and `SPACE$` formatting behavior.
101. Expanded the compatibility corpus with deterministic `POS` and `CSRLIN` cursor values.
102. Expanded the compatibility corpus with numeric zero results for false `AND`, `OR`, `NOT`, `XOR`, `EQV`, and `IMP` cases.
103. Expanded the compatibility corpus with deterministic divide-by-zero `HOW?` output and source diagnostics.
104. Expanded the compatibility corpus with deterministic invalid-statement `WHAT?` output and source diagnostics.
105. Expanded the compatibility corpus with deterministic fixed-length `INPUT$` keyboard input.
106. Expanded the compatibility corpus with deterministic empty-state `INKEY$` behavior.
107. Expanded the compatibility corpus with deterministic zero-angle `SIN`, `COS`, and `TAN` behavior.
108. Expanded the compatibility corpus with deterministic `LOG(1)` and `EXP(0)` behavior.
109. Expanded the compatibility corpus with deterministic `SQR(9)` behavior.
110. Fixed parsing and evaluation of explicit unary plus, so `PRINT +3.12` outputs `3.12` instead of `WHAT?`; added a focused regression test.
111. Added `?` as a scanner alias for `PRINT`, so `? 3` outputs `3`; added a focused regression test.
112. Fixed numeric formatting so `0.01` uses fixed `.01` output while `0.001` retains scientific notation; added boundary regressions.
113. Fixed parser progress on malformed nested-quote PRINT input, preventing a hang and preserving the partial `HE SAID` output before `WHAT?`.
114. Enabled multi-character numeric and string variable names at runtime; `NU=1` followed by `PRINT NU` now outputs `1`.
115. Applied the Level II first-two-character significance rule to variable storage, so `NUMBER=1` and `PRINT NU` refer to the same variable.
116. Rejected variable names containing the reserved `ON` keyword, so `GONE=7` reports `WHAT?`.
117. Added alphanumeric identifier support with optional `$` suffix, so `N1$="TOBY"` is a valid assignment.
118. Fixed string-variable assignment expressions, so `N1$=N2$` copies the source variable value.
119. Enabled string concatenation in assignment expressions, so string variables and literals can be combined with `+`.
120. Verified the first-two-character significance rule for string variables with `$` suffixes.
121. Verified the first-two-character significance rule for alphanumeric numeric variables.
122. Allowed integer-looking numeric literals beyond `Int32` range to fall back to Level II float parsing; `12345678901` now prints `1.23457E+10`.
123. Rejected adjacent text after a closed string literal, making malformed nested-quote PRINT forms consistently report `WHAT?` instead of evaluating trailing words as variables.
124. Verified explicit quoted-string concatenation remains valid after malformed-quote validation.
125. Added `^` exponentiation with higher precedence and fractional exponent support; `5^2` outputs `25` and `4^0.5` outputs `2`.
126. Verified right-associative exponentiation, so `2^3^2` evaluates as `2^(3^2)` and outputs `512`.
127. Verified exponentiation precedence over multiplication, so `2*3^2` outputs `18`.
128. Verified exponentiation precedence over division, so `8/2^2` outputs `2`.
129. Verified exponentiation precedence over `MOD`, so `10 MOD 2^2` outputs `2`.
130. Documented current unary-minus precedence: `-2^2` evaluates as `(-2)^2` and outputs `4`.
131. Verified exponentiation of variables and grouped expressions: `A^2` and `(A+1)^2` produce `25` and `36`.
132. Added explicit integer `%` suffix support, so `A%=3.9` stores the truncated integer value `3`.
133. Added explicit single-precision `!` suffix support, so `A!=3.9` preserves the single value.
134. Added explicit double-precision `#` suffix support, so `A#=1/3` stores a double value.
135. Verified the first-two-character significance rule for integer-suffixed variables.
136. Preserved high-precision decimal literals assigned to `#` variables and used round-trip double formatting, so `AB#=1.2345678901234567` retains all digits.
137. Verified the Level II 255-character maximum string capacity with a `STRING$`/`LEN` boundary regression.
138. Verified first-two-character significance for long alphanumeric string names with `$` suffixes.
139. Enforced the Level II 16-bit integer range when `%` variables are evaluated; `PRINT AB%` now reports overflow for `AB%=1234567`.
140. Verified valid `%` integer boundaries: `32767` and `-32768` are accepted.
141. Verified negative `%` overflow below the Level II lower bound; `-32769` reports `HOW?` at use.
142. Raised the double fixed-format threshold so `A#=14593*846` prints `12345678` instead of scientific notation.
143. Verified the double formatting boundary: values through eight digits remain fixed, while larger values use the established six-digit scientific format.
144. Enforced the documented single-precision `!` range at use; approximately `1.7E+38` is accepted and `1.8E+38` reports `HOW?`.
145. Enforced the documented double-precision `#` range at use; `1.8E+38` reports `HOW?`.
146. Verified a valid double-precision `#` value near the upper bound; `1.7E+38` is accepted.
147. Verified first-two-character normalization for long alphanumeric names with `%` suffixes.
148. Verified first-two-character normalization for long alphanumeric names with `!` suffixes.
149. Added comma statement separators for assignment-shaped continuations, so `A=3, B=5` executes both assignments.
150. Verified comma statement separators also handle string assignments, so `A$="A", B$="B"` executes both assignments.
151. Verified comma statement separators handle mixed numeric and string assignments.
152. Verified comma statement separators preserve complete numeric RHS expressions.
153. Verified comma statement separators preserve exponent expressions, including fractional powers.
154. Verified comma statement separators preserve parenthesized RHS expressions.
155. Verified comma statement separators preserve string-expression RHS assignments.
156. Verified comma statement separators preserve explicit `%` and `!` typed assignments.
157. Verified comma statement separators preserve high-precision `#` double assignments.
158. Verified first-two-character name collisions across comma-separated assignments.
159. Added Level II error labels: syntax/scan failures use `?SN ERROR`, and typed numeric overflows use `?OV ERROR`.
160. Fixed malformed nested-quote PRINT recovery so the valid prefix prints on its own line before `?SN ERROR`.
161. Added `D`/`d` exponent-marker support for high-precision double literals.
162. Verified negative `D` exponent literals and established scientific formatting for `1.23D-2`.
163. Verified positive `D` exponent literals with an explicit exponent, so `1.23D+2` evaluates to `123`.
164. Verified direct `D` exponent expressions without typed-variable assignment.
165. Added numeric-literal `#` suffix support, so `PRINT 1/3#` evaluates as double precision.
166. Added numeric-literal `!` suffix support, so `PRINT 1.23!` evaluates as single precision.
167. Added numeric-literal `%` suffix support, completing explicit `%`, `!`, and `#` numeric literal suffixes.
168. Enforced the 16-bit range for numeric `%` literals; `PRINT 32768%` now reports `?OV ERROR`.
169. Verified the negative numeric `%` boundary: `PRINT -32768%` is accepted while positive `32768%` remains overflow.
170. Verified negative numeric `!` suffix literals, such as `PRINT -1.23!`.
171. Updated the missing-assignment regression to the Level II `?SN ERROR` syntax label.
172. Verified long unquoted DATA strings and semicolon-separated PRINT string expressions after parser recovery changes.
173. Aligned syntax-error regression expectations with `?SN ERROR` while retaining `WHAT?` for runtime expression/statement errors.
174. Aligned unknown numeric identifier regressions with Level II default-zero semantics.
175. Aligned malformed `NEXT`, `READ`, `ON`, and function-argument regressions with Level II `?SN ERROR` diagnostics.
176. Aligned PRINT recovery and invalid-character regressions with Level II partial output and `?SN ERROR` diagnostics.
177. Stabilized the full `ErrorTest` baseline for Level II syntax labels, typed overflow labels, and diagnostic ordering.
178. Fixed parser end-of-line checks so valid terminal string expressions no longer emit false `?SN ERROR` diagnostics.
179. Aligned expression, native-function, and compatibility regressions with Level II high-precision numeric formatting.
180. Fixed bare-text string assignments so unquoted strings are preserved across single- and multi-statement lines.
181. Added IF...THEN...ELSE execution, preserved single-letter `R` variable contexts, and aligned the final unknown-identifier compatibility baseline.
182. Routed `BEEP` through the host abstraction, producing a short Windows console tone while keeping test hosts silent and observable.
183. Documented and tested the intentional silent, non-blocking no-op policy for `OUT` and `WAIT` without emulated TRS-80 port devices.
184. Routed `LPRINT` and `LLIST` through the host printer interface, with a Windows print dialog and deterministic captured documents in tests.
185. Started the line editor subsystem with a focused, console-independent editing buffer for cursor movement and insertion/deletion.
186. Connected the line editor buffer to console input with cursor movement, insertion, deletion, and terminal redraw support.
187. Added session-scoped Up-arrow command recall through the line editor history service.
188. Added Down-arrow recall toward newer history entries, clearing the buffer after the newest entry.
189. Added compatibility coverage for replacing an existing numbered program line through edited input.
190. Added a tested auto-numbering policy object as the foundation for `AUTO` command integration.
191. Integrated plain `AUTO` into interactive input with default 10-line numbering and blank-line termination.
192. Added `AUTO start` and `AUTO start,increment` option parsing to interactive input.
193. Added plain `EDIT line` input handling that preloads the existing source and routes the edited line through replacement.
194. Added Home and End cursor movement to the line editor buffer and interactive input handling.
195. Added Escape cancellation for the current line editor buffer.
196. Added draft preservation while navigating backward and forward through command history.
197. Added Ctrl+U as a line-clear shortcut alongside Escape cancellation.
198. Made Escape cancel the active input or edit immediately without submitting a blank line.
199. Aligned the command matrix with implemented `AUTO`/`EDIT` support and covered auto-numbering termination behavior.
200. Added `CLOAD` and `CSAVE` scanner aliases for file-backed `LOAD` and `SAVE` behavior.
201. Verified `CLOAD` and `CSAVE` execute the file-backed load/save round trip.
202. Added deterministic negative-control seeding and repeatability coverage for `RND`.
203. Added last-value repeat behavior for `RND(0)` with focused coverage.
204. Added non-blocking `INKEY$` host probing with deterministic available-key coverage.
205. Verified `INPUT$` returns partial available input at end-of-stream without padding.
206. Defined symmetric 64K address wrapping for `PEEK` and `POKE`, including negative addresses.
207. Corrected single- and double-precision formatting so ordinary hundredths such as `.07` remain fixed-point.
208. Verified the same fixed-point formatting for negative single-precision hundredths such as `-.07`.
209. Verified fixed-point formatting for double-precision hundredths such as `R#=.07`.
210. Corrected `LIST line` to emit only the requested line, distinct from open-ended `LIST line-`.
211. Added `LIST .` support for listing only the most recently entered or edited program line.
212. Made interactive `AUTO` display the active next line number as the editor prompt, advancing from `10 ` to `20 ` after submission.
213. Preserved available control characters in non-blocking `INKEY$` results, with deterministic Enter-key coverage.
214. Documented and tested the intentional empty result for extended keys without character payloads.
215. Verified `POS` and `CSRLIN` reflect the host cursor after console output.
216. Verified `POS` ignores its compatibility argument and returns the current cursor column.
217. Verified `CSRLIN` advances to the next 0-based row after a normal `PRINT` newline.
218. Defined symmetric screen-coordinate wrapping for negative `SET`, `RESET`, and `POINT` coordinates.
219. Verified `SPC` contributes no padding for nonpositive counts.
220. Verified `TAB` contributes no padding when its target is at or behind the current column.
221. Added 64-column wrapping to the fake host and verified cursor continuation after a full console line.
222. Restored the initial interactive prompt and made blank input scanner-safe by returning an empty `SourceLine`.
223. Made `InputCommand` honor the workflow `WritePrompt` setting while preserving numbered `AUTO` prompts.
224. Made Pause/Break exit interactive `AUTO` mode without submitting the current line.
225. Added direct `InputCommand` coverage for the Pause/Break `AUTO` exit path.
226. Added direct `InputCommand` coverage for AUTO source-line progression from 10 to 20.
227. Verified `CINT` exact midpoint rounding away from zero for positive and negative values.
228. Added 7-byte scalar-variable allocation accounting to `MEM`, including the `A=1` reduction from 15572 to 15565.
229. Added comma-separated multi-variable `INPUT` support for reading multiple values from one response line.
230. Preserved semicolon-separated `INPUT` as independent reads while restricting bulk reads to comma syntax.
231. Added apostrophe comments as `REM` aliases outside quoted strings.
232. Rounded the compound-interest sample's currency output to cents using `CINT`.
233. Verified `CINT` converts numeric string arguments with the same midpoint rounding rules.
234. Enforced the Level II 16-bit range for `CINT` and covered direct and rounded overflow reporting.
235. Enforced the documented single-precision range for `CSNG` with overflow coverage.
236. Made `EDIT line` display the selected line number while preloading the editable source.
237. Documented the intentional modern `EDIT` UI deviation from the original arrow-less TRS-80 keyboard workflow.
238. Enforced the documented double-precision range for `CDBL` with overflow coverage.
239. Made numbered program syntax errors report `?SN ERROR IN line` while preserving generic command syntax errors.
240. Opened `EDIT` automatically on the failing line after a numbered program syntax error.
241. Added regression coverage for symmetric negative-coordinate wrapping in `RESET`.
242. Verified bare `LOAD` and `SAVE` commands use Windows file-dialog paths while quoted paths remain supported.
243. Fixed `CLS` to clear the final graphics row and column as part of the full 128-by-48 screen.
244. Added regression coverage for positive-boundary wrapping of `SET` and `POINT` coordinates.
245. Added regression coverage for positive-boundary wrapping of `POKE` and `PEEK` addresses.
246. Verified `PRINT AT 1023` maps to the final display position at column 63, row 15.
247. Verified closed-range `LLIST` sends only selected program lines to the host printer.
248. Verified lower-bounded open-ended `LLIST` sends the starting line and all following lines to the host printer.
249. Fixed `LIST` to skip the final-page key wait, preserve between-page pauses, and print `READY` immediately on completion.
250. Updated `STOP` to report Level II `BREAK IN line` output with no blank line before `READY`.
251. Cleared the continuation point on `END` so `CONT` cannot execute following program lines.
252. Cleared the continuation point on `NEW` so `CONT` cannot execute erased program lines.
253. Cleared the continuation point during `LOAD` program replacement so `CONT` cannot execute stale program lines.
254. Preserved the current program when the Windows LOAD dialog is cancelled.
255. Verified cancelling the Windows SAVE dialog preserves the current program and emits no success message.
256. Applied DEF type declarations by variable initial and enforced 16-bit overflow on implicit `DEFINT` scalar assignments.
257. Preserved DEFDBL assignment precision by classifying unsuffixed constants with more than seven significant digits as double precision.
258. Preserved large double literals through evaluation and removed the artificial eight-digit scientific-format boundary.
259. Added Level II `?TM ERROR` diagnostics for incompatible binary operand types.
260. Reported `?TM ERROR` when `READ` targets a numeric variable with string DATA.
261. Reported `?TM ERROR` for numeric DATA read into string targets while honoring implicit `DEFSTR` targets.
262. Extended symmetric `READ` type-mismatch checks and target diagnostics to array elements.
263. Reported `?TM ERROR` for scalar string/numeric assignment mismatches in either direction.
264. Expanded the compatibility corpus with a scalar type-mismatch program and its `?TM ERROR` diagnostic.
265. Expanded the compatibility corpus with a `DATA`/`READ` type-mismatch program and its `?TM ERROR` diagnostic.
266. Relaunched Windows Terminal-hosted application instances under `conhost.exe` so configured console fonts are applied; preserved normal existing console launches.
267. Expanded the compatibility corpus with an array `DATA`/`READ` type-mismatch program and its `?TM ERROR` diagnostic.
268. Expanded the compatibility corpus with an incompatible binary-expression program and its `?TM ERROR` diagnostic.
269. Expanded the compatibility corpus with a typed integer-overflow program and its `?OV ERROR` diagnostic.
270. Expanded the compatibility corpus with a negative typed integer-overflow program and its `?OV ERROR` diagnostic.
271. Applied symmetric `?TM ERROR` validation to typed array element assignments before environment casting.
272. Expanded the compatibility corpus with a typed single-precision overflow program and its `?OV ERROR` diagnostic.
273. Expanded the compatibility corpus with a typed double-precision overflow program and its `?OV ERROR` diagnostic.
274. Expanded the compatibility corpus with an explicit integer-literal overflow program and its `?OV ERROR` diagnostic.
275. Expanded the compatibility corpus with an oversized-array program and its `SORRY` memory diagnostic.
276. Expanded the compatibility corpus with an invalid-goto program and its `HOW?` flow-control diagnostic.
277. Expanded the compatibility corpus with an invalid ON GOTO program and its HOW? flow-control diagnostic.
278. Added a `PRINT AT` wrap policy for negative and overflowed display positions with a focused cursor-position regression.
279. Allowed a blank `EDIT line` submission to delete the selected numbered program line through the normal replacement path.
280. Applied blank-line `EDIT` deletion consistently to syntax-error recovery through the pending-edit path.
281. Added the interactive `ED.` abbreviation for selecting an existing line through `EDIT`.
282. Added the interactive `AU.` abbreviation for starting `AUTO` line numbering.
283. Added parser regression coverage for the existing `IN.` `INPUT` abbreviation.
284. Added parser regression coverage for the existing `REA.` `READ` abbreviation.
285. Added parser regression coverage for the existing `RET.` `RETURN` abbreviation.
286. Added parser regression coverage for the existing `REST.` `RESTORE` abbreviation.
287. Added parser regression coverage for the existing `ST.` `STOP` abbreviation.
288. Added parser regression coverage for the existing `DEL.` `DELETE` abbreviation.
289. Gated Level I parser acceptance for Level II-only syntax families: exponentiation (`^`), `D`-exponent literals, multidimensional `DIM`, `MID$` assignment, selected statements (`BEEP`, `OUT`, `WAIT`, `LPRINT`, `LLIST`, `PRINT AT`), and binary-conversion built-ins (`CVI`, `CVS`, `CVD`, `MKI$`, `MKS$`, `MKD$`), while preserving Level II acceptance in focused `Level1SyntaxGatingTest` coverage.
290. Added profile-aware runtime diagnostics and conversion policy: Level1 now reports classic `WHAT?`/`HOW?` vocabulary, Level2 preserves `?SN`/`?OV`/`?TM`, Level II-only binary conversion built-ins are excluded from Level1 function registration, and focused `LevelRuntimeProfileTest` coverage verifies cross-profile behavior.
291. Added typed scalar edge-case coverage for declaration redeclaration and explicit suffix precedence: the last `DEF*` declaration for an initial now has focused regression coverage, and explicit `%`/`!`/`#` suffixes remain authoritative over unsuffixed `DEF*` defaults.
292. Added focused DATA lifecycle coverage proving `CLEAR` resets the `READ` stream position, so a subsequent `READ` restarts from the first DATA element without requiring explicit `RESTORE`.
293. Rejected duplicate `DIM` declarations with focused `HOW?` runtime coverage instead of silently replacing array dimensions.
294. Enforced the `CHR$` byte range with fractional truncation and focused out-of-range coverage.
295. Applied the same byte-range policy to `STRING$` with focused out-of-range coverage.
296. Added `?OD ERROR` handling and focused coverage for reading beyond available `DATA` elements.
297. Added source-position coverage so `?OD ERROR` identifies the first unread `READ` target.
298. Added source-position coverage for declared array-bound read errors, marking the failing subscript expression.
299. Added source-position coverage for declared array-bound assignment errors, marking the failing subscript expression.
300. Verified mixed numeric ordering comparisons across `int`, `float`, and `double` values.
301. Corrected relational truth values so true comparisons such as `PRINT 4>3` output `-1` and false comparisons output `0`.
302. Corrected `AND`, `OR`, `NOT`, `XOR`, `EQV`, and `IMP` true results to output Level II `-1` instead of `1`.
303. Prevented partial `PRINT` output before deferred missing-parenthesis `?SN ERROR` diagnostics while preserving malformed-string prefix recovery.
304. Added Level II string equality and ordering support with focused `PRINT "YES" > "NO"` coverage.
305. Added spaced `GO TO` parsing with focused execution coverage for a multi-line branch program.
306. Added `?UL ERROR` handling for undefined direct and spaced `GOTO` targets with source-position coverage.
307. Extended `?UL ERROR` handling to undefined computed `ON ... GOTO` targets with selected-entry source coverage.
308. Verified `?UL ERROR` handling for undefined computed `ON ... GOSUB` targets with selected-entry source coverage.
## Next Slice Queue

Work in this order unless manual research changes the dependency:

1. Define and test hardware-dependent behavior.
2. Add short Level II compatibility programs.
3. Implement the line editor as a separate subsystem, then revisit `AUTO` and `EDIT`.
4. Complete statement and command families one at a time as manual gaps are found.

## Ordered Completion Checklist

This checklist turns the remaining `Partial` and `Not audited` work into the next implementation sequence. Each item remains subject to the repository slice workflow: one behavior, one focused regression, one minimal change, affected validation, matrix update, separate commit, and push.

1. [ ] Confirm the Level I/Level II language-profile contract and document which syntax, typing, and diagnostics differ by profile.
2. [ ] Audit typed scalar promotion, invalid conversions, redeclarations, and mixed suffix behavior against the Level II manual.
3. [ ] Audit typed arrays for higher dimensions, implicit creation, redeclaration, type changes, bounds, and `CLEAR` interaction.
4. [ ] Audit string-function boundaries and conversion errors for `CHR$`, `ASC`, `LEN`, `LEFT$`, `RIGHT$`, `MID$`, `INSTR`, `STR$`, `VAL`, and case/trim helpers.
5. [ ] Audit numeric promotion, overflow, midpoint rounding, scientific/fixed formatting, and string-to-number conversions.
6. [ ] Audit logical-operator precedence and confirm numeric bitwise versus truth-value behavior in every operand context.
7. [ ] Audit random-number controls and keyboard edge cases, including TRS-80 extended-key policy for `INKEY$`.
8. [ ] Complete DATA/READ/RESTORE exhaustion, explicit-target, and `CLEAR` lifecycle coverage.
9. [ ] Complete error-compatibility coverage for source positions, partial output, syntax, overflow, type mismatch, and array bounds.
10. [ ] Complete the authoritative Level II built-in-function and statement inventory, adding one focused test for every remaining missing or intentionally limited item.
11. [ ] Formalize graphics, memory, display, and printer policies, retaining symmetric address/coordinate wrapping where already implemented.
12. [ ] Keep `OUT` and `WAIT` explicitly silent and non-blocking while no TRS-80 port devices are emulated.
13. [ ] Keep `CLOAD` and `CSAVE` explicitly file-backed aliases; cassette transport emulation remains unavailable.
14. [ ] Keep `PRINT AT` out-of-range behavior explicitly documented as wraparound across the 64-by-16 display.
15. [ ] Add richer `EDIT` forms and recall/rendering coverage where supported by the manual and current host UI.
16. [ ] Add richer `AUTO` forms and numbering-boundary coverage where supported by the manual.
17. [ ] Expand executable Level II compatibility programs for every completed family and keep host-dependent expectations deterministic.

### Explicit Hardware And Compatibility Policies

- `OUT` and `WAIT`: accepted as silent, non-blocking no-ops because no TRS-80 port devices are emulated.
- `BEEP`: routed through `IHost`; Windows hosts may produce a short tone while test hosts record the call without audio.
- `LPRINT` and `LLIST`: routed through `IHost.Print`; Windows hosts use the print dialog and test hosts capture documents.
- `CLOAD` and `CSAVE`: file-backed aliases for `LOAD` and `SAVE`; cassette transport is not emulated.
- `PRINT AT`: valid positions map to the 64-by-16 character display; negative and overflowed positions wrap onto that display.
- ROM/RAM and graphics: bounded host-machine approximations remain outside the scope of ROM emulation.
- Modern `EDIT`: cursor-based editing is an intentional compatibility deviation from the original arrow-less TRS-80 keyboard.

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
