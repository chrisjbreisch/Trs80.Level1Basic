# TRS-80 LEVEL I BASIC

## What is this?

It is an interpreter for Radio Shack's TRS-80 Level I BASIC written in C# 10 and .NET 6.0. It's an **interpreter**, not an emulator. More on that in a bit.

## Why?

Several reasons:
1. I've long been a "compiler junkie". I've been fascinated by compilers and interpreters for as long as I can remember.
2. Nostalgia. The very first computer I ever owned was a Radio Shack TRS-80 Model I Level II. 
So, why not do a LEVEL II interpreter? Well, I plan to at some point. But even though the computer was a Model I Level II,
we got it with a programming book that described Level I BASIC. This was David A. Lien's famous (infamous?) book: 
[Radio Shack BASIC Computer Language It's Easier Than you think!](https://archive.org/details/Basic_Computer_Language_Its_Easier_Than_You_Think_1978_Radio_Shack),
a copy of which is embedded in the project. I was perusing the internet one day and I came across this book again. I discovered I could buy a copy. So, I did do.
But then I wanted to do the examples in the book. There are lots of TRS-80 emulators  and interpreters around on the net, and some of them are pretty good.
I clearly didn't need to write a new one. But it felt incomplete somehow.
3. I like to code for fun and to learn. While there isn't much in this code that's likely to help me much at my job, it
does help me think about problem solving, and it allows me to code in a low-pressure relaxed way. I've been working on this 
code off and on for about two years, and it's just now in a state where I feel like I can bring it public.

## How?

After I got David Lien's book, and decided I wanted to write an interpreter, I dug up some of my old textbooks on the subject, but they seemed inadequeate.
They weren't written in the days of Object Oriented programming and were in C (or even Pascal!), and really showed their age. So, I started perusing the net
and came across this wonderful website [Crafting Interpreters](http://craftinginterpreters.com/) owned and created by Robert Nystrom. There's a book, too: 
[Crafting Interpreters](https://smile.amazon.com/gp/product/0990582930/ref=ppx_od_dt_b_asin_title_s00?ie=UTF8&psc=1), currently the #1 Best Seller in 
[Software Programming Compilers](https://smile.amazon.com/gp/bestsellers/books/3971/ref=zg_b_bs_3971_1) on [Amazon](https://www.amazon.com). The code
here is heavily based upon the ideas from the website. The book wasn't available when I started writing my code. I just bought it yesterday myself. It hasn't 
arrived yet. I'm sure it's very good, though.

Robert walks you through everything you need to know to craft an interpreter for his newly created language "lox". From a crafting perspective, lox is quite
a bit easier than BASIC, particularly Level I BASIC, which has a rather bizarre grammar. However, I was able to shoehorn Level I BASIC into Robert's concepts.

With the few exceptions noted below, it's a complete implementation of Level I BASIC right down to the three error messages. I've run every program and example
in the book (and supplied the code for you), and they all produce exactly the output expected. I think there are two examples of error messages where I supply
the correct error message, but the formatting isn't perfect. One of those isn't very hard to fix. The other would be a pain. If it was an actual formatting 
problem with output from the program, I would fix it. But I can live with an error message that just has a question mark in the wrong place. Also, my 
error messages supply a tiny bit more detail than the originals. I produce the original one, then underneath it there's usually some more helpful text in square
brackets ([]). Hopefully this will help a little.

## What does it do?

As I said earlier, it's an interpreter, not an emulator. I didn't want to write something to mimic the hardware of a TRS-80. I told you, I'm a compiler junkie.
I'm not a hardware junkie. Also, that would have gotten into mimicking the TRS-80 ROM and assembly language. I haven't programmed in assembly language in 30+
year, and I didn't particularly enjoy it then. As I said earlier, I wanted to work with modern tools. 

This means that the language isn't represented completely 100%. People did some crazy things with their code back then, mostly in Level II, 
but even some in Level I, where they found clever ways to access the system hardware through a language that was never designed to do that. If you did up
one of those old programs and try it run it on my interpreter, it will fail. It may run, but it won't do what you expect.

There are two commands that I didn't implement: `CLOAD` and `CSAVE`. These load and save programs to and from the TRS-80's cassette drive. Yes, you read that
right. We had a standard little cassette player and plugged into the computer. It loaded programs at about 500 bits per second, and the cassette tapes were 
usually useless after about 5 or 6 uses, so you were constantly making backups. It was painful. 
It would take a half hour just to load up a decently sized program.

In the place of `CLOAD` and `CSAVE`, I've given you `LOAD`, `SAVE`, and `MERGE`. `LOAD` and `SAVE` are analagous to `CLOAD` and `CSAVE`, but they use
your disk drives. `MERGE` is a creation of my own. Level I BASIC was designed to fit in 4K of ROM, so they cut a few corners. One of them was that 
many standard math library functions, Power, Logarithm, and Trig functons weren't included. People wrote their own, in BASIC, that you could access
as subroutines. In fact, Appendix A from the book contains a decent set. I've included all the code for these. `MERGE` lets you load these subroutines and add
them to the code you already have. It differs from `LOAD` in that `LOAD` destroys whatever is currently in memory.

## What doesn't it do?

I haven't supplied an editor or any editing tools. You want to edit, do that outside and use `LOAD` to bring in what you've written. It may not be an emulator, but 
it looks like one. You get the standard READY prompt, and you just start typing code.

It looks like this:

> READY
> >_

And, for exmple, here's the first program from the book:
> >10 PRINT "HELLO THERE. I AM YOUR NEW TRS-80 MICROCOMPUTER."

Which will of course output:
> HELLO THERE. I AM YOUR NEW TRS-80 MICROCOMPUTER.
>
> READY
> >_
