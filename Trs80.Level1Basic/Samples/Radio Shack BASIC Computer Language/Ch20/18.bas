10 input "Vertical Address (0 to 47)"; y
20 input "Step size"; s
30 cls
40 for x = 0 to 127 step s
50   set (x, y)
55   reset (x,y-1)
60 next x
70 y = y + 1
80 goto 40
