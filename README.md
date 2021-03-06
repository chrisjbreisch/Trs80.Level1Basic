# TRS-80 LEVEL I BASIC

## What is this?

It is an interpreter for Radio Shack's TRS-80 Level I BASIC written in C# 10 and .NET 6.0. 
It's an **interpreter**, not an emulator. More on that in a bit.

## Why?

Several reasons:
1. I've long been a "compiler junkie". I've been fascinated by compilers and interpreters 
for as long as I can remember.
2. Nostalgia. The very first computer I ever owned was a Radio Shack TRS-80 Model I Level II. 
So, why not do a Level II interpreter? Well, I plan to at some point. But even though the 
computer was a Model I Level II, we got it with a programming book that described Level I BASIC.
This was David A. Lien's famous (infamous?) book: 
[Radio Shack BASIC Computer Language It's Easier Than you think!](https://archive.org/details/Basic_Computer_Language_Its_Easier_Than_You_Think_1978_Radio_Shack),
a copy of which is embedded in the project. (This book was also released as 
[User's Manual for Level 1](https://archive.org/details/Level_1_Users_Manual_1977_David_Lien?msclkid=146b1fdecec911eca73d4773ce1df1c3).)
I was perusing the internet one day and I came across this book again. I discovered I could buy a copy. 
So, I did. But then I wanted to do the examples in the book. There are lots of TRS-80 emulators 
and interpreters around on the net, and some of them are pretty good. I clearly didn't need to write 
a new one. But it felt incomplete somehow.
3. I like to code for fun and to learn. While there isn't much in this code that's likely to help me 
much at my job, it does help me think about problem solving, and it allows me to code in a 
low-pressure relaxed way. I've been working on this code off and on for about two years, and it's just
now in a state where I feel like I can bring it public.

## How?

After I got David Lien's book, I decided that I wanted to write an interpreter. I dug up some of my 
old textbooks on the subject. But they seemed inadequeate. They weren't written in the days of 
Object Oriented programming and were in C (or even Pascal!), and really showed their age. So, I 
started perusing the net and came across this wonderful website
[Crafting Interpreters](http://craftinginterpreters.com/) owned and created by Robert Nystrom.
There's a book, too: 
[Crafting Interpreters](https://smile.amazon.com/gp/product/0990582930/ref=ppx_od_dt_b_asin_title_s00?ie=UTF8&psc=1),
currently the #1 Best Seller in 
[Software Programming Compilers](https://smile.amazon.com/gp/bestsellers/books/3971/ref=zg_b_bs_3971_1)
on [Amazon](https://www.amazon.com). The code here is heavily based upon the ideas from the website.
The book wasn't available when I started writing my code. I just bought it yesterday myself. It hasn't 
arrived yet. I'm sure it's very good, though.

Robert walks you through everything you need to know to craft an interpreter for his newly created
language "lox". From a crafting perspective, lox is quite a bit easier than BASIC, particularly Level
I BASIC, which has a rather bizarre grammar. However, I was able to shoehorn Level I BASIC into 
Robert's concepts. I later discovered that one of the reasons Tandy was able to have such a bizarre grammar
is that their interpreter included no scanner. They skipped directly to the parsing phase. With no tokens, 
they were free to ascribe different meanings to the same values based upon their position in a statement. 
You can do that with a scanner also, but sometimes it gets messy. On the other hand, Level I BASIC has no
objects or scope to worry about. Robert spends quite a bit of time on that, which I was able to mostly ignore.

Note that Robert compiles his language down to the bytecode level. And shows you how to create a virtual 
machine and do garbage collection, optimization, etc. I did not do that for my BASIC interpreter. I stuck
with a simple recursive descent parser and an AST tree walker for the interpreter. AST tree walkers are 
about the slowest interpreters one could reasonably create. But they're also simple to write, easy to debug,
and easy for someone perusing the code to understand. Even my nice simple tree-walking interpeter is 
about 4,000 times faster than my TRS-80 was. And that's with some artificial slowness that I added. 
This is primarily logging, but also the addition of [Workflow Core](https://www.nuget.org/packages/WorkflowCore/),
which is complete overkill for this application, but I wanted to play with it. I don't really feel the 
need to optimize further.

With the few exceptions noted below, it's a complete implementation of Level I BASIC right down to the 
three error messages. I've run every program and example in the book (and supplied the code for you), 
and they all produce exactly the output expected. My error messages do supply a tiny bit 
more detail than the originals. I produce the original one, then underneath it there's usually 
some more helpful text in square brackets ([]). Hopefully this will help a little.

## What does it do?

Again, it's an interpreter, not an emulator. I didn't want to write something to mimic the hardware 
of a TRS-80. I'm a compiler junkie, not a hardware junkie. Also, that would have gotten into 
mimicking the TRS-80 ROM and assembly language. I haven't programmed in assembly language in 30+
years, and I didn't particularly enjoy it then. I wanted to work with more modern tools. 

This means that the language isn't represented completely 100%. People did some crazy things with 
their code back then, mostly in Level II, but even some in Level I, where they found clever ways 
to access the system hardware through a language that was never designed to do that. If you dig up
one of those old programs and try to run it on my interpreter, it will fail. It may run, but it 
won't do what you expect.

Level I BASIC was designed to fit in 4K of ROM, so they cut a few corners. One of them was that 
many standard math library functions such as Power, Logarithm, and Trig functons weren't included. 
People wrote their own, in BASIC, that you could access as subroutines. In fact, Appendix A from 
the book contains a decent set. I do find the POWER subroutine to be inadequate. Mostly because it 
builds upon the LOGARITHM subroutine and the EXP subroutine. Each of these have a small amount of 
error, causing POWER to have a slightly larger error. If I can, I plan on implementing a better 
one. So far, no luck. But I've included all the code from Appendix A essentially as they are in 
the book for now. I did fix a couple of minor errors.

There are two commands that I didn't implement: `CLOAD` and `CSAVE`. These load and save programs 
to and from the TRS-80's cassette drive. Yes, you read that right. We had a standard little 
cassette player and plugged into the computer. It loaded programs at about 60 characters per second, 
which probably converts to about 2 or 3 lines of code per second. The cassette tapes were usually
useless after about 5 or 6 uses, so you were constantly making backups. It was painful. It would 
take a half hour just to load up a decently sized program.

In the place of `CLOAD` and `CSAVE`, I've implemented `LOAD`, `SAVE`, and `MERGE`. `LOAD` and `SAVE` 
are analagous to `CLOAD` and `CSAVE`, but they use the disk drives. `MERGE` is a creation of my own.
`MERGE` loads these subroutines mentioned above and add them to the code already created in the interpreter.
It differs from `LOAD` in that `LOAD` destroys whatever is currently in memory.

When the applications starts, it displays the standard READY prompt, and you just start typing code.

It looks like this:

>READY  
>\>_

And, for exmple, here's the first program from the book:
>\>10 PRINT "HELLO THERE. I AM YOUR NEW TRS-80 MICROCOMPUTER."

To run the program, just type `RUN`. Which will produce the following:
>\>RUN
>
>HELLO THERE. I AM YOUR NEW TRS-80 MICROCOMPUTER.  
>  
>READY  
>\>_

## Extensions to Level I BASIC
- The `LOAD`, `SAVE`, and `MERGE` commands mentioned above
- The `CHR$(n)` function. It takes an ASCII value and returns the corresponding character. When I was 
playing around, I needed to print a double quote ("). There's no way to do that in Level I BASIC. 
`CHR$(n)` is part of Level II BASIC, so I'm just getting a head start on the Level II interpreter. :)
- When you type, your text is converted to UPPER case automatically, except if it's between quotes (").
In this instance, both the UPPER case and the original are stored, but only the original is displayed.
The primary purpose of this is so that `LOAD`, `MERGE`, and `SAVE` will work on case-sensitive file systems.
It's not a perfect solution, and adding this feature has doubtlessly introduced some new bugs.
- Level I BASIC had exactly 29 variables. Variables could only be 1 letter long (A-Z). There were two 
string variables (A$, B$), and one array(A()). I've expanded on that __slightly__. I still only allow 
1 letter variables, but all of them can also be strings or arrays. So `10 C$="Chris"` is legal. So is 
`10 F(10) = 3.14159`.
- I'm sure I've read details on the floating-point implementation on the TRS-80 somewhere, but I can't find it now. 
It's all 32-bit single-precision in Level I, and that's what I've implemented also. However, the TRS-80 
implementation predates [IEEE 754](https://en.wikipedia.org/wiki/IEEE_754) and suffers from rounding error
in some cases that don't apply to modern computers. Some examples in the book display the issues with 
these rounding errors. My emulator will display the numbers with more accuracy.
- I've given you 16 K of RAM. The original Level I's had 4K, but 16K models were available. It's just a 
number for the emulator though, so that `PRINT MEM` works. It has no meaning. Feel free to write 
programs as large as you like. The numbers displayed by `PRINT MEM` will drop accordingly, and 
eventually go negative, but you can keep on typing.
- I'm probobaly not as strict on expressions as Level I BASIC was. I allow full expressions anywhere. 
Thus, `10 A=100 : GOSUB A` is legal, but is not in Tandy's original BASIC.
- The random number generator is much more random than the original. This is something I may correct in time.
- String variables could only hold 16 characters. Mine have no such limits.
- Slightly more detailed error messages.

## What doesn't it do?

I haven't supplied an editor or any editing tools. You want to edit, do that outside and use `LOAD` 
to bring in what you've written. Otherwise, you can type the code in by hand just like I did way back in 1978.

## What's next?
As I said above, my first computer was actually a Radio Shack TRS-80 Model I Level II. I intend to fork 
this code and modify it to handle Level II BASIC. That will take some time and will never be fully 
implemented. Level II gives you more access to the ROM & RAM, which I won't emulate. It also introduces static
typing, which will take a bit of rewrite. Level I BASIC was ahead of its time. It used dynamic typing. 
Level II's 'system has a limited line editor, which I will try to emulate, but might be a bit challenging.
The other "new" features should be easier to deal with.

## Contents
- Visual Studio 2022 solution with all the code. I'm sure it can be built in Visual Studio Code, but 
I've not attempted it. If I get the time, I'll work on that and supply directions.
- David Lien's book in PDF form in the Radio Shack BASIC Computer Language folder.
- The "Another Man's Treasure" font to make you feel you're really using a TRS-80. This font is from
a collection of 22 fonts which can be found [here](https://www.kreativekorp.com/software/fonts/trs80.shtml).
I am using "Another Mans Treasure MIB 64C 2X3Y", which most closely resembles the one used on
my TRS-80 Model I Level II. This font can be found in the root folder. I recommend you install it
to get the true "feel" of using the TRS-80. The font is not installed for you automatically. You'll
have to do it yourself.
- All of the samples and exercises from the book, excluding the progams in Part C, which are mostly 
worthless. This includes the subroutines in Appendix A with my slight modifications. All of the 
subroutines have built-in unit tests which can be executed by loading the subroutine (`LOAD`) and 
typing `run 32000`. Be aware that some subroutines depend upon others. You'll have to load those as 
well with `MERGE`. You'll get the unit tests for both subroutines after a `MERGE`.
- Caveat: Programs that depend on timing are likely not going to work very well without some 
refactoring. On the original TRS-80, the book advises you that a tight loop counting to 500 will take 
about one second. Even in my interpreter running in debug mode, I was about to create a tight loop that 
counted to 2,000,000 that ran in about one second. YMMV. I have replaced the delays in the code in 
most places, but for some it just doesn't work. And the graphics programs just expect your computer to 
be slow. There are no delays built in. These samples are in the 
Trs80.Level1Basic/Samples/Radio Shack BASIC Computer Language folder. They are organized by chapter and appendix.

## Bugs?
I'm sure that there are some. I have over 300 unit tests, but the appropriate amount is probably
closer to 1,000. For what it's worth, [dotCover](https://www.jetbrains.com/dotcover/) says that my unit
tests cover about 85% of the code. I'm not sure how useful a metric that is for something like an 
interpreter, but at least it's significantly greater than 0. 

There are no unit tests at all for the AST generator, and very few for the Workflow. Those are pulling 
the score down a bit. Even still, it's not just exercising the code that's important for an interpreter, 
it's also the sequence. Does it behave properly with nested FOR loops, or nested GOSUBs? Do successive 
GOTOs work? Can you use variables as indexes to arrays? These are the kinds of questions unit tests must
anser.

Also, as far as I know, there's no published formal spec, complete with error message formatting for 
Level I BASIC. So, in the end, the results are what I think is accurate. I may be wrong. It's possible 
that both the code and the unit tests are wrong. Time will tell.


# TL;DR
If you don't want to read the book, and just want to play, I'm mimicking the final pages of the book here,
which should be just enough to get you started.

## Summary of LEVEL 1 BASIC

| **Command**  |   **Purpose**                                          |   **Exanple**                 |   **Detailed in Chapter(s)**   |
|----------|----------------------------------------------------|--------------------------------|:--------------------------:|
| NEW      | Clears out all program lines stored in memory      | NEW (not part of program)      | 1                          |
| RUN      | Starts program execution at lowest-numbered line   | RUN (not part of program)      | 1                          |
| RUN ###  | Starts program execution at specified line number. | RUN 300 (not part of program)  | 11                         |
| LIST     | Displays the first 12 program lines stored in memory, starting at lowest numbered line. Use up arrow key to display higher numbered lines (if any)  | LIST (not part of program)     | 2                          |
| LIST ### | Same as LIST, but starts at specific line number   | LIST 300 (not part of program) | 11                         |
| CONT     | Continues program execution when BREAK AT ### is displayed | CONT (not part of program)     | 11                         |




| **Statement**  |   **Purpose**                                          |   **Exanple**                 |   **Detailed in Chapter(s)**   |
|----------|----------------------------------------------------|--------------------------------|:--------------------------:|
| PRINT    | Print value of a variable or expression; also prints whatever is inside quotes | 10 PRINT "A+B=";A+B              | 1,2,3                      |
| INPUT    | Tells Computer to let you enter data from the Keyboard. | 10 INPUT A,B,C | 7 |
| INPUT    | Also has built-in PRINT capablity | 10 INPUT "ENTER A";A | 7 |
| READ     | Reads data in DATA statement                       | 10 READ A,B,C,A$ | 16 |
| DATA     | Holds data to be read by READ statement            | 20 DATA 1,2,3,"SALLY" | 16 |
| RESTORE  | Causes next READ statement to start with first item in first DATA line | 30 RESTORE | 16 |
| LET      | (Optional) Assign a new value to variable on left of equals sign | 0 LET A=3.14159 | 2 |
| GOTO     | Transfers program control to designated program line | 10 GOTO 100 | 6 |
| IF-THEN  | Establishes a test point                           | 10 IF A=B THEN 300 | 6 |
| FOR-NEXT | Sets up a do-loop to be executed a specific number of times | 10 FOR I=1 to 10 | 10,11,13 |
|          |                                                    | 20 NEXT I | |
| STEP     | Specifies size of increment to be used in FOR-NEXT loops | 10 FOR I=0 to 10 STEP 2 | 10 |
| STOP     | Stops program execution and prints BREAK AT ### message | 10 IF A<B STOP | 11 |
| END      | Ends program execution and set program counter to zero | 99 END | 2 |
| GOSUB    | Transfers program control to subroutine beginning at specified line | 10 GOSUB 3000 | 15,25 |
| RETURN   | End subroutine execution and returns control to GOSUB line | 3010 RETURN | 15,25 |
| ON       | Multi-way branch used with GOTO and GOSUB          | 10 ON N GOTO 30, 40, 50 | 15 |
|          |                                                    | 10 ON N GOSUB 3000,4000,5000 | 15 | 

| **Print Modifiers** |   **Purpose**                                          |   **Exanple**                 |   **Detailed in Chapter(s)**   |
|-------------|----------------------------------------------------|--------------------------------|:--------------------------:|
| AT          | (Follows PRINT) Begins print at specified location on Display | 10 PRINT AT 650 "HELLO"              | 22                      |
| TAB         | (Follows PRINT) Begins print at specified number of spaces from left margin | 10 PRINT TAB(10);"MONTH";TAB(20);"RECEIPTS"| 12                      |

| **Graphics Statements** |   **Purpose**                                          |   **Exanple**                 |   **Detailed in Chapter(s)**   |
|-------------|----------------------------------------------------|--------------------------------|:--------------------------:|
| SET         | Lights up a specified location on Display          | 10 SET(30,40)                  | 20,22                      |
| RESET       | Turns off a specified graphics location on Display | 20 RESET(30,40)                | 20,22                      |
| POINT       | Check the specified graphics location. If point is "on", returns a 1; if "off", return a 0. | 30 IF POINT(30,40)=1 then PRINT 'ON'| 22                      |
| CLS         | Turns off all graphics locations (clears screen)   | 10 CLS                         | 10,20                      |

| **Built-In Functions** |   **Description**                               |   **Exanple**                 |   **Detailed in Chapter(s)**   |
|-------------|-----------------------------------------|--------------------------------|:--------------------------:|
| MEM         | Returns the number of free bytes left in memory | 10 PRINT MEM                   | 8 |
| INT(X)      | Returns the greatest integer less than or equal to X (-32768< x <32768) | 10 I=INT(Y)                    | 14 |
| ABS(X)      | Absolute value of X                     | 10 M=ABS(A)                    | 17 |
| RND(0)      | Returns a random number between 0 and 1 | 10 X=RND(0)                    | 19 |
| RND(N)      | Returns a random number betwween 1 and N (1 <= N < 32768) | 10 X=RND(500)                 | 19 |

| **Math Operators** |   **Function**                           |   **Exanple**                 |   **Detailed in Chapter(s)**   |
|-------------|------------------------------------|--------------------------------|:--------------------------:|
| +           | Addition                           | A+B                            | 3 |
| -           | Subtraction                        | A-B                            | 3 |
| *           | Multiplication                     | A*B                            | 3 |
| /           | Division                           | A/B                            | 3 |
| =           | Assigns value of right-hand side to variable on left-hand side | A=B                           | 3 |

| **Relational Operators** |   **Relationship**                           |   **Exanple**                 |   **Detailed in Chapter(s)**   |
|-------------|------------------------------------|-------------------------------|:--------------------------:|
| <           | Is less than                       | A<B                           | 6 |
| >           | Is greater than                    | A>B                           | 6 |
| =           | Is equal to                        | A=B                           | 6 |
| <=          | Is less than or equal to           | A<=B                          | 6 |
| >=          | Is greater than or equal to        | A>=B                          | 6 |
| <>          | Is not equal to                    | A<>B                          | 6 |

| **Logical Operators** |   **Function**                           |   **Exanple**                 |   **Detailed in Chapter(s)**   |
|-------------|------------------------------------|-------------------------------|:--------------------------:|
| *           | AND                                | (A=3)*(B=7) "A equals 3 and B equals 7" | 24 |
| +           | OR                                 | (A=3)+(B=7) "A equals 3 or B equals 7" | 24 |

| **Variables** |   **Purpose**                           |   **Exanple**                 |   **Detailed in Chapter(s)**   |
|-------------|------------------------------------|-------------------------------|:--------------------------:|
| A through Z | Take on number values              | A=3.14159 | 3 |
| A$ and B$   | Take on string values (up to 16 characters) | A$=RADIO SHACK | 16 |
| A(X)        | Store the elements of a one-dimensional array (X <= MEM/4-1) | A(0)=400 | 21 |

## LEVEL I Shorthand Dialect
| **Command/Statement** | **Abbreviation** | **Command/Statement** | **Abbreviation** |
|-------------------|--------------|-------------------|--------------|
| PRINT             | P.           | TAB (after PRINT) | T.           |
| NEW               | N.           | INT               | I.           |
| RUN               | R.           | GOSUB             | GOS.         |
| LIST              | L.           | RETURN            | RET.         |
| END               | E.           | READ              | REA.         |
| THEN              | T.           | DATA              | D.           |
| GOTO              | G.           | RESTORE           | REST.        |
| INPUT             | IN.          | ABS               | A.           |
| MEM               | M.           | RND               | R.           |
| FOR               | F.           | SET               | S.           |
| NEXT              | N.           | RESET             | R.           |
| STEP (after FOR)  | S.           | POINT             | P.           |
| STOP              | ST.          | PRINT AT          | P.A.         |
| CONT              | C.           |                   |              |

