10 y=1:n=0
20 in. "Do you wish to light the block (y/n);q
30 cls
32 x = 75
34 z = 20
40 if q = 0 goto 80
50 set(x,z)
60 goto 100
80 reset(x,z)
100 if point(x,z) = 1 p.at200,x;z;"is lit"
110 if point(x,z) = 0 p.at200,x;z;"is dark"
999 goto 999