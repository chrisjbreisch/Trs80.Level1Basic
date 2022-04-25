10 input "Horizontal starting point (0 to 127)"; x
20 input "Vertical starting point (0 to 47)"; y
30 cls
40 reset (x, y+47)
50 set (x, y)
60 y=y+1
70 goto 40
99 goto 99