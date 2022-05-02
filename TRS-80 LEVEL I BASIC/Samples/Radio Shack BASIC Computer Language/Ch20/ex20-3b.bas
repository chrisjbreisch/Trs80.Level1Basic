10 input " Horizontal starting point (0 to 127)"; x
20 input " Vertical starting oint (0 to 47)";y
30 cls
40 reset (x+1,y)
50 set (x,y)
60 x = x-1
70 if x>= 0 then 40
80 x = x + 128
90 goto 40
99 goto 99
