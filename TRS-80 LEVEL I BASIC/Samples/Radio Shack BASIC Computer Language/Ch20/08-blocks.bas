10 input "Horizontal starting point (0 to 127)";x
20 input "Vertical starting oint (0 to 47)";y
30 input "Length of each side (in blocks) -- (0 to 47)";k
40 cls
50 for l = x to x+k
60   set(l,y)
70   set(l,y+k)
80 next l
90 for m = y to y+k
100   set(x,m)
110   set(x+k,m)
120 next m
999 goto 999