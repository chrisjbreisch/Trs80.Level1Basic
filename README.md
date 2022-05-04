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
[Software Programming Compilers](https://smile.amazon.com/gp/bestsellers/books/3971/ref=zg_b_bs_3971_1) on (Amazon) [https://www.amazon.com].