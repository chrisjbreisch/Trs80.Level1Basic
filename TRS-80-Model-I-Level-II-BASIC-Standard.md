# TRS-80 Model I Level II BASIC Standard

## 1. Scope

1.1 This document specifies the syntax and semantics of TRS-80 Model I Level II BASIC.

1.2 This specification is derived from the published Level II BASIC language reference and is organized in a clause structure modeled on ISO 7185 Pascal.

1.3 This document defines the required behavior of a conforming Level II BASIC implementation. It does not require exact reproduction of undocumented ROM behavior, hardware-specific quirks, or machine-dependent extensions, but it does require that the language-level behavior observable to a BASIC program be consistent with this specification.

1.4 The keywords "shall", "shall not", "required", and "may" are normative.

---

## 2. Normative References

2.1 The following documents are normative for this specification:

- TRS-80 Model I Level II BASIC Language Reference, Joe Ganley;
- Radio Shack BASIC Computer Language, David A. Lien;
- ISO 7185: Pascal, as a model of standard clause organization and formal style;
- the repository implementation in this project, used as a compatibility validation target.

2.2 The repository implementation shall not be treated as the normative authority for undocumented ROM details.

---

## 3. Terms and Definitions

### 3.1 Program
A program is an ordered sequence of numbered source lines. Execution proceeds in ascending line-number order unless control flow explicitly alters the order.

### 3.2 Statement
A statement is a syntactic unit that performs an action or defines program behavior. A statement may appear in a program or in immediate mode.

### 3.3 Command
A command is a statement or directive executed immediately when entered without a line number or a directive that affects the execution environment.

### 3.4 Variable
A variable is a named storage location containing a value of a defined type.

### 3.5 Integer
An integer is a 16-bit signed value in the range -32768 through 32767 inclusive.

### 3.6 Single-Precision Real Value
A single-precision real value is a numeric value represented in 32-bit floating-point form.

### 3.7 Double-Precision Real Value
A double-precision real value is a numeric value represented in 64-bit floating-point form.

### 3.8 String
A string is a sequence of zero or more characters. The maximum string length shall be 255 characters unless a specific implementation defines a smaller limit.

### 3.9 Error
An error is a condition that prevents normal continuation of execution and requires a diagnostic report and an appropriate runtime response.

### 3.10 Immediate Mode
Immediate mode is the execution mode in which a statement or command is entered without a line number and is executed when the input line is complete.

---

## 4. Conformance

4.1 A conforming implementation shall:

1. accept the lexical forms and syntactic constructs defined in this specification;
2. implement the specified data types, operators, statements, commands, and built-in functions;
3. preserve the specified semantics of assignment, expression evaluation, control transfer, and error handling;
4. reject unsupported or malformed constructs by means of a detectable syntax or runtime error;
5. distinguish Level I-compatible constructs from Level II-only constructs where the language definition requires such distinction.

4.2 A conforming implementation may provide host-specific extensions, provided that such extensions do not alter the semantics of any construct defined by this specification.

4.3 A program is valid only if each source line is syntactically valid and semantically permitted by the implementation under this specification.

---

## 5. Syntax Notation

5.1 The following notation is normative:

- `[...]` denotes an optional syntactic element;
- `{ ... }` denotes repetition zero or more times;
- `A | B` denotes alternative forms;
- `A ...` denotes a sequence of one or more elements;
- uppercase identifiers denote reserved words;
- lowercase identifiers denote nonterminal symbols.

5.2 The following abstract grammar shall apply:

```text
program         ::= { line }
line            ::= [ line-number ] statement [ ':' statement ]
line-number     ::= digit { digit }
statement       ::= command | assignment | control-statement | data-statement | input-output-statement | declaration-statement
assignment      ::= [ LET ] variable '=' expression
command         ::= NEW | CLEAR [ size ] | RUN [ line-number ] | LIST [ line-number [ '-' line-number ] ] | LLIST [ line-number [ '-' line-number ] ] | LOAD filename | SAVE filename | MERGE filename
control-statement ::= GOTO line-number | GOSUB line-number | RETURN | IF expression THEN statement [ ELSE statement ] | FOR variable '=' expression TO expression [ STEP expression ] | NEXT [ variable ] | STOP | END | CONT | ON expression GOTO line-list | ON expression GOSUB line-list
line-list       ::= line-number { ',' line-number }
declaration-statement ::= DEFINT letter-range | DEFSNG letter-range | DEFDBL letter-range | DEFSTR letter-range
action-statement ::= PRINT expression-list | INPUT [ prompt ] variable-list | LPRINT expression-list | POKE integer ',' integer | SET '(' integer ',' integer ')' | RESET '(' integer ',' integer ')'
expression      ::= arithmetic-expression | relational-expression | logical-expression | string-expression
numeric-literal ::= [ sign ] digit { digit } [ '.' digit { digit } ] [ exponent ]
string-literal  ::= '"' { character } '"'
variable        ::= identifier [ type-suffix ]
identifier      ::= letter { letter | digit }
type-suffix     ::= '%' | '!' | '#' | '$'
letter-range    ::= letter [ '-' letter ]
```

5.3 The syntax of each built-in function shall conform to the function definitions in Section 11.

---

## 6. Lexical Elements

### 6.1 Character Set
The source character set shall include:

- letters A to Z and a to z;
- digits 0 to 9;
- arithmetic, relational, and structural punctuation including `+`, `-`, `*`, `/`, `^`, `(`, `)`, `,`, `;`, `:`, `=`, `?`, `.`, and `"`;
- whitespace characters including space, tab, carriage return, and line feed.

### 6.2 Case Insensitivity
Keywords and identifiers shall be case-insensitive. String literal contents shall preserve their original character case as entered unless a specific implementation defines otherwise.

### 6.3 Line Numbers
A program line shall have a line number in the range 0 through 65529 inclusive. Line numbers 65530 through 65535 are reserved for system use.

### 6.4 Statement Separation
Multiple statements on the same source line shall be separated by the colon character `:`.

### 6.5 Remarks
A `REM` statement shall introduce a comment that continues to the end of the current source line.

### 6.6 Identifier Restrictions
An identifier shall not contain any BASIC keyword as a substring. The first two characters of a variable name are significant. Identifiers such as `AB`, `ABC`, and `ABCD` shall be treated as equivalent for variable identity purposes.

---

## 7. Data Types and Storage Model

### 7.1 Type Categories
The language shall provide four principal value categories:

- integer;
- single-precision real;
- double-precision real;
- string.

### 7.2 Integer Representation
Integer values shall be stored in 16-bit two's-complement form and shall have the range -32768 through 32767 inclusive.

### 7.3 Real Representation
Single-precision and double-precision values shall be represented using their respective floating-point formats. An unsuffixed numeric variable shall default to single precision unless a declaration or suffix indicates otherwise.

### 7.4 String Representation
A string value shall be a sequence of characters stored dynamically in memory. The string length shall be represented in the variable record and shall be between 0 and 255 inclusive.

### 7.5 Type Determination
The type of a variable shall be determined by the first applicable rule in the following order:

1. explicit numeric suffix `%`, `!`, or `#`;
2. explicit string suffix `$`;
3. explicit type declaration (`DEFINT`, `DEFSNG`, `DEFDBL`, `DEFSTR`);
4. default dynamic typing when no declaration applies.

### 7.6 Name-Space Separation
In the absence of a declaration, the numeric and string name spaces shall be distinct. `A%`, `A!`, `A#`, and `A$` may refer to independent values.

### 7.7 Default Numeric Type
Unless otherwise declared, an unsuffixed numeric variable shall be treated as single precision.

### 7.8 Dynamic Typing
An implementation may permit a variable to change type during execution as a consequence of assignment and evaluation rules, provided that the resulting runtime behavior remains consistent with the language definition.

### 7.9 String Storage
String objects shall be stored in a heap or equivalent dynamic storage allocation model. The implementation shall preserve the string length and character content associated with each string variable.

---

## 8. Constants, Variables, and Declarations

### 8.1 Declaration Forms
The following declarations are part of the standard language:

- `DEFINT V [ - V ]*`
- `DEFSNG V [ - V ]*`
- `DEFDBL V [ - V ]*`
- `DEFSTR V [ - V ]*`

where `V` denotes a variable name or a variable range.

### 8.2 Declaration Semantics
A `DEF*` declaration shall assign the default type for all unsuffixed variables whose initial letter matches the declared range.

### 8.3 Range Declaration
A range declaration shall be written in the form `A-Z`, denoting all variables from `A` through `Z` inclusive.

### 8.4 Redeclaration
A redeclaration of a variable shall replace the prior default assignment for that variable. If the new type differs from the previous type, the prior value shall be discarded or converted according to the implementation's assignment rules.

### 8.5 Implicit String Declaration
If a variable is declared by a `DEFSTR` statement, the `$` suffix shall be treated as implicit for that variable.

### 8.6 Integer Literal
An integer literal shall consist of a decimal digit sequence optionally preceded by a sign.

### 8.7 Real Literal
A real literal may be represented in fixed-point or scientific notation. The precise interpretation of an untyped real literal shall be implementation-defined within the Level II model, but it shall remain consistent with the conversion and assignment rules of this specification.

### 8.8 String Literal
A string literal shall be enclosed in double quotation marks. A string literal shall preserve all characters within the delimiters exactly as written, except where the implementation defines an escape or quoting convention.

### 8.9 Numeric Suffixes
The suffixes `%`, `!`, and `#` shall denote integer, single-precision, and double-precision variables respectively.

### 8.10 Exponent Notation
A numeric literal may use an exponent form consistent with the Level II reference. An implementation shall accept exponent notation where the language permits it and shall reject malformed numeric forms as syntax errors.

---

## 9. Expressions

### 9.1 Expression Classes
An expression shall be any arithmetic, relational, logical, or string expression permitted by the language.

### 9.2 Arithmetic Operators
The arithmetic operators shall include:

- unary `+` and unary `-`;
- binary `+`, `-`, `*`, and `/`;
- exponentiation `^`.

### 9.3 Relational Operators
The relational operators shall include:

- `<`
- `<=`
- `=`
- `>=`
- `>`
- `<>`

The result of a relational expression shall be the integer value 0 for false and -1 for true.

### 9.4 Logical Operators
The logical operators shall include:

- `NOT`
- `AND`
- `OR`
- `XOR`
- `EQV`
- `IMP`

The result of a logical expression shall be represented as a numeric truth value, with false represented by 0 and true by -1.

### 9.5 String Concatenation
The `+` operator shall concatenate string operands to produce a single string value.

### 9.6 Operator Precedence
Operator precedence shall be, from highest to lowest:

1. unary `+`, unary `-`, and `NOT`;
2. exponentiation `^`;
3. multiplication, division, and `AND`;
4. addition, subtraction, and `OR`;
5. relational operators.

### 9.7 Mixed-Type Evaluation
If an expression contains mixed numeric types, the implementation shall convert the operands to a common numeric representation consistent with the language definition before evaluation.

### 9.8 String Evaluation
A string expression shall consist of string operands and string-concatenation operations unless the operation is explicitly a string function call or a string assignment.

---

## 10. Assignment and Program Execution

### 10.1 Assignment Syntax
The assignment form shall be:

- `[LET] variable = expression`
- `variable = string-expression`

### 10.2 Assignment Semantics
The value computed on the right side of an assignment shall be stored in the variable named on the left side. An assignment shall succeed only if the value is valid for the target variable type.

### 10.3 Numeric Assignment
When a numeric value is assigned to a numeric target, the implementation shall convert the source value to the target type when required by the declaration or suffix rules.

### 10.4 String Assignment
When a string value is assigned to a string target, the implementation shall store the string in the target variable. An assignment that exceeds the permitted string length shall produce a runtime error.

### 10.5 Numeric Casting Rules
When a value of one numeric type is assigned to a variable of another numeric type:

- an integer target shall receive the truncated integer value of the source;
- a single-precision target shall receive the rounded single-precision value of the source;
- a double-precision target shall receive the double-precision value of the source;
- a string-to-numeric conversion shall be permitted only when the value is valid under the conversion rules of the language.

### 10.6 Execution Order
Program execution shall proceed in ascending line-number order unless a statement or command alters the order by means of a control-transfer construct such as `GOTO`, `GOSUB`, `RETURN`, or an equivalent mechanism.

### 10.7 Immediate Execution
A command or statement entered without a line number shall be executed immediately in immediate mode.

### 10.8 Multiple Statements Per Line
A source line may contain more than one statement. Statements shall be executed left to right in the order in which they appear on the line.

### 10.9 Current Line Reference
The character `.` shall denote the current program line in contexts where a line number is required.

---

## 11. Statements and Commands

### 11.1 General Rule
A statement or command shall be accepted only if its keyword and operand structure conform to the syntax defined in this section.

### 11.2 Program-Control Statements

#### 11.2.1 `GOTO line`
The `GOTO` statement shall transfer control to the first statement on the specified line number.

#### 11.2.2 `GOSUB line`
The `GOSUB` statement shall save the return address and transfer control to the first statement on the specified line number.

#### 11.2.3 `RETURN`
The `RETURN` statement shall resume execution at the first statement following the most recently executed `GOSUB`.

#### 11.2.4 `ON expression GOTO line-list`
The `ON ... GOTO` statement shall select a target line from the list using the value of the expression as the selection index.

#### 11.2.5 `ON expression GOSUB line-list`
The `ON ... GOSUB` statement shall select a subroutine target from the list using the value of the expression as the selection index.

#### 11.2.6 `IF expression THEN statement [ELSE statement]`
The `IF` statement shall conditionally execute the statement following `THEN` or `ELSE` according to the truth value of the expression.

#### 11.2.7 `FOR variable = start TO end [STEP increment]`
The `FOR` statement shall initialize the loop variable and establish the loop-control state.

#### 11.2.8 `NEXT [variable ...]`
The `NEXT` statement shall increment the loop variable and continue the loop when the termination condition remains unsatisfied.

#### 11.2.9 `STOP`
The `STOP` statement shall halt execution and report the program stop state.

#### 11.2.10 `END`
The `END` statement shall terminate execution without producing the normal stop diagnostic.

#### 11.2.11 `CONT`
The `CONT` statement shall resume execution after a valid stop or break condition. An invalid `CONT` shall produce a runtime error.

### 11.3 Program-Management Commands

#### 11.3.1 `NEW`
The `NEW` command shall erase all program text and variables from memory.

#### 11.3.2 `CLEAR [size]`
The `CLEAR` command shall clear variables and may reset the string heap size when a size argument is supplied.

#### 11.3.3 `RUN [line]`
The `RUN` command shall begin execution at the first line of the program or at the specified line number.

#### 11.3.4 `LIST [start] [- end]`
The `LIST` command shall list a selected range of program lines to the display.

#### 11.3.5 `LLIST [start] [- end]`
The `LLIST` command shall list a selected range of program lines to the printer.

#### 11.3.6 `LOAD filename`
The `LOAD` command shall replace the current program and state with the contents of the specified file.

#### 11.3.7 `SAVE filename`
The `SAVE` command shall write the current program to the specified file.

#### 11.3.8 `MERGE filename`
The `MERGE` command shall merge the contents of a saved file into the current program memory.

#### 11.3.9 `DELETE line`
The `DELETE` statement shall remove the specified line number from the program.

#### 11.3.10 `DELETE start-end`
The `DELETE` statement shall remove the specified inclusive line range from the program.

### 11.4 Data Statements

#### 11.4.1 `DATA value [, value ...]`
The `DATA` statement shall store a sequence of literal values in the program data area.

#### 11.4.2 `READ variable [, variable ...]`
The `READ` statement shall read the next available values from the stored `DATA` list and assign them in order to the operand variables.

#### 11.4.3 `RESTORE [line]`
The `RESTORE` statement shall reset the data pointer to the beginning of the data list or to the first `DATA` item at or after the specified line number.

### 11.5 Input and Output Statements

#### 11.5.1 `PRINT expression-list`
The `PRINT` statement shall output the values in the expression list in the order specified.

#### 11.5.2 `? expression-list`
The `?` token shall be equivalent to `PRINT`.

#### 11.5.3 `INPUT [prompt;] variable [, variable ...]`
The `INPUT` statement shall display an optional prompt and assign values read from the keyboard to the specified variables.

#### 11.5.4 `LPRINT expression-list`
The `LPRINT` statement shall send output to the printer or equivalent host output channel.

#### 11.5.5 `TAB(n)`
The `TAB` function shall move the print cursor to the designated column position on the current line.

#### 11.5.6 `SPC(n)`
The `SPC` function shall generate a string containing the specified number of blanks.

#### 11.5.7 `POS(x)`
The `POS` function shall return the current horizontal cursor position.

#### 11.5.8 `CSRLIN`
The `CSRLIN` function shall return the current row position of the cursor.

### 11.6 Graphics and Host-Access Statements

#### 11.6.1 `SET (x, y)`
The `SET` statement shall turn on the pixel at the specified coordinates.

#### 11.6.2 `RESET (x, y)`
The `RESET` statement shall turn off the pixel at the specified coordinates.

#### 11.6.3 `POINT (x, y)`
The `POINT` function shall return a nonzero value if the pixel is set and zero otherwise.

#### 11.6.4 `POKE address, value`
The `POKE` statement shall write a byte value to the specified memory address.

#### 11.6.5 `PEEK(address)`
The `PEEK` function shall return the byte value at the specified address.

#### 11.6.6 `MEM`
The `MEM` function shall return the amount of free memory in bytes.

#### 11.6.7 `VARPTR(variable)`
The `VARPTR` function shall return the memory address associated with the specified variable.

#### 11.6.8 `USR(address)`
The `USR` function shall call the designated machine-language routine through the implementation's host interface.

### 11.7 Arrays

#### 11.7.1 `DIM variable(size)`
The `DIM` statement shall allocate a one-dimensional array.

#### 11.7.2 `DIM variable(size1, size2)`
The `DIM` statement shall allocate a two-dimensional array.

#### 11.7.3 Bounds Checking
Every array access shall be checked against the declared bounds. A subscript outside the declared range shall cause a runtime error.

### 11.8 Error Trapping

#### 11.8.1 `ON ERROR GOTO line`
The `ON ERROR GOTO` statement shall enable runtime error trapping and transfer control to the specified line when an error occurs.

#### 11.8.2 `ON ERROR GOTO 0`
The `ON ERROR GOTO 0` statement shall disable runtime error trapping.

#### 11.8.3 `RESUME [NEXT | line]`
The `RESUME` statement shall resume execution after a trapped error. `RESUME` with no argument shall resume at the failing statement; `RESUME NEXT` shall resume after the failing statement; and `RESUME line` shall resume at the specified line.

---

## 12. Built-in Functions

### 12.1 String Functions
The implementation shall provide the following string functions:

- `ASC(string)`
- `CHR$(n)`
- `LEFT$(string, n)`
- `RIGHT$(string, n)`
- `MID$(string, start [, length])`
- `LEN(string)`
- `STR$(value)`
- `VAL(string)`
- `STRING$(count, char)`
- `INSTR(start, haystack, needle)`
- `UCASE$(string)`
- `LCASE$(string)`
- `LTRIM$`, `RTRIM$`, `TRIM$` when supported

### 12.2 Numeric Functions
The implementation shall provide the following numeric functions:

- `ABS(n)`
- `SQR(n)`
- `SIN(n)`
- `COS(n)`
- `TAN(n)`
- `ATN(n)`
- `EXP(n)`
- `LOG(n)`
- `INT(n)`
- `FIX(n)`
- `CINT(n)`
- `CSNG(n)`
- `CDBL(n)`
- `SGN(n)`

### 12.3 Random Number Functions

- `RND(n)` shall return a pseudo-random value dependent on the supplied argument.
- `RANDOM` shall seed the random number generator using a host-appropriate source.

### 12.4 Binary Conversion Functions
The following conversion functions shall be supported in Level II:

- `CVI(string)`
- `CVS(string)`
- `CVD(string)`
- `MKI$(value)`
- `MKS$(value)`
- `MKD$(value)`

### 12.5 Keyboard and Input Functions

- `INKEY$` shall return a buffered keystroke if available; otherwise it shall return the empty string.
- `INPUT$` shall return a string read from the input source of the requested length.

---

## 13. Error Handling

### 13.1 Error Representation
A Level II BASIC error shall be reported in the standard BASIC form and shall be associated with a numeric error code and a line location where applicable.

### 13.2 Standard Error Codes
The implementation shall recognize the following core Level II error categories, at minimum:

- `NF` NEXT without FOR
- `SN` syntax error
- `RG` RETURN without GOSUB
- `OD` out of data
- `FC` illegal function call
- `OV` numeric overflow
- `OM` out of memory
- `UL` undefined line
- `BS` subscript out of range
- `DD` redimensioned array
- `/0` division by zero
- `ID` illegal direct mode usage
- `TM` type mismatch
- `OS` out of string space
- `LS` string too long
- `ST` string formula too complex
- `CN` cannot continue
- `NR` no resume
- `RW` resume without error
- `UE` unprintable error
- `MO` missing operand
- `FD` bad file data

### 13.3 `ERR` and `ERL`
The variables `ERR` and `ERL` shall be available to the runtime environment when an error is handled. `ERR` shall contain the error code and `ERL` shall contain the line number associated with the error condition.

### 13.4 Error Trapping
When `ON ERROR GOTO` is active, the implementation shall transfer execution to the specified line instead of terminating the program immediately.

### 13.5 Resume Semantics
The `RESUME` statement shall restart execution after a trapped error. `RESUME` with no argument shall resume at the failing statement; `RESUME NEXT` shall resume after the failing statement; and `RESUME line` shall resume at the specified line.

### 13.6 Invalid `CONT`
Executing `CONT` outside a valid stop or break context shall produce a runtime error.

### 13.7 Data-Read Errors
Attempting to read beyond the available `DATA` values shall raise `OD`. Reading a nonnumeric value into a numeric variable shall produce the relevant type or syntax diagnostic defined by the implementation.

---

## 14. Input and Output Model

### 14.1 Screen Geometry
The standard display shall be treated as a 64-column by 16-row text display. Cursor positions shall be addressed in row-major order.

### 14.2 `PRINT` Semantics
`PRINT` shall output the values in the expression list in the order specified. Values separated by semicolons shall be printed without an intervening tab stop unless the implementation defines a host-specific spacing rule. Values separated by commas shall be printed to the next tab stop.

### 14.3 `?` Shorthand
The `?` token shall be equivalent to `PRINT`.

### 14.4 `INPUT`
`INPUT` shall display an optional prompt and read one or more values from the keyboard, assigning them to the named variables.

### 14.5 Printer Output
The `LPRINT` statement shall direct output to the printer or equivalent host output device.

---

## 15. Memory Model and Host Interface

### 15.1 Program Storage
The implementation shall maintain the current program in memory as numbered source lines.

### 15.2 Variable Storage
Numeric and string variables shall occupy the implementation's variable storage area. String values shall use dynamic allocation or equivalent heap storage.

### 15.3 String Heap Size
The size of the available string heap shall be determined by the implementation and may be affected by the `CLEAR` command.

### 15.4 Host Interface
The host environment may provide keyboard, display, memory, printer, and graphics services. A conforming implementation may approximate unsupported hardware devices, but shall preserve the language-level semantics of Level II BASIC.

---

## 16. Implementation-Defined Features

16.1 The following aspects are implementation-defined and shall be documented by the implementation:

- exact formatting of floating-point output;
- the source of the random number seed;
- host-specific printer and graphics behavior;
- the exact diagnostic wording for semantically equivalent error conditions;
- the default string heap size when no explicit `CLEAR` value is supplied;
- treatment of host-specific hardware interfaces that are not part of the core language.

16.2 An implementation shall not alter the semantics of the language constructs defined in this standard merely because a host device provides a different physical capability.

---

## 17. Reserved Words

17.1 The following reserved words are part of the standard Level II language surface:

`ABS`, `AND`, `ASC`, `ATN`, `AUTO`, `CDBL`, `CHR$`, `CINT`, `CLEAR`, `CLS`, `CONT`, `COS`, `CSNG`, `DATA`, `DEFDBL`, `DEFINT`, `DEFSNG`, `DEFSTR`, `DELETE`, `DIM`, `EDIT`, `ELSE`, `END`, `ERL`, `ERR`, `EXP`, `FIX`, `FOR`, `FRE`, `GOSUB`, `GOTO`, `IF`, `INKEY$`, `INPUT`, `INPUT$`, `INT`, `INSTR`, `LEFT$`, `LEN`, `LET`, `LIST`, `LLIST`, `LOG`, `LPRINT`, `MEM`, `MERGE`, `MID$`, `NEW`, `NEXT`, `NOT`, `ON`, `OR`, `OUT`, `PEEK`, `POINT`, `POKE`, `POS`, `PRINT`, `RANDOM`, `READ`, `REM`, `RESET`, `RESTORE`, `RESUME`, `RETURN`, `RIGHT$`, `RND`, `RUN`, `SAVE`, `SET`, `SGN`, `SIN`, `SQR`, `STEP`, `STOP`, `STR$`, `STRING$`, `SYSTEM`, `TAB`, `TAN`, `THEN`, `TO`, `TRON`, `TROFF`, `USR`, `VAL`, `VARPTR`.

---

## 18. Compatibility Notes

18.1 The Level II language differs from earlier Level I BASIC in the following major respects:

- explicit type suffixes `%`, `!`, and `#`;
- explicit type declarations (`DEFINT`, `DEFSNG`, `DEFDBL`, `DEFSTR`);
- arrays and `DIM` semantics;
- enhanced string functions and binary conversion routines;
- explicit error trapping with `ON ERROR` and `RESUME`;
- additional built-in functions and control-flow constructs.

18.2 The present specification preserves historical Level II language behavior while organizing it as a formal standard reference derived from the original language definition and the repository implementation.

---

## 19. Summary of Normative Requirements

19.1 A conforming Level II BASIC implementation shall support:

1. integer, single-precision, double-precision, and string data types;
2. the variable naming, declaration, and suffix rules defined in this specification;
3. arithmetic, relational, and logical expressions with the specified precedence;
4. the standard program-control and program-management statements;
5. `DATA`, `READ`, and `RESTORE` semantics;
6. the Level II built-in functions defined in this specification;
7. runtime error handling and `ERR`/`ERL` semantics;
8. documented host-dependent behavior where the physical environment differs from the abstract language model.

---

## Annex A — Minimal Example Program

```basic
10 DEFINT A
20 DIM B(10)
30 A = 5
40 FOR I = 1 TO 5
50 B(I) = A * I
60 NEXT I
70 PRINT "SUM = "; B(1) + B(2) + B(3) + B(4) + B(5)
80 END
```

---

## Annex B — Keyword Index

`ABS, AND, ASC, ATN, AUTO, CDBL, CHR$, CINT, CLEAR, CLS, CONT, COS, CSNG, DATA, DEFDBL, DEFINT, DEFSNG, DEFSTR, DELETE, DIM, EDIT, ELSE, END, ERL, ERR, EXP, FIX, FOR, FRE, GOSUB, GOTO, IF, INKEY$, INPUT, INPUT$, INT, INSTR, LEFT$, LEN, LET, LIST, LLIST, LOG, LPRINT, MEM, MERGE, MID$, NEW, NEXT, NOT, ON, OR, OUT, PEEK, POINT, POKE, POS, PRINT, RANDOM, READ, REM, RESET, RESTORE, RESUME, RETURN, RIGHT$, RND, RUN, SAVE, SET, SGN, SIN, SQR, STEP, STOP, STR$, STRING$, SYSTEM, TAB, TAN, THEN, TO, TRON, TROFF, USR, VAL, VARPTR`

---

## Document Status

This document is a draft specification for TRS-80 Model I Level II BASIC. It is intended as a formal reference for future refinement, compatibility testing, and implementation documentation as the Level II interpreter work continues.

### 12.8 Error Trapping

#### 12.8.1 `ON ERROR GOTO line`
The `ON ERROR GOTO` statement shall enable runtime error trapping and transfer control to the specified line when an error occurs.

#### 12.8.2 `ON ERROR GOTO 0`
The `ON ERROR GOTO 0` statement shall disable runtime error trapping.

#### 12.8.3 `RESUME [NEXT | line]`
The `RESUME` statement shall resume execution after a trapped error. `RESUME` with no argument shall resume at the failing statement; `RESUME NEXT` shall resume after the failing statement; and `RESUME line` shall resume at the specified line.

---

## 13. Built-in Functions

### 13.1 String Functions

The implementation shall provide the following string functions:

- `ASC(string)`
- `CHR$(n)`
- `LEFT$(string, n)`
- `RIGHT$(string, n)`
- `MID$(string, start [, length])`
- `LEN(string)`
- `STR$(value)`
- `VAL(string)`
- `STRING$(count, char)`
- `INSTR(start, haystack, needle)`
- `UCASE$(string)`
- `LCASE$(string)`
- `LTRIM$`, `RTRIM$`, `TRIM$` where supported

### 13.2 Numeric Functions

The implementation shall provide the following numeric functions:

- `ABS(n)`
- `SQR(n)`
- `SIN(n)`
- `COS(n)`
- `TAN(n)`
- `ATN(n)`
- `EXP(n)`
- `LOG(n)`
- `INT(n)`
- `FIX(n)`
- `CINT(n)`
- `CSNG(n)`
- `CDBL(n)`
- `SGN(n)`

### 13.3 Random Number Functions

- `RND(n)` shall return a pseudorandom value dependent on the supplied argument.
- `RANDOM` shall seed the random number generator using a host-appropriate source.

### 13.4 Binary Conversion Functions

The following conversion functions shall be supported in Level II:

- `CVI(string)`
- `CVS(string)`
- `CVD(string)`
- `MKI$(value)`
- `MKS$(value)`
- `MKD$(value)`

### 13.5 Keyboard and Input Functions

- `INKEY$` shall return a buffered keystroke if available; otherwise it shall return the empty string.
- `INPUT$` shall return a string read from the input source of the requested length.

---

## 14. Error Handling

### 14.1 Error Representation
A Level II BASIC error shall be reported in the standard BASIC form and shall be associated with a numeric error code and a line location where applicable.

### 14.2 Standard Error Codes
The implementation shall recognize the following core Level II error categories, at minimum:

- `NF` NEXT without FOR
- `SN` syntax error
- `RG` RETURN without GOSUB
- `OD` out of data
- `FC` illegal function call
- `OV` numeric overflow
- `OM` out of memory
- `UL` undefined line
- `BS` subscript out of range
- `DD` redimensioned array
- `/0` division by zero
- `ID` illegal direct mode usage
- `TM` type mismatch
- `OS` out of string space
- `LS` string too long
- `ST` string formula too complex
- `CN` cannot continue
- `NR` no resume
- `RW` resume without error
- `UE` unprintable error
- `MO` missing operand
- `FD` bad file data

### 14.3 `ERR` and `ERL`
The variables `ERR` and `ERL` shall be available to the runtime environment when an error is handled. `ERR` shall contain the error code, and `ERL` shall contain the line number associated with the error condition.

### 14.4 Error Trapping
When `ON ERROR GOTO` is active, the implementation shall transfer execution to the specified line instead of terminating the program immediately.

### 14.5 Resume Semantics
The `RESUME` statement shall restart execution after a trapped error. `RESUME` with no argument shall resume at the failing statement, `RESUME NEXT` shall resume after the failing statement, and `RESUME line` shall resume at the specified line.

### 14.6 Invalid `CONT`
Executing `CONT` outside a valid stop or break context shall produce a runtime error.

### 14.7 Data-Read Errors
Attempting to read beyond the available `DATA` values shall raise an `OD` error. Reading a nonnumeric value into a numeric variable shall produce the relevant type or syntax diagnostic defined by the implementation.

---

## 15. Arrays and Dimensions

### 15.1 Declaration
The `DIM` statement shall allocate an array with the specified dimensions.

### 15.2 Dimensions
An implementation shall permit one-dimensional and two-dimensional arrays in the standard Level II model. Additional dimensions may be supported only if the implementation explicitly defines them.

### 15.3 Index Validation
Every array element access shall be validated against the declared bounds for each index. A subscript outside the valid range shall produce a runtime error.

### 15.4 Implicit Arrays
If an array is referenced before declaration and the implementation permits implicit array creation, the array shall be created with the default dimension and initialized according to the implementation's default value rules.

---

## 16. Input and Output Model

### 16.1 Screen Geometry
The standard display shall be treated as a 64-column by 16-row text display. Cursor positions shall be addressed in row-major order.

### 16.2 `PRINT` Semantics
`PRINT` shall output the values in the expression list in the order specified. Values separated by semicolons shall be printed without an intervening tab stop unless the implementation defines a host-specific spacing rule. Values separated by commas shall be printed to the next tab stop.

### 16.3 `?` Shorthand
The `?` token shall be equivalent to `PRINT`.

### 16.4 `INPUT`
`INPUT` shall display an optional prompt and then read one or more values from the keyboard and assign them to the named variables.

### 16.5 Printer Output
The `LPRINT` statement shall direct output to the printer or equivalent host output device.

---

## 17. Memory and Host Model

### 17.1 Program Storage
The implementation shall maintain the current program in memory as numbered source lines.

### 17.2 Variable Storage
Numeric and string variables shall occupy the implementation's variable storage area. String values shall use dynamic allocation or equivalent heap storage.

### 17.3 String Heap Size
The size of the available string heap shall be determined by the implementation and may be affected by the `CLEAR` command.

### 17.4 Host Interface
The host environment may provide keyboard, display, memory, printer, and graphics services. A conforming implementation may approximate unsupported hardware devices but shall preserve the language-level semantics of Level II BASIC.

---

## 18. Implementation-Defined Features

18.1 The following aspects are implementation-defined and shall be documented by the implementation:

- exact formatting of floating-point output;
- the source of the random number seed;
- host-specific printer and graphics behavior;
- the exact diagnostic wording for semantically equivalent error conditions;
- the default string heap size when no explicit `CLEAR` value is supplied;
- treatment of host-specific hardware interfaces that are not part of the core language.

18.2 An implementation shall not alter the semantics of the language constructs defined in this standard merely because a host device provides a different physical capability.

---

## 19. Reserved Words and Keywords

The following keywords are part of the standard Level II language surface:

`ABS`, `AND`, `ASC`, `ATN`, `AUTO`, `CDBL`, `CHR$`, `CINT`, `CLEAR`, `CLS`, `CONT`, `COS`, `CSNG`, `DATA`, `DEFDBL`, `DEFINT`, `DEFSNG`, `DEFSTR`, `DELETE`, `DIM`, `EDIT`, `ELSE`, `END`, `ERL`, `ERR`, `EXP`, `FIX`, `FOR`, `FRE`, `GOSUB`, `GOTO`, `IF`, `INKEY$`, `INPUT`, `INPUT$`, `INT`, `INSTR`, `LEFT$`, `LEN`, `LET`, `LIST`, `LLIST`, `LOG`, `LPRINT`, `MEM`, `MERGE`, `MID$`, `NEW`, `NEXT`, `NOT`, `ON`, `OR`, `OUT`, `PEEK`, `POINT`, `POKE`, `POS`, `PRINT`, `RANDOM`, `READ`, `REM`, `RESET`, `RESTORE`, `RESUME`, `RETURN`, `RIGHT$`, `RND`, `RUN`, `SAVE`, `SET`, `SGN`, `SIN`, `SQR`, `STEP`, `STOP`, `STR$`, `STRING$`, `SYSTEM`, `TAB`, `TAN`, `THEN`, `TO`, `TRON`, `TROFF`, `USR`, `VAL`, `VARPTR`

---

## 20. Compatibility Notes

20.1 The Level II language differs from earlier Level I BASIC in the following major respects:

- explicit type suffixes `%`, `!`, and `#`;
- explicit type declarations (`DEFINT`, `DEFSNG`, `DEFDBL`, `DEFSTR`);
- arrays and `DIM` semantics;
- enhanced string functions and binary conversion routines;
- explicit error trapping with `ON ERROR` and `RESUME`;
- additional built-in functions and control-flow constructs.

20.2 This specification presents the historical Level II language as a standard, while preserving the repository's compatibility focus and the distinction between the original hardware semantics and the modern implementation model.

---

## 21. Summary of Normative Requirements

21.1 A conforming Level II BASIC implementation shall support:

1. integer, single-precision, double-precision, and string data types;
2. the variable naming, declaration, and suffix rules defined in this specification;
3. arithmetic, relational, and logical expressions with the specified precedence;
4. the standard program control and program management statements;
5. `DATA`, `READ`, and `RESTORE` semantics;
6. the Level II built-in functions defined in this specification;
7. BASIC-style runtime error handling and `ERR`/`ERL` semantics;
8. documented host-dependent behavior where the physical environment differs from the abstract language model.

---

## 22. Annex A — Minimal Example Program

```basic
10 DEFINT A
20 DIM B(10)
30 A = 5
40 FOR I = 1 TO 5
50 B(I) = A * I
60 NEXT I
70 PRINT "SUM = "; B(1) + B(2) + B(3) + B(4) + B(5)
80 END
```

This example demonstrates declaration, array storage, loop control, and output semantics.

---

## 23. Annex B — Keyword Index

`ABS, AND, ASC, ATN, AUTO, CDBL, CHR$, CINT, CLEAR, CLS, CONT, COS, CSNG, DATA, DEFDBL, DEFINT, DEFSNG, DEFSTR, DELETE, DIM, EDIT, ELSE, END, ERL, ERR, EXP, FIX, FOR, FRE, GOSUB, GOTO, IF, INKEY$, INPUT, INPUT$, INT, INSTR, LEFT$, LEN, LET, LIST, LLIST, LOG, LPRINT, MEM, MERGE, MID$, NEW, NEXT, NOT, ON, OR, OUT, PEEK, POINT, POKE, POS, PRINT, RANDOM, READ, REM, RESET, RESTORE, RESUME, RETURN, RIGHT$, RND, RUN, SAVE, SET, SGN, SIN, SQR, STEP, STOP, STR$, STRING$, SYSTEM, TAB, TAN, THEN, TO, TRON, TROFF, USR, VAL, VARPTR`

---

## 24. Historical Note

24.1 The historical TRS-80 Model I Level II BASIC reference describes a language that is more powerful than Level I BASIC and more structured than the earliest ROM-only BASIC implementations.

24.2 The present specification preserves the historical language behavior while organizing it as a formal, specification-oriented document. It therefore serves as a practical standard reference derived from the original language definition and the repository implementation, rather than as a literal transcription of ROM machine code.

---

## 25. Document Status

25.1 This document is a draft specification for TRS-80 Model I Level II BASIC.

25.2 It is intended as a formal reference for future refinement, compatibility testing, and implementation documentation as the Level II interpreter work continues.

#### `IF expression THEN statement [ELSE statement]`
Conditionally execute one statement or a block of statements depending on the truth value of the expression.

#### `FOR variable = start TO end [STEP increment]`
Initialize the loop variable and set up a loop body.

#### `NEXT [variable ...]`
Advance the loop variable and repeat if the loop condition remains true.

#### `STOP`
Stop execution at the current line and report the BREAK state.

#### `END`
Terminate execution without the `BREAK IN line` message.

#### `CONT`
Resume execution after a stop or break condition, if valid.

### 11.3 Program Management

#### `NEW`
Erase all program text and variables from memory.

#### `CLEAR [size]`
Clear variables and optionally set the string heap size.

#### `RUN [line]`
Begin execution at the first program line or at the specified start line.

#### `LIST [start] [- end]`
List program lines to the display.

#### `LLIST [start] [- end]`
List program lines to the printer.

#### `LOAD filename`
Load a program from mass storage, replacing the current program state.

#### `SAVE filename`
Save the current program to mass storage.

#### `MERGE filename`
Merge a program file into the current program memory.

#### `DELETE line`
Delete a single numbered line.

#### `DELETE start-end`
Delete a closed range of lines.

### 11.4 Data Statements

#### `DATA value [, value ...]`
Store fixed data values in the program as static data.

#### `READ variable [, variable ...]`
Read successive values from the stored `DATA` list into the variables specified.

#### `RESTORE [line]`
Reset the DATA pointer to the beginning of the data list or to the first matching DATA item at or after the given line number.

### 11.5 Input and Output

#### `PRINT expression-list`
Print one or more values to the screen.

#### `? expression-list`
Equivalent to `PRINT`.

#### `INPUT [prompt;] variable [, variable ...]`
Read values from the keyboard and assign them to the specified variables.

#### `LPRINT expression-list`
Print to the printer.

#### `TAB(n)`
Move the print cursor to a column position on the current line.

#### `SPC(n)`
Print a specified number of spaces.

#### `POS(x)`
Return the current horizontal cursor position.

#### `CSRLIN`
Return the current row number.

### 11.6 Graphics and Host Access

#### `SET (x, y)`
Turn on the pixel at coordinates `(x, y)`.

#### `RESET (x, y)`
Turn off the pixel at coordinates `(x, y)`.

#### `POINT (x, y)`
Return a nonzero value if the specified pixel is set, else zero.

#### `POKE address, value`
Write a byte value to a memory location.

#### `PEEK(address)`
Return the byte at the specified address.

#### `MEM`
Return the amount of free memory in bytes.

#### `VARPTR(variable)`
Return the memory address associated with a variable.

#### `USR(address)`
Call a machine-language routine through the host interface.

### 11.7 Type Declarations and Array Handling

#### `DIM variable(size)`
Declare an array and allocate its elements.

#### `DIM variable(size1, size2)`
Declare a two-dimensional array.

Array indices shall be checked against the declared bounds; out-of-range indices shall cause a runtime error.

### 11.8 Error Trapping

#### `ON ERROR GOTO line`
Enable runtime error handling and redirect execution to the specified line number.

#### `ON ERROR GOTO 0`
Disable error trapping.

#### `RESUME [NEXT | line]`
Resume after a trapped error.

Error handling shall preserve the error location and error code for the current runtime fault.

---

## 12. Built-in Functions

The following built-in functions are part of the Level II BASIC language family.

### 12.1 String Functions

- `ASC(string)` returns the ASCII code of the first character.
- `CHR$(n)` returns a single-character string for the ASCII value `n`.
- `LEFT$(string, n)` returns the leftmost `n` characters.
- `RIGHT$(string, n)` returns the rightmost `n` characters.
- `MID$(string, start [, length])` returns a substring.
- `LEN(string)` returns the string length.
- `STR$(value)` converts a numeric value to a string.
- `VAL(string)` converts a prefix of characters to a numeric value.
- `STRING$(count, char)` returns a repeated-character string.
- `INSTR(start, haystack, needle)` searches for a substring.
- `UCASE$(string)` converts letters to uppercase.
- `LCASE$(string)` converts letters to lowercase.
- `LTRIM$`, `RTRIM$`, `TRIM$` trim whitespace or edge content where supported.

### 12.2 Numeric Functions

- `ABS(n)` absolute value
- `SQR(n)` square root
- `SIN(n)` sine
- `COS(n)` cosine
- `TAN(n)` tangent
- `ATN(n)` arctangent
- `EXP(n)` exponential
- `LOG(n)` natural logarithm
- `INT(n)` greatest integer less than or equal to `n`
- `FIX(n)` integer toward zero
- `CINT(n)` cast to integer
- `CSNG(n)` cast to single precision
- `CDBL(n)` cast to double precision
- `SGN(n)` sign function

### 12.3 Random Number Functions

- `RND(n)` returns a pseudorandom value depending on the supplied argument.
- `RANDOM` seeds the random generator using a host-dependent source.

### 12.4 Binary Conversion Functions

- `CVI(string)` converts a two-byte binary integer string to a value.
- `CVS(string)` converts a four-byte single-precision binary string.
- `CVD(string)` converts an eight-byte double-precision binary string.
- `MKI$(value)` converts an integer-like value to a two-byte binary string.
- `MKS$(value)` converts a single-precision value to a four-byte binary string.
- `MKD$(value)` converts a double-precision value to an eight-byte binary string.

### 12.5 Keyboard and Input Functions

- `INKEY$` returns a pressed key if one is buffered, otherwise the empty string.
- `INPUT$` reads a fixed number of characters from the input stream.

---

## 13. Error Handling

### 13.1 Error Reporting Model
The implementation shall report errors in a BASIC-like form, using the standard Level II error categories. Examples include:

- `SN ERROR` syntax error
- `NF ERROR` NEXT without FOR
- `RG ERROR` RETURN without GOSUB
- `OD ERROR` out of data
- `OV ERROR` numeric overflow
- `UL ERROR` undefined line
- `BS ERROR` subscript out of range
- `/0 ERROR` division by zero
- `TM ERROR` type mismatch
- `FC ERROR` illegal function call
- `ID ERROR` illegal direct mode operation
- `CN ERROR` cannot continue
- `NR ERROR` no resume

### 13.2 Error Trapping
When `ON ERROR GOTO` is active, runtime errors shall transfer control to the designated line, where `ERR` and `ERL` provide the error code and source line information.

### 13.3 `ERL` and `ERR`
- `ERR` holds the numeric error code.
- `ERL` holds the line number associated with the offending statement or source context.

### 13.4 Resume Rules
The `RESUME` statement shall restart execution after the handled error. `RESUME` with no argument recommences at the offending statement. `RESUME NEXT` continues after the offending statement. `RESUME line` transfers control to the specified line.

### 13.5 Invalid State Errors
An attempt to `CONT` when execution was not interrupted by a stop or break condition is an error.

### 13.6 Data Read Errors
Reading beyond the end of the `DATA` list shall raise an `OD ERROR`. Reading a nonnumeric value into a numeric variable shall produce the appropriate type or syntax diagnostic as defined by the implementation.

---

## 14. Arrays and Dimensions

### 14.1 Array Declaration
The `DIM` statement shall specify one or more array sizes. Arrays may be one-dimensional or two-dimensional, and the implementation may allow additional dimensions depending on the language profile.

### 14.2 Default Dimension
An array not explicitly declared shall use the default dimension if the implementation permits implicit creation.

### 14.3 Bounds
Indices shall be checked against the declared bounds for each dimension. Access outside the legal range shall produce a runtime subscript error.

### 14.4 Implicit Arrays
An array may be created implicitly when it is first referenced. Unless explicitly initialized, numeric elements are zero and string elements are empty strings.

---

## 15. Input/Output Model

### 15.1 Screen  
The display is a 64-column by 16-row text display. The upper-left corner is position 0, with row-major addressing used for `PRINT @`, `TAB`, and cursor-related output operations.

### 15.2 PRINT Semantics
PRINT values in sequence. Values separated by a semicolon are printed without intervening spaces; values separated by a comma are printed to the next tab stop. A trailing separator causes the next PRINT to continue in the current print position.

### 15.3 `?` Shorthand
`?` is equivalent to `PRINT`.

### 15.4 `INPUT`
The `INPUT` statement prints an optional prompt and reads one or more values from the keyboard. Multiple values may be entered on one line or across multiple prompts.

### 15.5 Printer Output
`LPRINT` and `LLIST` route output to the printer or print device if one is attached.

---

## 16. Memory and Host Model

### 16.1 Program Memory
The implementation stores the current program as numbered lines in memory.

### 16.2 Variable Memory
Numeric and string variables occupy memory in the dynamic variable area. Strings are heap allocated; numeric values use the machine's selected storage representation.

### 16.3 String Heap Size
The default string heap size and the size set by `CLEAR` shall be implementation-defined but shall be at least sufficient to support the language's string operations.

### 16.4 Host Interface
The host environment may provide keyboard, printer, graphics, and memory access through the BASIC interpreter. A conforming implementation may approximate hardware operations if the host does not provide a physical TRS-80 device.

---

## 17. Implementation-Defined Behavior

The following aspects are implementation-defined and may vary by an implementation or host environment:

- exact formatting of floating-point output
- the exact value of the random generator seed
- host-specific printer and graphics behavior
- the precise text of some diagnostics when equivalent errors are reported
- the exact handling of unsupported hardware and machine-specific interfaces
- the default string heap size used when no explicit value is provided to `CLEAR`

A conforming Level II BASIC implementation shall document those differences and ensure that core language semantics remain intact.

---

## 18. Reserved Words and Keywords

The following keywords are part of the language surface commonly associated with TRS-80 Model I Level II BASIC:

`ABS`, `AND`, `ASC`, `ATN`, `AUTO`, `CDBL`, `CHR$`, `CINT`, `CLEAR`, `CLS`, `CONT`, `COS`, `CSNG`, `DATA`, `DEFDBL`, `DEFINT`, `DEFSNG`, `DEFSTR`, `DELETE`, `DIM`, `EDIT`, `ELSE`, `END`, `ERL`, `ERR`, `ERROR`, `EXP`, `FIX`, `FOR`, `FRE`, `GOSUB`, `GOTO`, `IF`, `INKEY$`, `INPUT`, `INPUT$`, `INT`, `INSTR`, `LEFT$`, `LEN`, `LET`, `LIST`, `LLIST`, `LOG`, `LPRINT`, `MEM`, `MERGE`, `MID$`, `NEW`, `NEXT`, `NOT`, `ON`, `OR`, `OUT`, `PEEK`, `POINT`, `POKE`, `POS`, `PRINT`, `RANDOM`, `READ`, `REM`, `RESET`, `RESTORE`, `RESUME`, `RETURN`, `RIGHT$`, `RND`, `RUN`, `SAVE`, `SET`, `SGN`, `SIN`, `SQR`, `STEP`, `STOP`, `STR$`, `STRING$`, `SYSTEM`, `TAB`, `TAN`, `THEN`, `TO`, `TRON`, `TROFF`, `USR`, `VAL`, `VARPTR`

This list is representative; Level III disk BASIC and host-specific additions may introduce additional reserved names, but those names are outside the strict core Level II language unless explicitly supported by the implementation.

---

## 19. Compatibility Notes

This specification represents the standard language of the TRS-80 Model I Level II BASIC reference. It differs from the earlier Level I profile in the following principal ways:

- variable type suffixes `%`, `!`, and `#`
- direct type declarations (`DEFINT`, `DEFSNG`, `DEFDBL`, `DEFSTR`)
- arrays and `DIM`
- string functions and binary conversion functions
- stronger runtime typing and expression semantics
- explicit error-trapping with `ON ERROR` and `RESUME`
- additional built-in functions and control-flow operators

The project in this repository is a compatibility-oriented interpreter that includes a Level I baseline and adds Level II features incrementally. The standard in this document therefore defines the intended external behavior of the language rather than only the partial implementation state of any one codebase.

---

## 20. Summary of Normative Requirements

A conforming Level II BASIC implementation shall:

1. support integer, single-precision, double-precision, and string data types;
2. accept the standard variable naming and declaration rules;
3. implement the standard arithmetic, relational, and logical operators;
4. provide statement-level execution control and program management;
5. support `DATA`/`READ`/`RESTORE`, `FOR`/`NEXT`, `GOTO`/`GOSUB`, `ON`, `IF`, and `ERROR` handling;
6. provide the standard built-in functions described above;
7. produce BASIC error diagnostics in a recognizable, machine-compatible form;
8. define and document any host-dependent behavior not specified by the core language.

---

## 21. Annex A: Minimal Example Program

```basic
10 DEFINT A
20 DIM B(10)
30 A = 5
40 FOR I = 1 TO 5
50 B(I) = A * I
60 NEXT I
70 PRINT "SUM = "; B(1) + B(2) + B(3) + B(4) + B(5)
80 END
```

This example demonstrates type declaration, array storage, loop control, and output semantics.

---

## 22. Annex B: Canonical Keyword Index

`ABS, AND, ASC, ATN, AUTO, CDBL, CHR$, CINT, CLEAR, CLS, CONT, COS, CSNG, DATA, DEFDBL, DEFINT, DEFSNG, DEFSTR, DELETE, DIM, EDIT, ELSE, END, ERL, ERR, EXP, FIX, FOR, FRE, GOSUB, GOTO, IF, INKEY$, INPUT, INPUT$, INT, INSTR, LEFT$, LEN, LET, LIST, LLIST, LOG, LPRINT, MEM, MERGE, MID$, NEW, NEXT, NOT, ON, OR, OUT, PEEK, POINT, POKE, POS, PRINT, RANDOM, READ, REM, RESET, RESTORE, RESUME, RETURN, RIGHT$, RND, RUN, SAVE, SET, SGN, SIN, SQR, STEP, STOP, STR$, STRING$, SYSTEM, TAB, TAN, THEN, TO, TRON, TROFF, USR, VAL, VARPTR`

---

## 23. Historical Note

The TRS-80 Model I Level II BASIC reference describes a language that is more powerful than Level I BASIC and more structured than the minimal ROM-only programming environment of the earliest microcomputers. This standardization effort preserves the historical language behavior while organizing it according to a modern, specification-oriented layout.

The document intentionally emphasizes lexical and semantic portability, because the original BASIC manuals and ROM implementations were not written as ISO-like standards. The present specification is therefore a practical reference model derived from the original reference and the repository implementation, not a literal transcription of the ROM machine code.

---

## 24. Document Status

Status: Draft specification for TRS-80 Model I Level II BASIC.

This file is intended as a canonical source for further refinement, compatibility testing, and implementation documentation as the Level II interpreter work continues.
