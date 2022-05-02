10 cls
20 k = 900
30 f.x=1to59
40    p.atk+x, ".";
50 n.x: k = 964
60 for y = 0 to 13
70    p.aty*64+5, ".";
80 next y
100 p.at20,"G R A P H  H E A D I N G"
150 f.n = 0 to 14
200    p.atn*64,14-n;
250 n.n
300 f.x = 0 to 5
310    p.at k+10*x,x;
320 n.x
400 f.x=6to56 step 10
410    p.at k+x,":";
420 n.x
999 g.999