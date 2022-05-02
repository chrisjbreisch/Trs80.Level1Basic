10 input "Vertical position (0 to 47)"; y
20 cls
30 for x = 0 to 127
40   for j = 0 to 47
50     set(x,j)
60   next j
70 next x
80 for x = 0 to 127
90   reset(x,y)
100 next x
999 goto 999