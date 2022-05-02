10 input " Horizontal starting point (0 to 127)"; x
20 input " Vertical starting oint (0 to 47)";y
30 cls
40 reset (x,y+1)
50 set (x,y)
60 y = y-1
70 if y>= 0 then 40
80 y = y + 48
90 goto 40
99 goto 99
