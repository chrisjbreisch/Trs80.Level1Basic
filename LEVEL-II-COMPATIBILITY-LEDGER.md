# TRS-80 Level II BASIC Compatibility Ledger

Last reviewed: 2026-08-22

This document is the clause-level compatibility ledger for the TRS-80 Model I Level II BASIC specification in [TRS-80-Model-I-Level-II-BASIC-Standard.md](TRS-80-Model-I-Level-II-BASIC-Standard.md). It records the current implementation and test evidence; it is not a claim of complete historical or hardware conformance.

The ledger follows the first contiguous form of the standard: clauses 1 through 19, Annex A, and Annex B. The standard file contains appended legacy draft material after its first `Document Status` section. That material is not treated as additional normative clauses here.

## Status

✅ Verified · 🚧 Partial · ⬜ Not evidenced · ⚠️ Intentional limit

## How to Read This

- A parent clause is ✅ only when the stated parent requirement is supported by the child evidence beneath it. A ✅ child does not make an incomplete parent ✅.
- 🚧 means that the common behavior is implemented and tested, but a boundary, syntax form, lifecycle rule, or policy remains open.
- ⬜ means that no reliable implementation/test pair has been identified for the stated clause.
- ⚠️ means that the implementation deliberately uses a host approximation, no-op, or unavailable hardware policy.
- Notes name the controlling implementation path and focused test anchor. Broad feature labels without a behavior or test anchor are not treated as evidence.

## Sections 1-5

| Clause | Status | Evidence and boundary |
| --- | --- | --- |
| 1 Scope | 🚧 | The scanner, parser, interpreter, native-function registry, host APIs, and Level II compatibility corpus implement the language surface. No single complete conformance fixture covers the entire scope. |
| 1.1 | 🚧 | Syntax and semantics are exercised across `Scanner.cs`, `Parser.cs`, `Interpreter.cs`, `NativeFunctions.cs`, and `Level2CompatibilityTest.cs`; the complete standard is not executable as one test. |
| 1.2 | ✅ | The standard and feature matrix identify the published references and formal clause organization. This is documentary provenance, not runtime behavior. |
| 1.3 | 🚧 | `IHost.cs`, `Host.cs`, `Trs80Api.cs`, and the hardware compatibility programs preserve language-level behavior while documenting host approximations; undocumented ROM fidelity is intentionally limited. |
| 1.4 | ✅ | Normative terms are defined in the standard document. No executable test applies. |
| 2 Normative references | ✅ | The standard lists the language references, ISO 7185 as a style model, and the repository implementation as a validation target. |
| 2.1 | ✅ | References are explicitly named in the standard. This is documentary evidence. |
| 2.2 | ✅ | The standard and this ledger explicitly distinguish repository compatibility evidence from authority for undocumented ROM details. |
| 3 Terms and definitions | 🚧 | Program, statement, command, variable, numeric types, string, error, and immediate-mode concepts have corresponding runtime paths; definitions are not independently conformance-tested. |
| 3.1 Program | ✅ | Numbered-line storage and ascending execution are implemented in `Interpreter.cs`; `RunTest.cs` and `FlowControlTest.cs` exercise transfers and line order. |
| 3.2 Statement | ✅ | `Parser.Statement()` and interpreter dispatch support program and immediate statements; `ParserTest.cs`, `StatementListTest.cs`, and `LonghandSmokeTest.cs` provide anchors. |
| 3.3 Command | 🚧 | Command handlers under `Trs80.Level1Basic.Command/Commands` and `InputCommand` execute unnumbered operations; editor and dialog forms have separate host policies. |
| 3.4 Variable | ✅ | `Environment.cs` stores named scalar and array values with resolved types; `ExpressionTest.cs` and typed compatibility programs exercise the model. |
| 3.5 Integer | ✅ | Integer casting and overflow paths in `Environment.cs` enforce the 16-bit range; `ExpressionTest.cs` and `ErrorTest.cs` cover assignment boundaries. |
| 3.6 Single-precision real value | 🚧 | Single handling is implemented in `Environment.cs` and `NativeFunctions.cs`; `ExpressionTest.cs` and `NativeFunctionTest.cs` cover common precision behavior, not every boundary. |
| 3.7 Double-precision real value | 🚧 | Double evaluation and conversion are implemented in `Environment.cs` and `Interpreter.cs`; `ExpressionTest.cs`, `NativeFunctionTest.cs`, and compatibility programs cover common cases, not the complete promotion matrix. |
| 3.8 String | 🚧 | Dynamic string storage and the 255-character model are exercised by `ExpressionTest.cs`, `CommandTest.cs`, and `ErrorTest.cs`; heap exhaustion and every length boundary remain open. |
| 3.9 Error | ✅ | `ExceptionHandler.cs` reports runtime conditions; `ErrorTest.cs` covers core diagnostics and state. |
| 3.10 Immediate mode | ✅ | Immediate input and command dispatch are exercised by `CommandTest.cs` and `LonghandSmokeTest.cs`. |
| 4 Conformance | 🚧 | Scanner, parser, type, interpreter, host, and focused suites cover the supported surface; conformance remains limited by the partial rows below. |
| 4.1 | 🚧 | Defined syntax, types, operators, statements, functions, and diagnostics have distributed evidence in `ScannerTest.cs`, `ParserTest.cs`, `Level1SyntaxGatingTest.cs`, and focused interpreter tests; no full conformance run exists. |
| 4.2 | 🚧 | Host extensions are isolated behind `IHost.cs` and documented in the feature matrix; the extension inventory is not exhaustive. |
| 4.3 | ✅ | Parser rejection and runtime diagnostics are covered by `ParserTest.cs`, `ErrorTest.cs`, and `Level1SyntaxGatingTest.cs` for tested invalid forms. |
| 5 Syntax notation | 🚧 | The notation is documented and parser behavior follows the supported forms; the Markdown grammar is not machine-checked. |
| 5.1 | 🚧 | Optional/repeated/alternative forms are exercised by `ParserTest.cs` and command/function tests; notation coverage is not exhaustive. |
| 5.2 | 🚧 | `Parser.Statement()` and expression/declaration methods cover the abstract grammar's supported forms; hardware and historical forms retain policy gaps. |
| 5.3 | 🚧 | Native call parsing and registration in `Parser.cs` and `NativeFunctions.cs` are tested by `NativeFunctionTest.cs`; function-by-function edge coverage remains. |

## Section 6 Lexical Elements

| Clause | Status | Evidence and boundary |
| --- | --- | --- |
| 6 Lexical elements | 🚧 | Scanner and identifier behavior are broadly tested, but line-number and exhaustive character-set boundaries remain. |
| 6.1 | ✅ | `Scanner.cs` and `TokenType.cs` recognize the required letters, digits, punctuation, and whitespace, including standalone LF; `ScannerTest.Scanner_Accepts_Clause_6_1_Character_Set` and `ScannerTest.Scanner_Accepts_Clause_6_1_Newline_Characters` provide focused coverage. |
| 6.2 | ✅ | Keyword/identifier normalization and string-case preservation are exercised by `ScannerTest.cs` and string cases in `ExpressionTest.cs`. |
| 6.3 | ✅ | `Parser.GetLineNumberValue` accepts 0 through 65529 and rejects values above 65529; `ParserTest.Parser_Accepts_The_Maximum_Program_Line_Number` covers 65529, and `ParserTest.Parser_Rejects_Reserved_Program_Line_Numbers` covers 65530 and 65535. |
| 6.4 | ✅ | Colon-separated statement lists are parsed and executed by `Parser.cs`; `StatementListTest.cs` is the focused anchor. |
| 6.5 | ✅ | `REM` consumes the remainder of a source line; `ScannerTest.cs` and `ParserTest.cs` cover the behavior. |
| 6.6 | ✅ | Keyword-containing identifiers are rejected and first-two-character identity is applied in `Scanner.cs`, `Parser.cs`, and `Environment.cs`; identifier regressions are in `ExpressionTest.cs` and `Level1SyntaxGatingTest.cs`. |

## Sections 7-10

| Clause | Status | Evidence and boundary |
| --- | --- | --- |
| 7 Data types and storage model | 🚧 | `Environment.cs` and `Trs80Api.cs` implement the four value categories and storage paths; exact precision and heap boundaries remain partial. |
| 7.1 | ✅ | `Environment.VariableType` and typed compatibility programs cover integer, single, double, and string categories. |
| 7.2 | ✅ | Integer range enforcement is covered by `ExpressionTest.cs` and overflow cases in `ErrorTest.cs`. |
| 7.3 | 🚧 | Single/double representation and default numeric typing are implemented; `ExpressionTest.cs`, `NativeFunctionTest.cs`, and compatibility programs do not close every precision threshold. |
| 7.4 | 🚧 | Dynamic strings and the 0-255 length model are exercised; exact heap failure behavior is not fully evidenced. |
| 7.5 | 🚧 | Suffix/declaration/default resolution is implemented in `Environment.cs`; conflicting declarations and conversions remain open. |
| 7.6 | ✅ | Numeric and string name spaces are distinct for tested suffix forms in `Environment.cs` and `ExpressionTest.cs`. |
| 7.7 | 🚧 | Unsuffixed numeric defaults are tested in `ExpressionTest.cs` and `Level2CompatibilityTest.cs`; literal classification and formatting edges remain. |
| 7.8 | 🚧 | Assignment/cast paths permit supported type changes; invalid conversion policy is incomplete. |
| 7.9 | ✅ | String length/content preservation through assignment and slicing is anchored by `NativeFunctionTest.cs` and `ExpressionTest.cs`. |
| 8 Constants, variables, and declarations | 🚧 | Declaration parsing and type assignment are implemented, with open redeclaration and invalid-conversion cases. |
| 8.1 | ✅ | All four `DEF*` forms are parsed and applied; `ExpressionTest.cs` and declaration programs in `Level2CompatibilityTest.cs` cover them. |
| 8.2 | ✅ | `Environment.SetVariableType` applies defaults by initial letter; declaration tests cover the behavior. |
| 8.3 | ✅ | Range parsing for forms such as `A-C` is tested by declaration cases and compatibility programs. |
| 8.4 | ⬜ | Declaration mutation exists in `Environment.cs`, but no focused conflicting-redeclaration test establishes value discard/conversion behavior. |
| 8.5 | ✅ | `DEFSTR` implicit string typing is covered by `ExpressionTest.cs` and `Level2CompatibilityTest.cs`. |
| 8.6 | ✅ | Signed decimal literal parsing is covered by `ScannerTest.cs` and `ExpressionTest.cs`. |
| 8.7 | 🚧 | Fixed/scientific real parsing and formatting are covered by `ExpressionTest.cs` and `PrintTest.cs`; exact untyped literal rules remain. |
| 8.8 | 🚧 | String tokenization and malformed-quote recovery are covered by `ScannerTest.cs` and `PrintTest.cs`; escape/quoting policy is not defined. |
| 8.9 | ✅ | `%`, `!`, and `#` suffix resolution is covered by typed declaration and compatibility tests. |
| 8.10 | 🚧 | Exponent parsing is implemented and malformed forms diagnose through scanner/parser paths; the complete exponent/error matrix remains open. |
| 9 Expressions | 🚧 | Expression visitors and precedence methods cover the supported arithmetic, relation, logic, and string operations; mixed-type boundaries remain. |
| 9.1 | ✅ | `ExpressionTest.cs`, `LogicalTest.cs`, and `PrintTest.cs` cover the supported expression families. |
| 9.2 | 🚧 | Unary/binary arithmetic and exponentiation are implemented; mixed-type overflow cases remain. |
| 9.3 | ✅ | Relational operators and true `-1`/false `0` are covered by `ExpressionTest.cs`, `LogicalTest.cs`, and compatibility programs. |
| 9.4 | 🚧 | All listed logical operators and truth values are covered by `LogicalTest.cs`; bitwise edge semantics need a complete matrix. |
| 9.5 | ✅ | String concatenation is implemented in `Interpreter.cs` and covered by string assignment regressions in `ExpressionTest.cs`. |
| 9.6 | ✅ | Parser precedence methods `Imp`, `Eqv`, `Xor`, `Or`, `And`, `Comparison`, `Term`, `Factor`, and `Power` are exercised by expression/print tests. |
| 9.7 | 🚧 | Numeric promotion exists in `Interpreter.cs`/`Environment.cs`; the complete int/single/double promotion matrix is not tested. |
| 9.8 | 🚧 | String type checks exist in `Interpreter.cs`; invalid mixed string/numeric expressions need broader coverage. |
| 10 Assignment and program execution | 🚧 | Assignment, source order, line order, and immediate execution are implemented; promotion and invalid conversion boundaries remain. |
| 10.1 | ✅ | `[LET]` assignment and string assignment parse through `Parser.cs`; `ExpressionTest.cs` and `ParserTest.cs` anchor the forms. |
| 10.2 | ✅ | `Interpreter.Assign` and `Environment.SetVariable` store computed values for common targets. |
| 10.3 | 🚧 | `Environment.CastValue` converts numeric targets; promotion and overflow boundaries remain. |
| 10.4 | 🚧 | String assignment works, but the exact overlength diagnostic path needs a focused boundary test. |
| 10.5 | 🚧 | Truncation, rounding, double preservation, and conversions are covered across `ExpressionTest.cs` and `NativeFunctionTest.cs`; invalid conversion/midpoint policy remains. |
| 10.6 | ✅ | The execution loop and transfer visitors in `Interpreter.cs` are covered by `RunTest.cs` and `FlowControlTest.cs`. |
| 10.7 | ✅ | Immediate-mode command handling is covered by `CommandTest.cs` and `LonghandSmokeTest.cs`. |
| 10.8 | ✅ | Left-to-right colon statement execution is covered by `StatementListTest.cs`. |
| 10.9 | ⬜ | Current-line reference handling is present in parser/interpreter paths, but no dedicated `.` test has been identified. |

## Section 11 Statements and Commands

| Clause | Status | Evidence and boundary |
| --- | --- | --- |
| 11 Statements and commands | 🚧 | Parser dispatch is broad and focused suites cover the main families; host and historical edge policies remain. |
| 11.1 | ✅ | Invalid keyword/operand structures reach parser diagnostics; `ParserTest.cs`, `ErrorTest.cs`, and `Level1SyntaxGatingTest.cs` cover tested forms. |
| 11.2 Program control | ✅ | Control visitors and continuation state in `Interpreter.cs` are covered by `FlowControlTest.cs`, `RunTest.cs`, and compatibility programs. |
| 11.2.1 | ✅ | GOTO line lookup and transfer are exercised by `FlowControlTest.cs` and compatibility programs. |
| 11.2.2 | ✅ | GOSUB return-address state is exercised by `FlowControlTest.cs` and the compatibility subroutine program. |
| 11.2.3 | ✅ | RETURN stack behavior is covered by `FlowControlTest.cs` and `RecursionTest.cs`. |
| 11.2.4 | ✅ | ON GOTO selector behavior is covered by `FlowControlTest.cs` and the computed-branch compatibility program. |
| 11.2.5 | ✅ | ON GOSUB selector and return behavior are covered by `FlowControlTest.cs` and the computed-subroutine program. |
| 11.2.6 | ✅ | IF/THEN/ELSE and inline branch parsing/execution are covered by `FlowControlTest.cs`, `LogicalTest.cs`, and compatibility programs. |
| 11.2.7 | ✅ | FOR initialization and loop state are covered by `FlowControlTest.cs` and loop compatibility programs. |
| 11.2.8 | ✅ | NEXT increment/termination, including descending STEP, is covered by `FlowControlTest.cs`. |
| 11.2.9 | ✅ | STOP output and lifecycle are covered by `FlowControlTest.cs` and the STOP/CONT compatibility program. |
| 11.2.10 | ✅ | END termination behavior is covered by `RunTest.cs` and compatibility programs. |
| 11.2.11 | ✅ | CONT continuation and invalid-state errors are covered by `FlowControlTest.cs` and `ErrorTest.cs`. |
| 11.3 Program-management commands | 🚧 | Core command handlers and file/range tests exist; editor, dialog, abbreviation, and host boundaries remain. |
| 11.3.1 | ✅ | NEW program/state reset is covered by `RunTest.cs`, `CommandTest.cs`, and the NEW compatibility program. |
| 11.3.2 | ✅ | CLEAR variable/array reset and optional string capacity are covered by `CommandTest.cs`, `ExpressionTest.cs`, and compatibility programs. |
| 11.3.3 | ✅ | RUN at first/explicit line is covered by `RunTest.cs` and compatibility programs. |
| 11.3.4 | 🚧 | LIST range output is covered by `LineListTest.cs`; pagination and editor boundaries remain. |
| 11.3.5 | ⚠️ | LLIST range dispatch and `IHost.Print` are covered by `LineListTest.cs`/`Trs80Test.cs`; physical printer fidelity is outside scope. |
| 11.3.6 | ✅ | LOAD replacement and file fixtures are covered by `FileTest.cs` and round-trip compatibility programs. |
| 11.3.7 | ✅ | SAVE file output is covered by `FileTest.cs` and round-trip compatibility programs. |
| 11.3.8 | ✅ | MERGE line combination is covered by `FileTest.cs` and the MERGE compatibility program. |
| 11.3.9 | ✅ | Single-line DELETE is covered by `CommandTest.cs` and compatibility programs. |
| 11.3.10 | ✅ | Closed/open/reversed DELETE ranges are covered by `CommandTest.cs` and `LineListTest.cs`; remaining abbreviations are separate policy work. |
| 11.4 Data statements | ✅ | DATA/READ/RESTORE implementation in `DataElements.cs` and `Interpreter.cs` is covered by `DataTest.cs` and data compatibility programs. |
| 11.4.1 | ✅ | DATA literals retain source order in `DataTest.cs`. |
| 11.4.2 | ✅ | READ consumes successive values into targets in `DataTest.cs`. |
| 11.4.3 | ✅ | RESTORE beginning/line selection is covered by `DataTest.cs` and the explicit RESTORE compatibility program. |
| 11.5 Input and output statements | 🚧 | PRINT, INPUT, LPRINT, TAB, SPC, POS, and CSRLIN have focused coverage; formatting boundaries remain. |
| 11.5.1 | ✅ | PRINT expression lists are covered by `PrintTest.cs` and console compatibility programs. |
| 11.5.2 | ✅ | Scanner `?` alias and PRINT dispatch are covered by `ShorthandTest.cs`, `ScannerTest.cs`, and `PrintTest.cs`. |
| 11.5.3 | 🚧 | INPUT prompts and assignments are covered by `InputTest.cs`; malformed/end-of-input cases remain. |
| 11.5.4 | ⚠️ | LPRINT routes through `IHost.Print` and is tested through `Trs80Test.cs`; physical printer output is host-mediated. |
| 11.5.5 | ✅ | TAB behavior is covered by `NativeFunctionTest.cs` and `PrintTest.cs`. |
| 11.5.6 | ✅ | SPC behavior is covered by `NativeFunctionTest.cs` and `PrintTest.cs`. |
| 11.5.7 | 🚧 | POS is implemented in `Trs80Api.cs` and covered by cursor compatibility cases; display-edge positions need more tests. |
| 11.5.8 | 🚧 | CSRLIN is implemented and covered by cursor compatibility cases; display-edge positions need more tests. |
| 11.6 Graphics and host access | 🚧 | Graphics/memory APIs are tested through `Host.cs`, `Trs80Api.cs`, and `Trs80Test.cs`; hardware execution and address functions retain limits. |
| 11.6.1 | ✅ | SET pixel behavior, including host coordinate wrapping, is covered by `Trs80Test.cs` and the hardware compatibility program. |
| 11.6.2 | ✅ | RESET pixel behavior is covered by `Trs80Test.cs` and the hardware compatibility program. |
| 11.6.3 | ✅ | POINT pixel queries are covered by `Trs80Test.cs` and the hardware compatibility program. |
| 11.6.4 | ✅ | POKE memory writes are covered by `Trs80Test.cs` and the hardware compatibility program. |
| 11.6.5 | ✅ | PEEK memory reads are covered by `Trs80Test.cs` and the hardware compatibility program. |
| 11.6.6 | 🚧 | MEM is implemented in `Trs80Api.cs` and covered by `NativeFunctionTest.cs`; exact free-memory accounting is host-defined. |
| 11.6.7 | ⬜ | `VARPTR` appears in the native-function surface, but no reliable runtime address semantics or focused behavior test has been identified. |
| 11.6.8 | ⚠️ | `USR` has no machine-language execution evidence; the host interface does not emulate TRS-80 machine-code calls. |
| 11.7 Arrays | 🚧 | Array storage/index validation in `Environment.cs` is covered for one/two dimensions; higher dimensions and all redeclaration/type combinations remain. |
| 11.7.1 | ✅ | One-dimensional DIM allocation and access are covered by `ExpressionTest.cs` and typed-array compatibility programs. |
| 11.7.2 | ✅ | Two-dimensional DIM allocation and access are covered by `ExpressionTest.cs` and compatibility programs. |
| 11.7.3 | ✅ | Bounds validation is covered by `ExpressionTest.cs` and `ErrorTest.cs`. |
| 11.8 Error trapping | ✅ | `ExceptionHandler.cs` and interpreter error state are covered by `ErrorTest.cs` and trapped-error compatibility programs. |
| 11.8.1 | ✅ | ON ERROR handler entry is covered by `ErrorTest.cs`. |
| 11.8.2 | ✅ | ON ERROR GOTO 0 reset is covered by `ErrorTest.cs`. |
| 11.8.3 | ✅ | RESUME variants and continuation points are covered by `ErrorTest.cs` and compatibility cases. |

## Sections 12-16

| Clause | Status | Evidence and boundary |
| --- | --- | --- |
| 12 Built-in functions | 🚧 | `NativeFunctions.cs` and `Trs80Api.cs` register and execute the main function families; domain, rounding, malformed-conversion, and historical-function edges remain. |
| 12.1 String functions | 🚧 | `NativeFunctionTest.cs` and string compatibility programs cover ASC, CHR$, slicing, LEN, STR$, VAL, STRING$, INSTR, case, and trim helpers; conversion/error boundaries remain. |
| 12.2 Numeric functions | 🚧 | `NativeFunctionTest.cs` and `ExpressionTest.cs` cover the listed math/conversion functions; domain and exact rounding policies remain. |
| 12.3 Random number functions | 🚧 | RND controls and deterministic behavior are covered by `NativeFunctionTest.cs` and compatibility programs; remaining manual control boundaries are open. |
| 12.4 Binary conversion functions | 🚧 | CVI/CVS/CVD and MKI$/MKS$/MKD$ valid little-endian paths are covered by `NativeFunctionTest.cs` and binary compatibility programs; invalid lengths/ranges need tests. |
| 12.5 Keyboard/input functions | 🚧 | INKEY$ and INPUT$ are covered by `NativeFunctionTest.cs` and `InputTest.cs`; extended-key encoding remains unresolved. |
| 13 Error handling | 🚧 | `ExceptionHandler.cs`, interpreter diagnostics, and `ErrorTest.cs` cover the core model; the complete listed error-code mapping remains open. |
| 13.1 | 🚧 | BASIC-style code/line diagnostics are covered by `ErrorTest.cs`; not every standard error has source-location evidence. |
| 13.2 | 🚧 | Core `SN`, `NF`, `RG`, `OD`, `FC`, `OV`, `TM`, `UL`, `BS`, and `/0` paths are tested; every listed code is not mapped one-to-one. |
| 13.3 | ✅ | ERR/ERL storage and handler visibility are covered by `ErrorTest.cs` and error compatibility programs. |
| 13.4 | ✅ | Active ON ERROR handlers redirect execution in `ExceptionHandler.cs`; `ErrorTest.cs` is the focused anchor. |
| 13.5 | ✅ | RESUME, RESUME NEXT, and RESUME line behavior is covered by `ErrorTest.cs`. |
| 13.6 | ✅ | Invalid CONT state produces an error; `FlowControlTest.cs` and `ErrorTest.cs` cover it. |
| 13.7 | ✅ | DATA exhaustion and READ type mismatches are covered by `DataTest.cs` and `ErrorTest.cs`. |
| 14 Input and output model | 🚧 | Screen, PRINT, INPUT, printer, and cursor APIs are implemented and tested; display-edge formatting remains. |
| 14.1 | 🚧 | 64-by-16 geometry is represented in `Host.cs`/`Trs80Api.cs` and exercised by `Trs80Test.cs`; all display-edge positions need a dedicated inventory. |
| 14.2 | 🚧 | PRINT semicolon/comma formatting is covered by `PrintTest.cs`; exact line-width boundaries remain. |
| 14.3 | ✅ | `?` equivalence is covered by `ShorthandTest.cs` and `PrintTest.cs`. |
| 14.4 | 🚧 | INPUT prompt/value assignment is covered by `InputTest.cs`; malformed and end-of-input behavior remains partial. |
| 14.5 | ⚠️ | LPRINT uses the host printer abstraction and is tested through `Trs80Test.cs`; physical output is intentionally not emulated. |
| 15 Memory model and host interface | 🚧 | Program, variable, string, keyboard, display, memory, printer, and graphics abstractions are present; exact heap/ROM/cassette fidelity is limited. |
| 15.1 | ✅ | Numbered program storage is exercised by `RunTest.cs`, `LineListTest.cs`, and `FileTest.cs`. |
| 15.2 | 🚧 | Scalar/array/string storage is implemented in `Environment.cs` and tested by `ExpressionTest.cs`; exact internal storage layout is abstracted. |
| 15.3 | 🚧 | CLEAR/string capacity interaction is covered by `CommandTest.cs` and `ExpressionTest.cs`; exhaustive heap accounting is not evidenced. |
| 15.4 | ⚠️ | `IHost.cs`, `Host.cs`, and `Trs80Api.cs` provide host services; ROM and cassette transport are intentionally not emulated. |
| 16 Implementation-defined features | 🚧 | Formatting, random seed, diagnostics, heap, and hardware policies are documented in the feature matrix and distributed tests; no exhaustive policy registry links every choice to a test. |
| 16.1 | 🚧 | The listed implementation-defined choices are documented across `LEVEL-II-FEATURE-MATRIX.md`, host/API code, and focused tests; registry coverage is incomplete. |
| 16.2 | 🚧 | Host abstraction/no-op policies preserve tested language behavior, but the extension and policy inventory is not exhaustive. |

## Sections 17-19

| Clause | Status | Evidence and boundary |
| --- | --- | --- |
| 17 Reserved words | 🚧 | Keyword mapping in `TokenType.cs`/`Scanner.cs` and parser dispatch have inventory tests; reserved-but-unimplemented runtime policies remain. |
| 17.1 | 🚧 | `ScannerTest.cs`, `ParserTest.cs`, `ShorthandTest.cs`, and `Level1SyntaxGatingTest.cs` cover VM-backed keywords and aliases; `TRON`/`TROFF` require explicit policy. |
| 18 Compatibility notes | 🚧 | Level I/II differences are documented in the standard and feature matrices, with syntax gates and compatibility programs; historical comparison remains editorial. |
| 18.1 | ✅ | Type suffixes, declarations, arrays, enhanced functions, error trapping, and control-flow differences are explicitly listed and exercised by Level I/II tests. |
| 18.2 | 🚧 | The standard preserves the compatibility focus and the corpus provides executable examples; complete historical behavior comparison remains an audit. |
| 19 Summary of normative requirements | 🚧 | The eight summary requirements are represented by the underlying clause rows; the summary cannot exceed the least-supported clause. |
| 19.1 | 🚧 | Full Interpreter tests and focused suites cover the summary categories, while host, conversion, error-matrix, and policy gaps remain. |
| Annex A Minimal Example Program | 🚧 | The parser/interpreter/environment support its constructs and equivalent programs exist in `Level2CompatibilityTest.cs`; the exact printed sample is not a named regression. |
| Annex B Keyword Index | 🚧 | `TokenType.cs` and `Scanner.cs` provide the comparison source and `ScannerTest.cs` provides inventory evidence; unsupported/reserved runtime policies remain. |

## Highest-Value Follow-Up Evidence

1. Add the exact Annex A program as a named executable regression.
2. Split Clause 13.2 into one row per listed error code with expected diagnostic and source-location evidence.
3. Close typed promotion, invalid conversion, literal precision, and assignment-overflow cases across Clauses 7-10.
4. Add boundary tests for line numbers, `PRINT AT`, cursor positions, graphics wrapping, and memory addresses.
5. Decide explicit policies for `VARPTR`, `USR`, `TRON`, and `TROFF`.
6. Add lifecycle tests for DATA, handlers, arrays, and continuation state after `CLEAR`, `NEW`, `LOAD`, `MERGE`, and `CONT`.
