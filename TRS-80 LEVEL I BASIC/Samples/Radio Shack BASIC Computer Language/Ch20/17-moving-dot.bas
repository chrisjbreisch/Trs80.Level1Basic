10 cls
20 for m=0 to 127
30   set (m,0)
40   set (m,47)
50 next m
60 y=14
70 d=1
80 reset (64, y+48-d)
90 set (64, y)
100 y=y+d
110 if y=48 then 130
120 if y<>-1 then 80
130 y=y-2*d
140 d=-d
150 goto 90
999 goto 999
