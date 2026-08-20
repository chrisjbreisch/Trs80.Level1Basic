# TRS-80 Level I BASIC Feature Matrix

This document describes the strict `BasicLanguageLevel.Level1` profile implemented by the interpreter. Level I is the compatibility profile for the original TRS-80 Level I BASIC surface. It shares the scanner, parser architecture, runtime, host abstractions, and test harness with Level II, but rejects Level II-only syntax at the parser boundary and applies Level I default typing and diagnostics.

The matrix distinguishes language behavior from host limitations. A feature is complete only when the behavior is implemented, its profile policy is stated, and focused tests cover the relevant path.

## Profile Contract

| Item | Level I policy | Evidence |
| --- | --- | --- |
| Profile selection | Select with `AppSettings:BasicLevel=Level1`, `AppSettings__BasicLevel=Level1`, or explicit `TestController(BasicLanguageLevel.Level1)` construction. | `BasicLevelConfigurationTest`, `Level1SyntaxGatingTest` |
| Default profile | Application default remains Level II; Level I is opt-in. | `BasicLevelConfigurationTest` |
| Tokenization | Uses the shared scanner. Profile-specific rejection belongs to the parser. | `Parser`, `Scanner`, `Level1SyntaxGatingTest` |
| Default numeric type | Unsuffixed numeric variables default to 16-bit integer behavior. Fractional assignment truncates toward zero. | `Environment`, `Level1SyntaxGatingTest` |
| Explicit numeric suffixes | `%`, `!`, and `#` are rejected in Level I. | `Level1SyntaxGatingTest` data-driven suffix tests |
| String suffix | `$` remains accepted for Level I-compatible string variables. | `Level1SyntaxGatingTest` |
| Type declarations | `DEFINT`, `DEFSNG`, `DEFDBL`, and `DEFSTR` are Level II-only and rejected with `WHAT?`. | `Level1SyntaxGatingTest` |
| Syntax diagnostics | Level I parser and scanner failures use classic `WHAT?` vocabulary. | `ExceptionHandler`, `Level1SyntaxGatingTest` |
| Runtime diagnostics | Shared runtime policies remain available where the operation exists in Level I; profile-specific differences are documented below. | `ExceptionHandler`, `ErrorTest` |

## Language Core

| Area | Status | Level I behavior | Remaining policy |
| --- | --- | --- | --- |
| Variables | Implemented | Original single-letter numeric variables and `$` string variables are supported. Runtime name normalization follows the interpreter’s compatibility rules. | Keep Level I name limits and normalization covered by the existing baseline. |
| Unsuffixed numeric assignment | Implemented | Defaults to integer behavior; `A=3.9` stores and prints `3`. Values outside the Level I integer range report the profile’s runtime error. | Maintain boundary regressions. |
| String variables | Implemented | `$` variables store strings and default to empty string. | Maintain string assignment and concatenation regressions. |
| Arithmetic | Implemented | Core `+`, `-`, `*`, `/`, `MOD`, and supported grouping behavior are available where valid for Level I values. | Audit any remaining precision differences against the Level I manual. |
| Comparisons | Implemented | Numeric and string comparisons are supported by the shared runtime. True results are numeric `-1`; false results are `0`. | Keep comparison and truth-value regressions. |
| Logical operators | Implemented | `AND`, `OR`, `NOT`, `XOR`, `EQV`, and `IMP` use numeric truthiness. True results are `-1`; false results are `0`. | Confirm whether any strict Level I bitwise edge differs from the shared policy. |
| Control flow | Implemented | `IF`, `THEN`, `ELSE`, `GOTO`, spaced `GO TO`, `GOSUB`, `RETURN`, `FOR/NEXT`, `ON`, `STOP`, `CONT`, `END`, and `RUN` are supported. | Keep Level I flow-control and error tests green. |
| Data flow | Implemented | `DATA`, `READ`, `RESTORE`, and explicit `RESTORE line` are supported by the shared runtime. | Preserve the explicit Level I policy if manual review identifies a form outside the profile. |
| User-defined functions | Intentionally limited | `DEF FN` is treated as a Level II extension and is not part of the strict Level I surface. | Add a direct Level I rejection regression if strict manual compatibility requires it. |

## Rejected Level II Syntax

The Level I parser rejects these forms with the classic `WHAT?` diagnostic. The scanner still tokenizes the shared language surface so Level II can accept the same forms through the same grammar infrastructure.

| Syntax family | Level I result | Focused evidence |
| --- | --- | --- |
| `DEFINT`, `DEFSNG`, `DEFDBL`, `DEFSTR` | Rejected | `Level1SyntaxGatingTest.Level1_Rejects_Defint_Declaration` |
| Numeric `%`, `!`, `#` suffixes | Rejected | `Level1SyntaxGatingTest.Level1_Rejects_Numeric_Type_Suffixes` |
| `D`/`d` exponent literals | Rejected | `Level1SyntaxGatingTest.Level1_Rejects_Level2_Only_Syntax` |
| Exponentiation `^` | Rejected | `Level1SyntaxGatingTest.Level1_Rejects_Level2_Only_Syntax` |
| Multidimensional `DIM A(1,1)` | Rejected | `Level1SyntaxGatingTest.Level1_Rejects_Level2_Only_Syntax` |
| `MID$` assignment | Rejected | `Level1SyntaxGatingTest.Level1_Rejects_Level2_Only_Syntax` |
| `BEEP` | Rejected | `Level1SyntaxGatingTest.Level1_Rejects_Level2_Only_Syntax` |
| `OUT` | Rejected | `Level1SyntaxGatingTest.Level1_Rejects_Level2_Only_Syntax` |
| `WAIT` | Rejected | `Level1SyntaxGatingTest.Level1_Rejects_Level2_Only_Syntax` |
| `LPRINT` | Rejected | `Level1SyntaxGatingTest.Level1_Rejects_Level2_Only_Syntax` |
| `LLIST` | Rejected | `Level1SyntaxGatingTest.Level1_Rejects_Level2_Only_Syntax` |
| `PRINT AT` | Rejected | `Level1SyntaxGatingTest.Level1_Rejects_Level2_Only_Syntax` |
| `CVI`, `CVS`, `CVD` | Rejected | `Level1SyntaxGatingTest.Level1_Rejects_Level2_Only_Syntax` and profile native registration policy |
| `MKI$`, `MKS$`, `MKD$` | Rejected | Profile native registration policy |

## Built-in Functions

| Family | Status | Level I behavior | Policy |
| --- | --- | --- | --- |
| String inspection | Implemented | `ASC`, `LEN`, `LEFT$`, `RIGHT$`, `MID$`, `INSTR`, and `STRING$` are available where they belong to the shared Level I-compatible surface. | Preserve empty-string and boundary behavior. |
| Character conversion | Implemented | `CHR$` and `STRING$` use the host’s documented byte-range policy. | Character codes outside `0..255` are rejected by the shared runtime policy. |
| Numeric conversion | Implemented | `CINT`, `CSNG`, `CDBL`, `FIX`, `INT`, `STR$`, and `VAL` are available according to the profile’s supported surface. | Level I default integer behavior is distinct from Level II default single behavior. Keep conversion and overflow tests explicit. |
| Math | Implemented | `ABS`, `SGN`, `SQR`, `SIN`, `COS`, `TAN`, `ATN`, `LOG`, and `EXP` are shared where supported. | Domain and precision differences remain an audit item if the manual requires them. |
| Random | Implemented | `RND` is available with the shared deterministic test policy. | Hardware-specific random sequence identity is not promised. |
| Keyboard/input | Implemented | `INPUT`, `INPUT$`, and `INKEY$` use the host abstraction. | Extended-key payloads without character data intentionally return empty; end-of-input behavior is documented by tests. |
| Memory/display | Implemented/limited | `PEEK`, `POKE`, `MEM`, `FRE`, `POS`, `CSRLIN`, `SPC`, and `TAB` use bounded host approximations. | ROM/RAM identity and machine-specific display behavior are outside strict emulation scope. |
| Binary conversion | Intentionally unavailable | `CVI`, `CVS`, `CVD`, `MKI$`, `MKS$`, and `MKD$` are Level II-only and rejected by the parser/profile registry. | Do not silently approximate these functions in Level I. |

## Statements And Commands

| Family | Status | Level I behavior | Host or compatibility policy |
| --- | --- | --- | --- |
| Program editing | Implemented | `LIST`, `LOAD`, `SAVE`, `MERGE`, `CLEAR`, `DELETE`, `NEW`, and `RUN` are available through the shared command surface. | File dialogs are Windows-host behavior; quoted paths remain scriptable. |
| Immediate control flow | Implemented | Immediate `GOTO`, `GO TO`, `ON ... GO TO`, and `ON ... GOSUB` can execute into a loaded program. | Undefined targets report `?UL ERROR`; invalid selectors in program mode report `?FC ERROR`. |
| Error handling | Implemented/limited | `ON ERROR GO TO`, `ON ERROR GO TO 0`, `RESUME`, and `ERL` are available through the shared runtime. | Immediate divide-by-zero uses `?/0 ERROR` and `ERL=65535`; numbered program errors retain line numbers. |
| `AUTO` and `EDIT` | Implemented | Interactive line editing is available through the shared command subsystem. | Cursor-based editing is a documented modern-host deviation from the original arrow-less keyboard. |
| `BEEP` | Level II-only | Rejected in strict Level I syntax. | Level II routes it through `IHost`; test hosts remain silent and observable. |
| `OUT` and `WAIT` | Level II-only | Rejected in strict Level I syntax. | Level II accepts silent, non-blocking no-ops because no TRS-80 port devices are emulated. |
| `LPRINT` and `LLIST` | Level II-only | Rejected in strict Level I syntax. | Level II routes output through `IHost.Print`; Windows hosts may show a print dialog. |
| `CLOAD` and `CSAVE` | Level I-compatible command policy | File-backed aliases are available through the shared command implementation. | Cassette transport is not emulated. |
| `PRINT AT` | Level II-only | Rejected in strict Level I syntax. | Level II maps positions onto the 64-by-16 host display with documented wrapping. |
| `SYSTEM` | Shared lifecycle | Accepted where scanner/parser support exists. | Terminates the current BASIC run through the machine lifecycle; no ROM-level emulation is implied. |

## Diagnostics And State

| Situation | Level I result | Notes |
| --- | --- | --- |
| Level I-only syntax failure | `WHAT?` | Classic profile vocabulary. |
| Level II-only syntax in Level I | `WHAT?` | Parser rejects it at the profile boundary. |
| Numeric overflow | Profile runtime overflow policy | `%`, implicit integer, single, and double limits are enforced by owning runtime code. |
| Type mismatch | Shared type-mismatch policy where the operation exists | Level II uses `?TM ERROR`; Level I vocabulary remains profile-specific where required. |
| Undefined branch target | `?UL ERROR` | Source marker identifies the target. |
| Out-of-range `ON` selector | `?FC ERROR` in program mode | Immediate selectors follow the documented fall-through policy. |
| Immediate runtime divide-by-zero | `?/0 ERROR` | No extra `READY`; `ERL` becomes `65535`. |
| Numbered program syntax error | `WHAT?` in Level I | `ERL` records the failing line. |
| `NEW` or `LOAD` | Clears `ERL` and active error-handler state | Program replacement lifecycle is explicit and tested. |

## Intentional Limitations

- Level I is a strict profile, not a ROM emulator. Unsupported Level II-only syntax is rejected rather than silently approximated.
- ROM/RAM layout, cassette transport, TRS-80 I/O ports, printer hardware, and exact hardware random sequences are outside the emulator scope.
- `CLOAD` and `CSAVE` use file-backed aliases rather than cassette transport.
- `OUT` and `WAIT` have no emulated port devices in the Level I-compatible host.
- The modern cursor-based `EDIT` experience differs from the original TRS-80 keyboard workflow.
- Windows console, dialog, printer, and font behavior is host-specific and should not be treated as language semantics.

## Evidence And Next Work

Primary focused evidence:

- `Trs80.Level1Basic.Interpreter.Test/Level1SyntaxGatingTest.cs`
- `Trs80.Level1Basic.Interpreter.Test/ExpressionTest.cs`
- `Trs80.Level1Basic.Interpreter.Test/ErrorTest.cs`
- `Trs80.Level1Basic.Interpreter.Test/FlowControlTest.cs`
- `Trs80.Level1Basic.Interpreter.Test/NativeFunctionTest.cs`
- `Trs80.Level1Basic.Interpreter.Test/PrintTest.cs`
- `Trs80.Level1Basic.Environment.Test/BasicLevelConfigurationTest.cs`
- `Trs80.Level1Basic.VirtualMachine/Interpreter/Environment.cs`
- `Trs80.Level1Basic.VirtualMachine/Interpreter/Interpreter.cs`
- `Trs80.Level1Basic.VirtualMachine/Exceptions/ExceptionHandler.cs`

The next Level I audit should focus on strict-manual differences not already covered by the shared baseline: exact variable-name limits, Level I-specific numeric overflow vocabulary, any Level I-only command abbreviations, and representative Level I compatibility programs. Each new policy should receive a focused regression and a matrix update.

## Maintenance Rules

- Mark a row `Implemented` only when runtime behavior and focused tests exist.
- Mark a row `Intentionally limited` when the host policy is explicit and tested.
- Keep Level I and Level II differences visible; do not infer Level I support from Level II implementation alone.
- Update this matrix whenever a profile boundary, host policy, or diagnostic contract changes.
- Run the full interpreter project after shared parser, environment, interpreter, exception, host, or runtime changes.
