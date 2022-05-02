10 input "Horizontal starting point (0 to 127)"; x
20 input "Vertical starting point (0 to 47)"; y
30 input "Lower barrier";k
40 cls
50 for m=0 to 127
60   set (m,k)
70 next m
80 reset (x,y+47)
90 set(x,y)
100 y=y+1
110 if y<48 then 130
120 y=y-48
130 if y<>k then 80
999 goto 999
    