10 input "Horizontal starting point (0 to 127)"; x
20 input "Vertical starting point (0 to 47)"; y
30 cls
40 reset (x+126, y)
50 set (x, y)
60 x=x+1
70 goto 40
99 goto 99