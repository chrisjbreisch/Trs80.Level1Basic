10 input "Vertical Address (0 to 47)"; y
20 input "Step size"; s
30 cls
40 for x = 0 to 127 step s
50   set(x, y)
60 next x
99 goto 99